using Framework.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Social.Domain.Entities
{
    [Table("t_LiveChat_Config")]
    public class Config : Entity
    {
        [Column("SiteId")]
        public new int Id { get; set; }
        public bool IfCustomAwayEnable { get; set; }
        public bool IfDepartmentEnable { get; set; }
    }
}
