using Framework.Core;
using Social.Infrastructure.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Social.Domain.Entities
{
    [Table("t_Social_MessageAttachment")]
    public class MessageAttachment : Entity, IShardingBySiteId
    {
        public int MessageId { get; set; }
        [MaxLength(200)]
        public string OriginalId { get; set; }
        [MaxLength(500)]
        public string OriginalLink { get; set; }
        public MessageAttachmentType Type { get; set; }

        public string Name { get; set; }
        public string MimeType { get; set; }
        public long Size { get; set; }
        public string Url { get; set; }
        public string PreviewUrl { get; set; }
        public byte[] RawData { get; set; }

        public virtual Message Message { get; set; }
    }
}
