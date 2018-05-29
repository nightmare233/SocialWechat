using Framework.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Social.Domain.Entities
{
    [Table("t_LiveChat_CustomAwayStatus")]
    public class CustomAwayStatus: EntityWithSite
    {
        public string Name { get; set; }
        public bool IfSystem { get; set; }
        public bool IfVisible { get; set; }
        public bool IfDeleted { get; set; }
        public int OrderId { get; set; }
    }
}
