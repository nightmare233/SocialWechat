using Framework.Core;
using Social.Domain.Entities;
using Social.Infrastructure;
using Social.Infrastructure.Enum;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Tweetinvi.Models;

namespace Social.Domain.DomainServices.Twitter
{
    public interface ITwitterTweetService
    {
        Task<TwitterProcessResult> ProcessTweet(SocialAccount currentAccount, ITweet currentTweet);
    }

    public class TwitterTweetService : ServiceBase, ITwitterTweetService
    {
        private IConversationService _conversationService;
        private IMessageService _messageService;
        private ISocialUserService _socialUserService;
        private ISocialAccountService _socialAccountService;
        private ITweetTreeWalker _twitterTreeWalker;
        private INotificationManager _notificationManager;

        public TwitterTweetService(
            IConversationService conversationService,
            IMessageService messageService,
            ISocialUserService socialUserService,
            ISocialAccountService socialAccountService,
            INotificationManager notificationManager,
            ITweetTreeWalker twitterTreeWalker
            )
        {
            _conversationService = conversationService;
            _messageService = messageService;
            _socialUserService = socialUserService;
            _socialAccountService = socialAccountService;
            _twitterTreeWalker = twitterTreeWalker;
            _notificationManager = notificationManager;
        }

        public async Task<TwitterProcessResult> ProcessTweet(SocialAccount currentAccount, ITweet currentTweet)
        {
            TwitterProcessResult result = new TwitterProcessResult(_notificationManager);

            // ignore if first tweet was sent by integration account.
            // 集成账号创建的第一条tweet不需要创建conversation.
            if (IsFirstTweetCreateByCurrentIntegrationAccount(currentAccount, currentTweet))
            {
                return result;
            }

            // ignore if tweet was created by customer but didn't mention current account.
            // 客户创建的Tweet, 但是没有@当前集成账号，不需要创建conversation.
            if (IsTweetCreatedByAnotherButDoNotMentionIntegrationAccount(currentAccount, currentTweet))
            {
                return result;
            }

            // ignore if tweet was sent at agent console.
            // tweet是在访客监控页面由agent回复的，则不需要再次处理
            if (IsSendAtAgentConsole(currentTweet))
            {
                return result;
            }

            IList<SocialAccount> allAccounts = await _socialAccountService.GetAccountsAsync(SocialUserSource.Twitter);
            // ignore if tweet created by another integration account.
            // tweet由其他集成账号创建，其他账号的任务会处理，不需要重复处理请求。
            if (IsCreateByOtherIntegrationAccount(currentAccount, allAccounts, currentTweet))
            {
                return result;
            }
            // ignore if first tweet was sent by other integration account.
            // 集成账号创建的第一条tweet不需要创建conversation.
            if (IsFristTweetCreateByOtherIntegrationAccount(currentAccount, allAccounts, currentTweet))
            {
                return result;
            }
            // ignore if tweet mentioned multiple integration account 
            // and other integration account should process this tweet in case we process the same tweet again;
            // 客户创建的Tweet, 但是@了多个集成账号，让优先级高（按@顺序）的集成账号处理, 防止数据重复被处理。
            if (IsTweetCreatedByAnotherAndMentionedAnotherIntegrationAccount(currentAccount, allAccounts, currentTweet))
            {
                return result;
            }

            List<ITweet> tweets = _twitterTreeWalker.BuildTweetTree(currentTweet);
            return await AddTweets(currentAccount, allAccounts, tweets);
        }

        private bool IsCreateByOtherIntegrationAccount(SocialAccount currentAccount, IList<SocialAccount> allAccounts, ITweet currentTweet)
        {
            return currentTweet.CreatedBy.IdStr != currentAccount.SocialUser.OriginalId
                && allAccounts.Any(t => t.SocialUser.OriginalId == currentTweet.CreatedBy.IdStr);
        }

        private bool IsFirstTweetCreateByCurrentIntegrationAccount(SocialAccount currentAccount, ITweet currentTweet)
        {
            bool isFirstTweet = currentTweet.InReplyToStatusId == null;
            return isFirstTweet && currentTweet.CreatedBy.IdStr == currentAccount.SocialUser.OriginalId;
        }

        private bool IsFristTweetCreateByOtherIntegrationAccount(SocialAccount currentAccount, IList<SocialAccount> allAccounts, ITweet currentTweet)
        {
            var otherAccountOrginalIds = allAccounts.Where(t => t.Id != currentAccount.Id).Select(t => t.SocialUser.OriginalId);
            bool isFirstTweet = currentTweet.InReplyToStatusId == null;
            return isFirstTweet && otherAccountOrginalIds.Contains(currentTweet.CreatedBy.IdStr);
        }

        private bool IsTweetCreatedByAnotherButDoNotMentionIntegrationAccount(SocialAccount currentAccount, ITweet currentTweet)
        {
            bool isCreatedByAnother = currentTweet.CreatedBy.IdStr != currentAccount.SocialUser.OriginalId;
            bool isMentionMe = currentTweet.UserMentions.Any(t => t.IdStr == currentAccount.SocialUser.OriginalId);
            return isCreatedByAnother && !isMentionMe;
        }

        private bool IsTweetCreatedByAnotherAndMentionedAnotherIntegrationAccount(SocialAccount currentAccount, IList<SocialAccount> allAccounts, ITweet currentTweet)
        {
            bool isCreatedByAnother = currentTweet.CreatedBy.IdStr != currentAccount.SocialUser.OriginalId;
            bool shouldProcessByOtherIntegrationAccount = false;
            bool hasOtherIntegrationAccount =
                currentTweet.UserMentions.Any(t => t.IdStr != currentAccount.SocialUser.OriginalId && allAccounts.Any(a => a.SocialUser.OriginalId == t.IdStr));

            if (hasOtherIntegrationAccount)
            {
                var firstIntegrationAccount = currentTweet.UserMentions.FirstOrDefault(t => allAccounts.Any(a => a.SocialUser.OriginalId == t.IdStr));

                if (firstIntegrationAccount != null && firstIntegrationAccount.IdStr != currentAccount.SocialUser.OriginalId)
                {
                    shouldProcessByOtherIntegrationAccount = true;
                }
            }

            return isCreatedByAnother && shouldProcessByOtherIntegrationAccount;
        }

        private bool IsReplyToIrrelevantPerson(SocialAccount currentAccount, IList<SocialAccount> allAccounts, ITweet currentTweet, IList<ITweet> tweets)
        {
            bool isSendByIntegratioAccont = IsIntegrationAccount(currentTweet.CreatedBy.IdStr, allAccounts);
            if (isSendByIntegratioAccont)
            {
                var inReplyToStatus = tweets.FirstOrDefault(t => t.Id == currentTweet.InReplyToStatusId);
                if (inReplyToStatus != null)
                {
                    return inReplyToStatus.UserMentions.All(t => !allAccounts.Any(x => x.SocialUser.OriginalId == t.IdStr));
                }
            }
            return false;
        }

        private bool IsSendAtAgentConsole(ITweet currentTweet)
        {
            return _messageService.FindAll().Any(t => t.OriginalId == currentTweet.IdStr && t.SendAgentId != null);
        }

        private async Task<TwitterProcessResult> AddTweets(SocialAccount currentAccount, IList<SocialAccount> allAccounts, List<ITweet> tweets)
        {
            var result = new TwitterProcessResult(_notificationManager);
            if (!tweets.Any())
            {
                return result;
            }

            tweets = tweets.OrderByDescending(t => t.CreatedAt).ToList();

            // 如果是集成账号回复的客户的tweet，并且客户的tweet没有@任何集成账号，则不处理
            if (IsReplyToIrrelevantPerson(currentAccount, allAccounts, tweets.First(), tweets))
            {
                return result;
            }

            foreach (var tweet in tweets)
            {
                // if current integration account doesn't involved.
                if (!IsIntergrationAccountInvolved(tweet, allAccounts))
                {
                    continue;
                }

                // ignore if processed before.
                if (_messageService.FindAll().Any(t => t.OriginalId == tweet.IdStr))
                {
                    break;
                }

                await AddTweet(result, currentAccount, allAccounts, tweets, tweet);
            }

            return result;
        }

        private bool IsIntergrationAccountInvolved(ITweet tweet, IList<SocialAccount> allAccounts)
        {
            // tweet is created by current integration account or mentioned current integration account.
            return IsIntegrationAccount(tweet.CreatedBy.IdStr, allAccounts) || tweet.UserMentions.Any(t => IsIntegrationAccount(t.IdStr, allAccounts));
        }

        private async Task AddTweet(
            TwitterProcessResult result,
            SocialAccount currentAccount,
            IList<SocialAccount> allAccounts,
            List<ITweet> tweets,
            ITweet currentTweet)
        {
            var senderOriginalId = currentTweet.CreatedBy.IdStr;
            var receiverOrignalId = GetReceiverOriginalId(currentAccount, allAccounts, currentTweet);
            if (string.IsNullOrEmpty(receiverOrignalId))
            {
                return;
            }
            // integration account sent to intergration account
            // 集成账号@集成账号
            if (IsIntegrationAccount(senderOriginalId, allAccounts)
                && IsIntegrationAccount(receiverOrignalId, allAccounts))
            {
                List<string> customerOriginalIds = _twitterTreeWalker.FindCustomerOriginalIdsInTweetTree(currentTweet, tweets, allAccounts);
                if (!customerOriginalIds.Any())
                {
                    return;
                }

                await SaveTweet(result, currentAccount, allAccounts, currentTweet, tweets, senderOriginalId, receiverOrignalId, customerOriginalIds);
            }

            // integration account sent to customer
            // 集成账号@客户
            if (IsIntegrationAccount(senderOriginalId, allAccounts)
                && !IsIntegrationAccount(receiverOrignalId, allAccounts))
            {
                var customerOriginalIds = new List<string> { receiverOrignalId };
                await SaveTweet(result, currentAccount, allAccounts, currentTweet, tweets, senderOriginalId, receiverOrignalId, customerOriginalIds);
            }

            // customer sent to integration account
            // 客户@集成账号
            if (!IsIntegrationAccount(senderOriginalId, allAccounts)
                && IsIntegrationAccount(receiverOrignalId, allAccounts))
            {
                var customerOriginalIds = new List<string> { senderOriginalId };
                await SaveTweet(result, currentAccount, allAccounts, currentTweet, tweets, senderOriginalId, receiverOrignalId, customerOriginalIds);
            }
        }

        private async Task SaveTweet(
            TwitterProcessResult result,
            SocialAccount currentAccount,
            IList<SocialAccount> allAccounts,
            ITweet currentTweet,
            List<ITweet> tweets,
            string senderOriginalId,
            string receiverOriginalId,
            List<string> customerOrginalIds)
        {
            var sender = await _socialUserService.GetOrCreateTwitterUser(senderOriginalId);
            var receiver = await _socialUserService.GetOrCreateTwitterUser(receiverOriginalId);

            var message = TwitterConverter.ConvertToMessage(currentTweet);
            message.SenderId = sender.Id;
            message.ReceiverId = receiver.Id;

            await UnitOfWorkManager.RunWithNewTransaction(currentAccount.SiteId, async () =>
            {
                var conversations = await FindConversations(tweets, customerOrginalIds);
                await SaveConversations(result, currentAccount, allAccounts, conversations, message);
            });

        }

        private async Task<IList<Conversation>> FindConversations(IList<ITweet> tweets, List<string> customerOriginalIds)
        {
            List<string> tweetIds = tweets.Select(t => t.IdStr).ToList();
            return await _conversationService.FindAll().Include(t => t.Messages)
                  .Where(c => c.Messages.Any(m => tweetIds.Contains(m.OriginalId)))
                  .Where(c => c.Messages.Any(m => customerOriginalIds.Contains(m.Sender.OriginalId) || customerOriginalIds.Contains(m.Receiver.OriginalId)))
                  .ToListAsync();
        }

        private string GetReceiverOriginalId(SocialAccount account, IList<SocialAccount> socialAccounts, ITweet currentTweet)
        {
            List<string> accountOrignalIds = socialAccounts.Select(t => t.SocialUser.OriginalId).ToList();

            // if first tweet sent by integraton account and didn't mentioned any one, 
            // the recipient should be the integraiton account himself.
            if (!currentTweet.UserMentions.Any())
            {
                return account.SocialUser.OriginalId;
            }

            // if tweet sent by customer, the reciver should be the first integration account in the user mentions.
            if (!IsIntegrationAccount(currentTweet.CreatedBy.IdStr, socialAccounts))
            {
                return currentTweet.UserMentions.Where(t => accountOrignalIds.Contains(t.IdStr))
                    .Select(t => t.IdStr).FirstOrDefault();
            }
            // if tweet sent by integration account, the reciver should either be customer or the first integration account in the user mentions.
            else
            {
                var receiverOriginalId = currentTweet.UserMentions.Where(t => !accountOrignalIds.Contains(t.IdStr))
                    .Select(t => t.IdStr).FirstOrDefault();
                if (!string.IsNullOrEmpty(receiverOriginalId))
                {
                    return receiverOriginalId;
                }

                return currentTweet.UserMentions.Select(t => t.IdStr).FirstOrDefault();
            }
        }

        private bool IsIntegrationAccount(string originalId, IList<SocialAccount> socialAccounts)
        {
            return socialAccounts.Any(t => t.SocialUser.OriginalId == originalId);
        }

        private async Task SaveConversations(
            TwitterProcessResult result,
            SocialAccount currentAccount,
            IList<SocialAccount> allAccounts,
            IList<Conversation> conversations,
            Message message)
        {
            if (conversations.Any())
            {
                foreach (var conversation in conversations)
                {
                    if (conversation.Messages.Any(t => t.OriginalId == message.OriginalId))
                    {
                        continue;
                    }
                    message.ConversationId = conversation.Id;
                    conversation.Messages.Add(message);
                    if (message.SendTime > conversation.LastMessageSentTime)
                    {
                        conversation.IfRead = false;
                        bool isSendByIntegrationAccount = allAccounts.Any(t => t.Id == message.SenderId);
                        conversation.Status = isSendByIntegrationAccount ? ConversationStatus.PendingExternal : ConversationStatus.PendingInternal;
                        conversation.LastMessageSenderId = message.SenderId;
                        conversation.LastMessageSentTime = message.SendTime;
                    }
                    _conversationService.Update(conversation);
                    await CurrentUnitOfWork.SaveChangesAsync();
                    result.WithUpdatedConversation(conversation);
                    result.WithNewMessage(message);
                }
            }
            else
            {
                var conversation = new Conversation
                {
                    OriginalId = message.OriginalId,
                    Source = ConversationSource.TwitterTweet,
                    Priority = ConversationPriority.Normal,
                    Status = ConversationStatus.New,
                    Subject = TwitterUtil.GetSubject(message.Content),
                    LastMessageSenderId = message.SenderId,
                    LastMessageSentTime = message.SendTime
                };
                conversation.Messages.Add(message);
                _conversationService.AddConversation(currentAccount, conversation);
                await CurrentUnitOfWork.SaveChangesAsync();
                result.WithNewConversation(conversation);
            }
        }
    }
}
