using Social.Domain.Entities;
using Social.Infrastructure.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Social.Domain.DomainServices
{
    public class PriorityExpression : OptionExpression
    {
        public PriorityExpression() : base("Priority", "Priority")
        {
        }

        protected override object GetValue(string rawValue)
        {
            ConversationPriority value;
            Enum.TryParse(rawValue, out value);
            return value;
        }
    }
}
