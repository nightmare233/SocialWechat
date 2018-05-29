using Framework.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Social.Domain.Entities
{
    /// <summary>
    /// This table is juset for table, should use rabbit mq later.
    /// </summary>
    [Table("t_Social_FacebookWebHookRawData")]
    public class FacebookWebHookRawData : Entity, ISoftDelete
    {
        public string Data { get; set; }
        public DateTime CreatedTime { get; set; }
        public bool IsDeleted { get; set; }
    }
}
