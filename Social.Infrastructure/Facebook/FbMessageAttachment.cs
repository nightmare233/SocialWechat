using Social.Infrastructure.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Social.Infrastructure.Facebook
{
    public class FbMessageAttachment
    {
        public string Id { get; set; }
        public string MimeType { get; set; }
        public string Name { get; set; }
        public long Size { get; set; }
        public string Url { get; set; }
        public string PreviewUrl { get; set; }
        public MessageAttachmentType Type { get; set; }
    }
}
