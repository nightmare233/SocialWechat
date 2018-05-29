using Framework.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Social.Domain.Entities
{
    [Table("t_Cpanel_ConfigOption")]
    public class CpanelConfigOption : Entity<string>
    {
        [Column("OptionKey")]

        public new string Id { get; set; }
        public string OptionValue { get; set; }
        public int Type { get; set; }
        public string Description { get; set; }
    }
}
