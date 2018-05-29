using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Social.Infrastructure.Facebook
{
    public class FbPage
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string AccessToken { get; set; }
        public string Category { get; set; }
        public string Avatar { get; set; }
        public string Link { get; set; }
    }
}
