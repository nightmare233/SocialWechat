using System.Threading.Tasks;
using Social.Domain.Entities;
using Social.Infrastructure.Facebook;
using Social.Infrastructure.Enum;
using Framework.Core;
using System.Linq;

namespace Social.Domain.DomainServices.Facebook
{
    public class NewCommentStrategy : WebHookStrategy
    {
        public NewCommentStrategy(IDependencyResolver resolver) : base(resolver)
        {

        }

        public override bool IsMatch(FbHookChange change)
        {
            return change.Field == "feed"
                && change.Value.PostId != null
                && change.Value.CommentId != null
                && change.Value.Item == "comment"
                && change.Value.Verb == "add";
        }

        public async override Task<FacebookProcessResult> Process(SocialAccount socialAccount, FbHookChange change)
        {
            var result = new FacebookProcessResult(NotificationManager);
            string token = socialAccount.Token;
            if (IsDuplicatedMessage(change.Value.CommentId))
            {
                return result;
            }

            FbComment comment = FbClient.GetComment(socialAccount.Token, change.Value.CommentId);
            SocialUser sender = await GetOrCreateFacebookUser(socialAccount.Token, comment.from.id);

            var conversation = GetConversation(change.Value.PostId);
            if (conversation == null)
            {
                return result;
            }

            Message message = FacebookConverter.ConvertToMessage(comment);
            message.SenderId = sender.Id;
            var parent = GetParent(change.Value.PostId, comment);
            if (parent != null)
            {
                message.ParentId = parent.Id;
                message.ReceiverId = parent.SenderId;
                bool ifParentIsComment = parent.ParentId != null;
                if (ifParentIsComment)
                {
                    message.Source = MessageSource.FacebookPostReplyComment;
                }
            }

            message.ConversationId = conversation.Id;
            conversation.IfRead = false;
            conversation.Status = sender.Id != socialAccount.Id ? ConversationStatus.PendingInternal : ConversationStatus.PendingExternal;
            conversation.LastMessageSenderId = message.SenderId;
            conversation.LastMessageSentTime = message.SendTime;

            if (conversation.TryToMakeWallPostVisible(socialAccount))
            {
                result.WithNewConversation(conversation);
            }

            conversation.Messages.Add(message);

            await UpdateConversation(conversation);
            await CurrentUnitOfWork.SaveChangesAsync();

            if (!conversation.IsHidden)
            {
                result.WithNewMessage(message);
            }

            return result;
        }

        private Message GetParent(string postId, FbComment comment)
        {
            Message parent;
            if (comment.parent == null)
            {
                parent = GetMessage(MessageSource.FacebookPost, postId);
            }
            else
            {
                parent = GetMessage(MessageSource.FacebookPostComment, comment.parent.id);
            }

            return parent;
        }
    }
}
