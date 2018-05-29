using Framework.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Social.Domain.Entities
{
    [Table("t_AuditLog")]
    public class AuditLog: Entity,IShardingBySiteId
    {
        public int ApplicationType { get; set; }
        public DateTime ActionTime { get; set; }
        [Column("Operator")]
        public int OperatorId { get; set; }
        public int ActionType { get; set; }
        public string ActionDetail { get; set; }
        public string ActionDetailData { get; set; }
    }
}
