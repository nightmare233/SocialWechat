using Framework.Core;
using Social.Infrastructure.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Social.Domain.Entities
{
    [Table("t_Social_Message")]
    public class Message : Entity, ISoftDelete, IShardingBySiteId
    {
        public Message()
        {
            Attachments = new List<MessageAttachment>();
            Children = new List<Message>();
        }

        public int ConversationId { get; set; }

        public MessageSource Source { get; set; }

        [Required]
        [MaxLength(200)]
        public string OriginalId { get; set; }

        [MaxLength(500)]
        public string OriginalLink { get; set; }

        public int? ParentId { get; set; }

        public DateTime SendTime { get; set; }

        public int SenderId { get; set; }

        public int? ReceiverId { get; set; }

        public int? SendAgentId { get; set; }
        [MaxLength(10000)]
        public string Content { get; set; }

        [MaxLength(500)]
        public string Story { get; set; }

        public bool IsDeleted { get; set; }

        [MaxLength(200)]
        public string QuoteTweetId { get; set; }

        public virtual Conversation Conversation { get; set; }

        public virtual IList<MessageAttachment> Attachments { get; set; }

        public virtual Message Parent { get; set; }

        public virtual SocialUser Sender { get; set; }

        public virtual SocialUser Receiver { get; set; }

        public virtual User SendAgent { get; set; } 

        public virtual IList<Message> Children { get; set; }

        public int IntegrationAccountId
        {
            get
            {
                if (Receiver != null && Receiver.IsIntegrationAccount)
                {
                    return Receiver.Id;
                }

                if (Sender.IsIntegrationAccount)
                {
                    return Sender.Id;
                }

                return 0;
            }
        }

        public SocialUser IntegrationAccount
        {
            get
            {
                if (Receiver != null && Receiver.IsIntegrationAccount)
                {
                    return Receiver;
                }

                if (Sender.IsIntegrationAccount)
                {
                    return Sender;
                }

                return null;
            }
        }
    }
}
