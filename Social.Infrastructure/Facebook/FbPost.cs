using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Social.Infrastructure.Facebook
{
    public class FbPost
    {
        public string id { get; set; }
        public string message { get; set; }
        public string story { get; set; }
        public string type { get; set; }
        public string link { get; set; }

        public bool is_hidden { get; set; }
        public bool is_published { get; set; }

        public string status_type { get; set; }

        public DateTime created_time { get; set; }
        public DateTime updated_time { get; set; }
        public DateTime tagged_time { get; set; }

        public FbUser from { get; set; }
        public FbData<FbUser> to { get; set; }
        public string permalink_url { get; set; }
        public FbPagingData<FbComment> comments { get; set; }
        public FbData<FbAttachment> attachments { get; set; }
    }
}
