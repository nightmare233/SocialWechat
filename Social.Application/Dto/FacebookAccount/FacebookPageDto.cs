using Social.Infrastructure.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Social.Application.Dto
{
    public class FacebookPageDto
    {
        public int Id { get; set; }
        public string FacebookId { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public string Avatar { get; set; }
        public string SignInAs { get; set; }
        public bool IfEnable { get; set; }
        public bool IfConvertMessageToConversation { get; set; }
        public bool IfConvertVisitorPostToConversation { get; set; }
        public bool IfConvertWallPostToConversation { get; set; }
        public int? ConversationDepartmentId { get; set; }
        public int? ConversationAgentId { get; set; }
        public ConversationPriority? ConversationPriority { get; set; }
    }
}
