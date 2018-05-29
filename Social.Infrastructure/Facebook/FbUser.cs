using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Social.Infrastructure.Facebook
{
    public class FbUser
    {
        public string id { get; set; }
        public string name { get; set; }
        public string email { get; set; }
        public string pic { get; set; }
        public string link { get; set; }
        public string profile_type { get; set; }
    }
}
