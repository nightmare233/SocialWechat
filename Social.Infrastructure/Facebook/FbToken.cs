using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Social.Infrastructure.Facebook
{
    public class FbToken
    {
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }
        [JsonProperty("token_type")]
        public string Type { get; set; }
        [JsonProperty("expires_in")]
        public long? ExpiresIn { get; set; }
    }
}