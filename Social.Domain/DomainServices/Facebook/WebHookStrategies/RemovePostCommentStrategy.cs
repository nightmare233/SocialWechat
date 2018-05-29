using System.Threading.Tasks;
using Social.Domain.Entities;
using Social.Infrastructure.Facebook;
using Social.Infrastructure.Enum;
using Framework.Core;

namespace Social.Domain.DomainServices.Facebook
{
    public class RemovePostCommentStrategy : WebHookStrategy
    {
        public RemovePostCommentStrategy(IDependencyResolver resolver) : base(resolver)
        {
        }

        public override bool IsMatch(FbHookChange change)
        {
            return change.Field == "feed"
                && change.Value.PostId != null
                && change.Value.CommentId != null
                && change.Value.Item == "comment"
                && change.Value.Verb == "remove";
        }

        public async override Task<FacebookProcessResult> Process(SocialAccount socialAccount, FbHookChange change)
        {
            var result = new FacebookProcessResult(NotificationManager);
            var sources = new[] { MessageSource.FacebookPostComment, MessageSource.FacebookPostReplyComment };
            var message = GetMessage(sources, change.Value.CommentId);
            if (message != null)
            {
                await DeleteMessage(message);
                result.WithDeletedMessage(message);
            }

            return result;
        }
    }
}
