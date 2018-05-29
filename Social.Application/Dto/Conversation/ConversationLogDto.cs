using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Social.Application.Dto
{
    public class ConversationLogDto
    {
        public int Id { get; set; }
        public int ConversationId { get; set; }
        public string Content { get; set; }
        public DateTime CreatedTime { get; set; }
    }
}
