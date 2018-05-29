using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Social.Infrastructure.Facebook
{
    public class FbComment
    {
        public string id { get; set; }
        public DateTime created_time { get; set; }
        public FbComment parent { get; set; }
        public FbUser from { get; set; }
        public string message { get; set; }
        public string permalink_url { get; set; }
        public FbAttachment attachment { get; set; }
        public int comment_count { get; set; }
        public bool is_hidden { get; set; }
        public FbPagingData<FbComment> comments { get; set; }

        public string PostId { get; set; }
    }
}
