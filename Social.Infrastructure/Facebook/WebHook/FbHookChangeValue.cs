using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Social.Infrastructure.Facebook
{
    public class FbHookChangeValue
    {
        public string Item { get; set; }
        [JsonProperty("comment_id")]
        public string CommentId { get; set; }
        [JsonProperty("parent_id")]
        public string ParentId { get; set; }
        [JsonProperty("sender_name")]
        public string SenderName { get; set; }
        [JsonProperty("sender_id")]
        public string SenderId { get; set; }
        [JsonProperty("post_id")]
        public string PostId { get; set; }
        [JsonProperty("verb")]
        public string Verb { get; set; }
        [JsonProperty("created_time")]
        public long CreateTime { get; set; }
        [JsonProperty("is_hidden")]
        public bool IsHidden { get; set; }
        public string Message { get; set; }
        [JsonProperty("thread_id")]
        public string ThreadId { get; set; }
        [JsonProperty("page_id")]
        public string PageId { get; set; }
        [JsonProperty("link")]
        public string Link { get; set; }
        [JsonProperty("published")]
        public bool IsPublished { get; set; }
    }
}
