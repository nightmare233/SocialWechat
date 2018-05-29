using System.Threading.Tasks;
using Social.Infrastructure.Facebook;
using Social.Domain.Entities;
using Social.Infrastructure.Enum;
using System.Collections.Generic;
using System.Linq;
using Framework.Core;

namespace Social.Domain.DomainServices.Facebook
{
    /// <summary>
    /// Process facebook hook data when a new facebook message created.
    /// </summary>
    public class NewMessageStrategy : WebHookStrategy
    {
        public NewMessageStrategy(IDependencyResolver resolver) : base(resolver)
        {
        }

        public override bool IsMatch(FbHookChange change)
        {
            return change.Field == "conversations" && change.Value.ThreadId != null;
        }

        public async override Task<FacebookProcessResult> Process(SocialAccount socialAccount, FbHookChange change)
        {
            var result = new FacebookProcessResult(NotificationManager);
            if (!socialAccount.IfConvertMessageToConversation)
            {
                return result;
            }

            IList<FbMessage> fbMessages = await FbClient.GetMessagesFromConversationId(socialAccount.Token, change.Value.ThreadId, 10);
            string[] orgiginalIds = fbMessages.Select(t => t.Id).ToArray();
            List<Message> existedMessages = MessageService.FindAll().Where(t => t.Source == MessageSource.FacebookMessage && orgiginalIds.Contains(t.OriginalId)).ToList();
            // remove duplicated messages.
            fbMessages = fbMessages.Where(t => !existedMessages.Any(m => m.OriginalId == t.Id)).OrderByDescending(t => t.SendTime).ToList();

            foreach (var fbMessage in fbMessages)
            {
                await Process(result, fbMessage, socialAccount, change);
            }

            return result;
        }

        private async Task Process(FacebookProcessResult result, FbMessage fbMessage, SocialAccount socialAccount, FbHookChange change)
        {
            SocialUser sender = await GetOrCreateFacebookUser(socialAccount.Token, fbMessage.SenderId);
            SocialUser receiver = await GetOrCreateFacebookUser(socialAccount.Token, fbMessage.ReceiverId);

            var existingConversation = GetUnClosedConversation(change.Value.ThreadId);
            if (existingConversation != null)
            {
                Message message = FacebookConverter.ConvertMessage(fbMessage, sender, receiver);
                message.ConversationId = existingConversation.Id;
                bool isNewMessage = fbMessage.SendTime > existingConversation.LastMessageSentTime;

                if (isNewMessage)
                {
                    existingConversation.IfRead = false;
                    existingConversation.Status = sender.Id != socialAccount.SocialUser.Id ? ConversationStatus.PendingInternal : ConversationStatus.PendingExternal;
                    existingConversation.LastMessageSenderId = message.SenderId;
                    existingConversation.LastMessageSentTime = message.SendTime;
                }

                existingConversation.Messages.Add(message);
                await UpdateConversation(existingConversation);
                await CurrentUnitOfWork.SaveChangesAsync();

                result.WithNewMessage(message);
            }
            else
            {
                if (sender.Id == socialAccount.Id)
                {
                    return;
                }

                Message message = FacebookConverter.ConvertMessage(fbMessage, sender, receiver);
                var conversation = new Conversation
                {
                    OriginalId = change.Value.ThreadId,
                    Source = ConversationSource.FacebookMessage,
                    Priority = ConversationPriority.Normal,
                    Status = ConversationStatus.New,
                    Subject = GetSubject(message.Content),
                    LastMessageSenderId = message.SenderId,
                    LastMessageSentTime = message.SendTime
                };
                conversation.Messages.Add(message);
                await AddConversation(socialAccount, conversation);
                await CurrentUnitOfWork.SaveChangesAsync();

                result.WithNewConversation(conversation);
            }
        }
    }
}
