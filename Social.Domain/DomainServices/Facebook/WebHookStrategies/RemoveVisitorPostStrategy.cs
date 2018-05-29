using System.Threading.Tasks;
using Social.Domain.Entities;
using Social.Infrastructure.Facebook;
using Framework.Core;

namespace Social.Domain.DomainServices.Facebook
{
    public class RemoveVisitorPostStrategy : WebHookStrategy
    {
        public RemoveVisitorPostStrategy(IDependencyResolver resolver) : base(resolver)
        {
        }

        public override bool IsMatch(FbHookChange change)
        {
            return change.Field == "feed"
                && change.Value.PostId != null
                && change.Value.Item == "post"
                && change.Value.Verb == "remove";
        }

        public async override Task<FacebookProcessResult> Process(SocialAccount socialAccount, FbHookChange data)
        {
            var result = new FacebookProcessResult(NotificationManager);
            var conversation = GetConversation(data.Value.PostId);
            if (conversation != null)
            {
                await DeleteConversation(conversation);
                result.WithDeletedConversation(conversation);
            }

            return result;
        }
    }
}
