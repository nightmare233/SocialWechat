using Framework.Core;
using Social.Domain.DomainServices.Twitter;
using Social.Domain.Entities;
using Social.Infrastructure;
using Social.Infrastructure.Enum;
using Social.Infrastructure.Facebook;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Transactions;
using Tweetinvi.Models;
using Message = Social.Domain.Entities.Message;

namespace Social.Domain.DomainServices
{
    public interface IMessageService : IDomainService<Message>
    {
        Message FindByOriginalId(MessageSource source, string originalId);
        Message FindByOriginalId(MessageSource[] sources, string originalId);
        IQueryable<Message> FindAllByConversationId(int conversationId);
        IQueryable<Message> FindAllByConversationIds(int[] conversationIds);
        bool IsDuplicatedMessage(MessageSource messageSource, string originalId);
        Message ReplyTwitterTweetMessage(int conversationId, int twitterAccountId, string message, bool isCloseConversation = false);
        Message ReplyTwitterDirectMessage(int conversationId, string message, bool isCloseConversation = false);
        Message ReplyFacebookMessage(int conversationId, string content, bool isCloseConversation = false);
        Message ReplyFacebookPostOrComment(int conversationId, int postOrCommentId, string content, bool isCloseConversation = false);
        IList<Message> ChangeAttachmentUrl(IList<Message> messages);
    }

    public class MessageService : DomainService<Message>, IMessageService
    {
        private ISocialAccountService _socialAccountService;
        private IConversationService _conversationService;
        private IFbClient _fbClient;

        public MessageService(
            ISocialAccountService socialAccountService,
            IConversationService conversationService,
            IFbClient fbClient
            )
        {
            _socialAccountService = socialAccountService;
            _conversationService = conversationService;
            _fbClient = fbClient;
        }

        public override IQueryable<Message> FindAll()
        {
            return base.FindAll().Where(t => t.IsDeleted == false);
        }

        public override Message Find(int id)
        {
            return base.FindAll().Where(t => t.IsDeleted == false && t.Id == id).FirstOrDefault();
        }

        public Message FindByOriginalId(MessageSource source, string originalId)
        {
            return FindAll().Where(t => t.OriginalId == originalId && t.Source == source).FirstOrDefault();
        }

        public Message FindByOriginalId(MessageSource[] sources, string originalId)
        {
            if (sources == null)
            {
                return null;
            }

            return FindAll().Where(t => t.OriginalId == originalId && sources.Contains(t.Source)).FirstOrDefault();
        }

        public IQueryable<Message> FindAllByConversationId(int conversationId)
        {
            return FindAll()
                .Include(t => t.Attachments)
                .Include(t => t.SendAgent)
                .Include(t => t.Sender.SocialAccount)
                .Include(t => t.Receiver.SocialAccount)
                .Where(t => t.ConversationId == conversationId);
        }

        public IQueryable<Message> FindAllByConversationIds(int[] conversationIds)
        {
            if (conversationIds == null || !conversationIds.Any())
            {
                return new List<Message>().AsQueryable();
            }

            return FindAll()
                .Include(t => t.Sender)
                .Include(t => t.Receiver)
                .Where(t => conversationIds.Contains(t.ConversationId));
        }

        public Message ReplyFacebookMessage(int conversationId, string content, bool isCloseConversation = false)
        {
            var conversation = _conversationService.CheckIfExists(conversationId);
            if (conversation.Source != ConversationSource.FacebookMessage)
            {
                throw SocialExceptions.BadRequest("Conversation source must be facebook message.");
            }

            _conversationService.CheckIfCanReopenWhenReply(conversation);

            var messages = Repository.FindAll().Include(t => t.Sender).Include(t => t.Receiver).Where(t => t.ConversationId == conversation.Id).ToList();
            SocialAccount socialAccount = GetSocialAccountFromMessages(messages);
            if (socialAccount == null)
            {
                throw SocialExceptions.BadRequest("Facebook integration account can't be found from conversation.");
            }

            SocialUser recipient = messages.Where(t => t.Sender.Type == SocialUserType.Customer)
                .OrderByDescending(t => t.SendTime)
                .Select(t => t.Sender).FirstOrDefault();

            // publish message to facebook
            string fbMessageId = _fbClient.PublishMessage(socialAccount.Token, conversation.OriginalId, content);
            if (string.IsNullOrWhiteSpace(fbMessageId))
            {
                return null;
            }
            // create message
            var message = new Message
            {
                ConversationId = conversation.Id,
                Content = content,
                SenderId = socialAccount.Id,
                ReceiverId = recipient != null ? recipient.Id : socialAccount.Id,
                SendAgentId = UserContext.UserId,
                SendTime = DateTime.UtcNow,
                OriginalId = fbMessageId,
                Source = MessageSource.FacebookMessage
            };

            Save(message, isCloseConversation);

            return this.Find(message.Id);
        }

        private void Save(Message message, bool isCloseConversation)
        {
            using (var uow = UnitOfWorkManager.Begin(TransactionScopeOption.RequiresNew))
            {
                Repository.Insert(message);

                // upadte conversation
                var conversation = _conversationService.Find(message.ConversationId);
                conversation.Status = isCloseConversation ? ConversationStatus.Closed : ConversationStatus.PendingExternal;
                conversation.LastMessageSenderId = message.SenderId;
                conversation.LastMessageSentTime = message.SendTime;
                conversation.LastRepliedAgentId = UserContext.UserId;
                _conversationService.Update(conversation);
                uow.Complete();
            }
        }

        public Message ReplyFacebookPostOrComment(int conversationId, int postOrCommentId, string content, bool isCloseConversation = false)
        {
            var conversation = _conversationService.CheckIfExists(conversationId);
            if (conversation.Source != ConversationSource.FacebookVisitorPost && conversation.Source != ConversationSource.FacebookWallPost)
            {
                throw SocialExceptions.BadRequest("Conversation source must be facebook visitor/wall post.");
            }

            var messages = Repository.FindAll().Include(t => t.Sender).Include(t => t.Receiver).Where(t => t.ConversationId == conversation.Id).ToList();
            var postMessage = messages.FirstOrDefault(t => t.ParentId == null);
            SocialAccount socialAccount = GetSocialAccountFromMessages(messages);
            if (socialAccount == null)
            {
                throw SocialExceptions.BadRequest("Facebook integration account can't be found from conversation.");
            }

            var previousMessages = GetFacebookPreviousMessages(messages, postOrCommentId);
            Message comment = null;
            foreach (var previousMessage in previousMessages)
            {
                if (previousMessage.IsDeleted)
                {
                    continue;
                }

                bool isReplyToReplyComment = previousMessage.ParentId != null && previousMessage.ParentId != postMessage.Id;
                if (isReplyToReplyComment)
                {
                    //todo @current message user
                    continue;
                }

                // publish comment
                string fbCommentId = _fbClient.PublishComment(socialAccount.Token, previousMessage.OriginalId, content);
                if (string.IsNullOrWhiteSpace(fbCommentId))
                {
                    continue;
                }

                var fbComment = _fbClient.GetComment(socialAccount.Token, fbCommentId);

                // add message
                comment = new Message
                {
                    ConversationId = conversation.Id,
                    Content = content,
                    SenderId = socialAccount.Id,
                    ReceiverId = previousMessage.SenderId,
                    ParentId = previousMessage.Id,
                    SendAgentId = UserContext.UserId,
                    SendTime = fbComment.created_time,
                    OriginalId = fbCommentId,
                    OriginalLink = fbComment.permalink_url,
                    Source = MessageSource.FacebookPostComment
                };
                if (comment.ParentId != postMessage.Id)
                {
                    comment.Source = MessageSource.FacebookPostReplyComment;
                }

                Save(comment, isCloseConversation);

                break;
            }

            if (comment == null)
            {
                throw SocialExceptions.OriginalPostOrTweetHasBeenDeleted();
            }

            return this.Find(comment.Id);
        }

        public Message ReplyTwitterDirectMessage(int conversationId, string message, bool isCloseConversation = false)
        {
            var twitterService = DependencyResolver.Resolve<ITwitterService>();

            Conversation conversation = _conversationService.CheckIfExists(conversationId);
            if (conversation.Source != ConversationSource.TwitterDirectMessage)
            {
                throw SocialExceptions.BadRequest("Conversation source must be twitter direct message.");
            }

            _conversationService.CheckIfCanReopenWhenReply(conversation);

            var messages = Repository.FindAll().Include(t => t.Sender).Include(t => t.Receiver).Where(t => t.ConversationId == conversation.Id).ToList();
            SocialAccount twitterAccount = GetSocialAccountFromMessages(messages);
            if (twitterAccount == null)
            {
                throw SocialExceptions.BadRequest($"Twitter account has been deleted.");
            }

            var customer = messages.Where(t => t.IsDeleted == false && t.Sender.Type == SocialUserType.Customer)
                .OrderByDescending(t => t.SendTime).Select(t => t.Sender).FirstOrDefault();
            if (customer == null)
            {
                throw new NotSupportedException($"Cant't find last message from conversation. Conversation Id={conversationId}.");
            }

            IUser prviousTwitterUser = twitterService.GetUser(twitterAccount, long.Parse(customer.OriginalId));
            if (prviousTwitterUser == null)
            {
                throw new NotSupportedException($"Cant't find twitter user. Conversation Id={conversationId}.");
            }

            // publish twitter direct message
            IMessage twitterDirectMessage = twitterService.PublishMessage(twitterAccount, prviousTwitterUser, message);

            // create message to db
            Message directMessage = TwitterConverter.ConvertToMessage(twitterDirectMessage);
            directMessage.ConversationId = conversation.Id;
            directMessage.SenderId = twitterAccount.Id;
            directMessage.SendAgentId = UserContext.UserId;
            directMessage.ReceiverId = customer.Id;

            Save(directMessage, isCloseConversation);

            return this.Find(directMessage.Id);
        }

        public Message ReplyTwitterTweetMessage(int conversationId, int twitterAccountId, string content, bool isCloseConversation = false)
        {
            var twitterService = DependencyResolver.Resolve<ITwitterService>();
            Conversation conversation = _conversationService.CheckIfExists(conversationId);
            if (conversation.Source != ConversationSource.TwitterTweet)
            {
                throw SocialExceptions.BadRequest("Conversation source must be twitter tweet.");
            }

            SocialAccount twitterAccount = _socialAccountService.FindAllWithDeleted().FirstOrDefault(t => t.Id == twitterAccountId);
            if (twitterAccount != null && twitterAccount.IsDeleted)
            {
                twitterAccount = _socialAccountService.FindAll().FirstOrDefault(t => t.SocialUser.OriginalId == twitterAccount.SocialUser.OriginalId);
            }
            if (twitterAccount == null)
            {
                throw SocialExceptions.BadRequest("Twitter account has been deleted.");
            }

            var messages = Repository.FindAll().Include(t => t.Sender).Include(t => t.Receiver).Where(t => t.ConversationId == conversation.Id).ToList();
            var previousMessages = messages.Where(t => t.SenderId != twitterAccountId).OrderByDescending(t => t.SendTime);
            Message replyMessage = null;
            foreach (var previousMessage in previousMessages)
            {
                if (previousMessage.IsDeleted)
                {
                    continue;
                }
                //if (previousMessage.Sender.Type == SocialUserType.IntegrationAccount)
                //{
                //    continue;
                //}

                ITweet previousTweet = twitterService.GetTweet(twitterAccount, long.Parse(previousMessage.OriginalId));
                if (previousTweet != null && !previousTweet.IsTweetDestroyed)
                {
                    // publish tweet
                    if (!content.Contains("@" + previousTweet.CreatedBy.ScreenName))
                    {
                        content = "@" + previousTweet.CreatedBy.ScreenName + " " + content;
                    }
                    var replyTweet = twitterService.ReplyTweet(twitterAccount, previousTweet, content);
                    if (replyTweet == null)
                    {
                        continue;
                    }

                    // add message
                    replyMessage = TwitterConverter.ConvertToMessage(replyTweet);
                    replyMessage.ConversationId = conversation.Id;
                    replyMessage.SenderId = twitterAccount.Id;
                    replyMessage.SendAgentId = UserContext.UserId;
                    replyMessage.ReceiverId = previousMessage.Sender.Id;

                    Save(replyMessage, isCloseConversation);

                    break;
                }
            }

            if (replyMessage == null)
            {
                throw SocialExceptions.OriginalPostOrTweetHasBeenDeleted();
            }

            return this.Find(replyMessage.Id);
        }


        public bool IsDuplicatedMessage(MessageSource messageSource, string originalId)
        {
            return Repository.FindAll().Any(t => t.Source == messageSource && t.OriginalId == originalId);
        }

        private SocialAccount GetSocialAccountFromMessages(IList<Message> messages)
        {
            var accounts = GetSocialAccountsFromMessages(messages);
            var account = accounts.FirstOrDefault(t => t.IsDeleted == false);
            if (account == null && accounts.Any())
            {
                //if intefration account was deleted and integrated again,we should use the new social account.
                var originalId = accounts.First().SocialUser.OriginalId;
                account = _socialAccountService.FindAll().Where(t => t.SocialUser.OriginalId == originalId).FirstOrDefault();
            }
            return account;
        }

        private IList<SocialAccount> GetSocialAccountsFromMessages(IList<Message> messages)
        {
            var accountsWhoSendMessage =
                messages.Where(t => t.Sender.Type == SocialUserType.IntegrationAccount)
                .Select(t => t.Sender.Id)
                .Distinct()
                .ToList();
            var accountsWhoReceiveMessage =
                messages.Where(t => t.Receiver != null && t.Receiver.Type == SocialUserType.IntegrationAccount)
                .Select(t => t.Receiver.Id)
                .Distinct()
                .ToList();
            List<int> accountIds = accountsWhoSendMessage.Union(accountsWhoReceiveMessage).Distinct().ToList();
            return _socialAccountService.FindAllWithDeleted().Where(t => accountIds.Contains(t.Id)).ToList();
        }

        private IList<Message> GetFacebookPreviousMessages(IList<Message> messages, int previousMessageId)
        {
            List<Message> previousMessages = new List<Message>();
            var prviousMessage = messages.FirstOrDefault(t => t.Id == previousMessageId);
            while (prviousMessage != null)
            {
                previousMessages.Add(prviousMessage);
                if (prviousMessage.ParentId != null)
                {
                    prviousMessage = messages.FirstOrDefault(t => t.Id == prviousMessage.ParentId);
                }
                else
                {
                    break;
                }
            }

            return previousMessages.OrderByDescending(t => t.SendTime).ToList();
        }

        public IList<Message> ChangeAttachmentUrl(IList<Message> messages)
        {
            foreach (var message in messages)
            {
                foreach (var attachment in message.Attachments)
                {
                    if (attachment.RawData != null)
                    {
                        string url = AppSettings.AttachmentUrl + $"/{attachment.Id}?siteId={UserContext.SiteId}";
                        attachment.Url = url;
                    }
                }
            }
            return messages;
        }
    }
}
