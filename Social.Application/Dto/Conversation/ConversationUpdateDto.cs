using Social.Infrastructure.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Framework.Core;

namespace Social.Application.Dto
{
    public class ConversationUpdateDto
    {
        //[Enum]
        //[Required]
        //public ConversationSource? Source { get; set; }

        public bool IfRead { get; set; }

        public int? AgentId { get; set; }

        public int? DepartmentId { get; set; }

        [Enum]
        [Required]
        public ConversationStatus? Status { get; set; }

        [Required]
        [MaxLength(200)]
        public string Subject { get; set; }

        [Enum]
        [Required]
        public ConversationPriority? Priority { get; set; }

        [MaxLength(2000)]
        public string Note { get; set; }
    }
}
