using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Social.Infrastructure.Facebook
{
    public class FbMessage
    {
        public FbMessage()
        {
            Attachments = new List<FbMessageAttachment>();
        }

        public string Id { get; set; }
        public DateTime SendTime { get; set; }
        public string SenderId { get; set; }
        public string SenderEmail { get; set; }
        public string ReceiverId { get; set; }
        public string ReceiverEmail { get; set; }
        public string Content { get; set; }
        public string Link { get; set; }
        public string Story { get; set; }

        // Comment Id or Post Id
        public string ParentId { get; set; }
        public IList<FbMessageAttachment> Attachments { get; set; }
    }
}
