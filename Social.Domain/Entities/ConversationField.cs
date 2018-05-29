using Framework.Core;
using Social.Infrastructure.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Social.Domain.Entities
{
    [Table("t_Social_ConversationField")]
    public class ConversationField : EntityWithSite
    {
        public ConversationField()
        {
            Conditions = new List<FilterCondition>();
            Options = new List<ConversationFieldOption>();
        }

        public bool IfSystem { get; set; }
        public FieldDataType DataType { get; set; }
        public string Name { get; set; }

        public virtual IList<FilterCondition> Conditions { get; set; }

        public virtual IList<ConversationFieldOption> Options { get; set; }

    }
}
