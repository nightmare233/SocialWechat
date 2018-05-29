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
    [Table("t_Social_ConversationLog")]
    public class ConversationLog : Entity, IHaveCreation, IShardingBySiteId
    {
        public int ConversationId { get; set; }
        public ConversationLogType Type { get; set; }
        [MaxLength(500)]
        public string Content { get; set; }
        public DateTime CreatedTime { get; set; }
        public int CreatedBy { get; set; }

        public virtual Conversation Conversation { get; set; }
    }
}
