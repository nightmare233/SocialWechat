using Social.Infrastructure.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Social.Application.Dto
{
    public class UpdateFacebookPageDto
    {
        [Required]
        public bool? IfEnable { get; set; }
        [Required]
        public bool? IfConvertMessageToConversation { get; set; }
        [Required]
        public bool? IfConvertVisitorPostToConversation { get; set; }
        [Required]
        public bool? IfConvertWallPostToConversation { get; set; }

        public int? ConversationDepartmentId { get; set; }
        public int? ConversationAgentId { get; set; }
        public ConversationPriority? ConversationPriority { get; set; }
    }
}
