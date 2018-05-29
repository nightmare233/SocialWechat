using Social.Domain.Entities;
using Social.Infrastructure.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Social.Application.Dto
{
    public class ConversationDto
    {
        public int Id { get; set; }

        public string Num
        {
            get { return "S" + Id; }
        }

        public ConversationSource Source { get; set; }

        public string OriginalId { get; set; }

        public string OriginalLink { get; set; }

        public bool IfRead { get; set; }

        public DateTime LastMessageSentTime { get; set; }

        public int LastMessageSenderId { get; set; }

        public string LastMessageSenderName { get; set; }
        public string LastMessageSenderAvatar { get; set; }
        public string LastMessageSenderUrl { get; set; }

        public string LastMessage { get; set; }

        public int? LastRepliedAgentId { get; set; }

        public int? AgentId { get; set; }
        public string AgentName { get; set; }

        public int? DepartmentId { get; set; }
        public string DepartmentName { get; set; }

        public ConversationStatus Status { get; set; }

        public int LastIntegrationAccountId { get; set; }
        public string LastIntegrationAccountName { get; set; }
        public string LastIntegrationAccountAvatar { get; set; }

        public int CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string CustomerAvatar { get; set; }

        [Required]
        [MaxLength(200)]
        public string Subject { get; set; }

        public ConversationPriority Priority { get; set; }

        [MaxLength(2000)]
        public string Note { get; set; }
    }
}
