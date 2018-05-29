using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Social.Application.Dto
{
    public class MessageAttachmentUrlDto
    {
        public int Id { get; set; }
        public byte[] RawData { get; set; }
        public string MimeType { get; set; }
    }
}
