using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Social.Infrastructure.Facebook
{
    public class FbConversation
    {
        public FbConversation()
        {
            Messages = new FbPagingData<FbMessage>();
        }

        public string Id { get; set; }
        public DateTime UpdateTime { get; set; }
        public FbPagingData<FbMessage> Messages { get; set; }
    }
}
