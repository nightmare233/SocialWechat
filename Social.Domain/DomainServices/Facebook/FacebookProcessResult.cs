using Social.Domain.Entities;
using Social.Infrastructure;
using Social.Infrastructure.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Social.Domain.DomainServices.Facebook
{
    public class FacebookProcessResult
    {
        public IList<Conversation> NewConversations { get; set; }
        public IList<Conversation> UpdatedConversations { get; set; }
        public IList<Message> NewMessages { get; set; }
        public IList<Conversation> DeletedConversations { get; set; }
        public IList<Message> DeletedMessages { get; set; }

        private INotificationManager _notificationManager { get; set; }

        public FacebookProcessResult(
            INotificationManager notificationManager
            )
        {
            NewConversations = new List<Conversation>();
            UpdatedConversations = new List<Conversation>();
            NewMessages = new List<Message>();
            DeletedConversations = new List<Conversation>();
            DeletedMessages = new List<Message>();
            _notificationManager = notificationManager;
        }

        public void WithNewConversation(Conversation conversation)
        {
            if (NewConversations.Any(t => t.Id == conversation.Id))
            {
                return;
            }

            NewConversations.Add(conversation);
        }

        public void WithUpdatedConversation(Conversation conversation)
        {
            if (NewConversations.Any(t => t.Id == conversation.Id))
            {
                return;
            }

            if (UpdatedConversations.Any(t => t.Id == conversation.Id))
            {
                return;
            }

            UpdatedConversations.Add(conversation);
        }

        public void WithDeletedConversation(Conversation conversation)
        {
            DeletedConversations.Add(conversation);
        }

        public void WithNewMessage(Message message)
        {
            if (NewConversations.Any(t => t.Id == message.ConversationId))
            {
                return;
            }

            if (NewMessages.Any(t => t.Id == message.Id))
            {
                return;
            }

            NewMessages.Add(message);
        }

        public void WithDeletedMessage(Message message)
        {
            DeletedMessages.Add(message);
        }

        public async Task Notify(int siteId)
        {
            await NotifyNewConversations(siteId);
            await NotifyUpdateConversations(siteId);
            await NotifyNewMessages(siteId);
        }

        private async Task NotifyNewConversations(int siteId)
        {
            foreach (var newConversation in NewConversations)
            {
                await _notificationManager.NotifyNewConversation(siteId, newConversation.Id);
            }
        }

        private async Task NotifyUpdateConversations(int siteId)
        {
            foreach (var updatedConversation in UpdatedConversations)
            {
                await _notificationManager.NotifyUpdateConversation(siteId, updatedConversation.Id);
            }
        }

        private async Task NotifyNewMessages(int siteId)
        {
            foreach (var newMessage in NewMessages.OrderBy(t => t.Id))
            {
                if (newMessage.Source == MessageSource.FacebookMessage)
                {
                    await _notificationManager.NotifyNewFacebookMessage(siteId, newMessage.Id);
                }
                if (newMessage.Source == MessageSource.FacebookPostComment || newMessage.Source == MessageSource.FacebookPostReplyComment)
                {
                    await _notificationManager.NotifyNewFacebookComment(siteId, newMessage.Id);
                }
            }
        }
    }
}
