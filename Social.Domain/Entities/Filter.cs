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
    [Table("t_Social_Filter")]
    public class Filter : EntityWithSite, IHaveCreation
    {
        public Filter()
        {
            Conditions = new List<FilterCondition>();
        }

        [Required]
        [MaxLength(200)]
        public string Name { get; set; }
        public int Index { get; set; }
        public bool IfPublic { get; set; }
        public FilterType Type { get; set; }

        [MaxLength(200)]
        public string LogicalExpression { get; set; }

        public int CreatedBy { get; set; }
        public DateTime CreatedTime { get; set; }

        public virtual IList<FilterCondition> Conditions { get; set; }
    }
}
