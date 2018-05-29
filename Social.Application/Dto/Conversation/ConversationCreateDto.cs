using Social.Infrastructure.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Social.Application.Dto
{
    public class ConversationCreateDto
    {
        public ConversationSource Source { get; set; }

        public string OriginalId { get; set; }

        public bool IfRead { get; set; }

        public DateTime LastMessageSentTime { get; set; }

        public int LastMessageSenderId { get; set; }

        public int? LastRepliedAgentId { get; set; }

        public int? AgentId { get; set; }

        public int? DepartmentId { get; set; }

        public ConversationStatus Status { get; set; }

        [Required]
        [MaxLength(200)]
        public string Subject { get; set; }

        public ConversationPriority Priority { get; set; }

        [MaxLength(2000)]
        public string Note { get; set; }
    }
}
