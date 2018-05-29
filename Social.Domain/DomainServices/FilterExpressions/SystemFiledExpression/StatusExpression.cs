using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Social.Domain.Entities;
using Social.Infrastructure.Enum;

namespace Social.Domain.DomainServices
{
    public class StatusExpression : OptionExpression
    {
        public StatusExpression() : base("Status", "Status")
        {
        }

        protected override object GetValue(string rawValue)
        {
            ConversationStatus value;
            Enum.TryParse(rawValue, out value);
            return value;
        }
    }
}
