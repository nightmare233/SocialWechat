using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Social.Infrastructure.Facebook
{
    public class FbHookMessage
    {
        public FbHookProfile Sender { get; set; }
        public FbHookProfile Recipient { get; set; }
        public long Timestamp { get; set; }
        public FbHookMessageContent Message { get; set; }
        public FbHookMessageDelivery Delivery { get; set; }
    }
}
