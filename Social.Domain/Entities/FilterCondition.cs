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
    [Table("t_Social_FilterCondition")]
    public class FilterCondition : EntityWithSite
    {
        public int FilterId { get; set; }
        public int FieldId { get; set; }
        public ConditionMatchType MatchType { get; set; }
        [MaxLength(200)]
        public string Value { get; set; }
        public int Index { get; set; }

        public virtual Filter Filter { get; set; }
        public virtual ConversationField Field { get; set; }
    }
}
