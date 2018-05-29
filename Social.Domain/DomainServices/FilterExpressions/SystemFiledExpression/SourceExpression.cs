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
    public class SourceExpression : OptionExpression
    {
        public SourceExpression() : base("Source", "Source")
        {
        }

        protected override object GetValue(string rawValue)
        {
            ConversationSource value;
            Enum.TryParse(rawValue, out value);
            return value;
        }
    }
}
