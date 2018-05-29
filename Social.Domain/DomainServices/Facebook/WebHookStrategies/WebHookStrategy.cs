using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Social.Domain.Entities;
using Social.Infrastructure.Facebook;
using Framework.Core;
using Social.Infrastructure.Enum;
using Social.Infrastructure;

namespace Social.Domain.DomainServices.Facebook
{
    public abstract class WebHookStrategy : ServiceBase, IWebHookSrategy
    {
        protected IConversationService ConversationService { get; set; }
        protected ISocialUserService SocialUserService { get; set; }
        protected IMessageService MessageService { get; set; }
        protected INotificationManager NotificationManager { get; set; }
        protected FacebookConverter FacebookConverter { get; set; }
        protected IFbClient FbClient { get; set; }

        public WebHookStrategy(IDependencyResolver dependencyResolver)
        {
            ConversationService = dependencyResolver.Resolve<IConversationService>();
            SocialUserService = dependencyResolver.Resolve<ISocialUserService>();
            MessageService = dependencyResolver.Resolve<IMessageService>();
            NotificationManager = dependencyResolver.Resolve<INotificationManager>();
            FbClient = dependencyResolver.Resolve<IFbClient>();
            FacebookConverter = new FacebookConverter();
        }


        public abstract bool IsMatch(FbHookChange change);

        public abstract Task<FacebookProcessResult> Process(SocialAccount socialAccount, FbHookChange change);

        protected string GetSubject(string message)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                return "No Subject";
            }

            return message.Length <= 200 ? message : message.Substring(200);
        }

        protected bool IsDuplicatedMessage(string originalId)
        {
            return MessageService.FindAll().Any(t => t.OriginalId == originalId);
        }

        protected bool IsDuplicatedMessage(MessageSource source, string originalId)
        {
            return MessageService.IsDuplicatedMessage(source, originalId);
        }

        protected Message GetMessage(MessageSource source, string originalId)
        {
            return MessageService.FindByOriginalId(source, originalId);
        }

        protected Message GetMessage(MessageSource[] sources, string originalId)
        {
            return MessageService.FindByOriginalId(sources, originalId);
        }

        protected async Task DeleteMessage(Message message)
        {
            await MessageService.DeleteAsync(message);
        }

        protected Conversation GetConversation(string originalId)
        {
            var conversations = ConversationService.FindAll().Where(t => t.OriginalId == originalId);
            return conversations.FirstOrDefault();
        }

        protected Conversation GetUnClosedConversation(string originalId)
        {
            return ConversationService.GetUnClosedConversation(originalId);
        }

        protected async Task UpdateConversation(Conversation conversation)
        {
            //   ConversationService.Update(conversation);

            await ConversationService.UpdateAsync(conversation);
        }

        protected async Task<Conversation> AddConversation(SocialAccount socialAccount, Conversation conversation)
        {
            if (socialAccount.ConversationDepartmentId.HasValue)
            {
                conversation.DepartmentId = socialAccount.ConversationDepartmentId.Value;
                conversation.Priority = socialAccount.ConversationPriority ?? ConversationPriority.Normal;
            }

            if (socialAccount.ConversationAgentId.HasValue)
            {
                conversation.AgentId = socialAccount.ConversationAgentId.Value;
                conversation.Priority = socialAccount.ConversationPriority ?? ConversationPriority.Normal;
            }

           return await ConversationService.InsertAsync(conversation);
        }

        protected async Task DeleteConversation(Conversation conversation)
        {
            await ConversationService.DeleteAsync(conversation);
        }

        protected async Task<SocialUser> GetOrCreateFacebookUser(string token, string fbUserId)
        {
            return await SocialUserService.GetOrCreateFacebookUser(token, fbUserId);
        }
    }
}
