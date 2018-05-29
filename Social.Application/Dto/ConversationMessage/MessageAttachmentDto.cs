using Social.Infrastructure.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Social.Application.Dto
{
    public class MessageAttachmentDto
    {
        public MessageAttachmentType Type { get; set; }
        public string Url { get; set; }
        public string PreviewUrl { get; set; }
        public string Name { get; set; }
        public string MimeType { get; set; }

        public string FileType { get; set; }
    }
}
