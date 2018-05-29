using System;
using System.Threading.Tasks;
using Social.Domain.Entities;
using Social.Infrastructure.Facebook;
using Framework.Core;
using Social.Infrastructure.Enum;
using System.Linq;

namespace Social.Domain.DomainServices.Facebook
{
    public class NewPostStrategy : WebHookStrategy
    {
        public NewPostStrategy(IDependencyResolver resolver) : base(resolver)
        {
        }

        public override bool IsMatch(FbHookChange change)
        {
            bool isTextPost = change.Field == "feed"
                && change.Value.PostId != null
                && change.Value.Item == "post"
                && change.Value.Verb == "add";

            bool isPhotOrVideoPost = change.Field == "feed"
                && change.Value.PostId != null
                //&& change.Value.Item == "photo"
                && change.Value.Verb == "add"
                && !string.IsNullOrWhiteSpace(change.Value.Link)
                && change.Value.IsPublished;

            bool isWallPost = IsWallPost(change);

            return isTextPost || isPhotOrVideoPost || isWallPost;
        }

        private bool IsWallPost(FbHookChange change)
        {
            bool isWallPost = change.Field == "feed"
                && change.Value.PostId != null
                && change.Value.Item == "status"
                && change.Value.Verb == "add"
                && change.Value.IsPublished;

            return isWallPost;
        }

        public override async Task<FacebookProcessResult> Process(SocialAccount socialAccount, FbHookChange change)
        {
            var result = new FacebookProcessResult(NotificationManager);
            if (IsWallPost(change) && !socialAccount.IfConvertWallPostToConversation)
            {
                return result;
            }
            if (!IsWallPost(change) && !socialAccount.IfConvertVisitorPostToConversation)
            {
                return result;
            }

            string token = socialAccount.Token;
            if (IsDuplicatedMessage(MessageSource.FacebookPost, change.Value.PostId))
            {
                return result;
            }

            FbPost post = await FbClient.GetPost(socialAccount.Token, change.Value.PostId);
            SocialUser sender = await GetOrCreateFacebookUser(socialAccount.Token, post.from.id);

            Message message = FacebookConverter.ConvertToMessage(post);
            message.SenderId = sender.Id;
            if (message.SenderId != socialAccount.Id)
            {
                message.ReceiverId = socialAccount.Id;
            }
            var conversation = new Conversation
            {
                OriginalId = change.Value.PostId,
                Source = ConversationSource.FacebookVisitorPost,
                Priority = ConversationPriority.Normal,
                Status = ConversationStatus.New,
                Subject = GetSubject(message.Content),
                LastMessageSenderId = message.SenderId,
                LastMessageSentTime = message.SendTime
            };

            if (change.Value.Verb == "add" && message.SenderId == socialAccount.Id)
            {
                conversation.Source = ConversationSource.FacebookWallPost;
                conversation.IsHidden = true;
            }

            conversation.Messages.Add(message);
            await AddConversation(socialAccount, conversation);
            await CurrentUnitOfWork.SaveChangesAsync();

            if (!conversation.IsHidden)
            {
                result.WithNewConversation(conversation);
            }

            return result;
        }
    }
}
