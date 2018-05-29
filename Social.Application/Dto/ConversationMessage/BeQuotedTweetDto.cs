using Social.Infrastructure.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Social.Application.Dto
{
    public class BeQuotedTweetDto
    {
        public BeQuotedTweetDto()
        {
            Attachments = new List<MessageAttachmentDto>();
        }

        public MessageSource Source { get; set; }
        public string OriginalLink { get; set; }
        public string UserAvatar { get; set; }
        public string UserLink { get; set; }
        public string UserName { get; set; }
        public string UserScreenName { get; set; }
        public string Content { get; set; }
        public DateTime SendTime { get; set; }

        public List<MessageAttachmentDto> Attachments { get; set; }
    }
}
