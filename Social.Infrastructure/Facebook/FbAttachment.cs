using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Social.Infrastructure.Facebook
{
    public class FbAttachment
    {
        public string description { get; set; }
        public string type { get; set; }
        public string url { get; set; }
        public FbAttachmentMedia media { get; set; }
    }
}
