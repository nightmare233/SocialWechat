using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Social.Application.Dto
{
    public class MessageAttachmentRawDto
    {
        public int Id { get; set; }
        public string Url { get; set; }
        public byte[] RawData { get; set; }
    }
}
