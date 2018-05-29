using Social.Domain.DomainServices.FilterExpressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Social.Domain.Entities;
using System.Linq.Expressions;
using System.Text.RegularExpressions;

namespace Social.Domain.DomainServices
{
    public class LastMessageSentbyExpression: StringExpression
    {
        public LastMessageSentbyExpression():base("Last Message Sent by")
        {
        }

        protected override Expression<Func<Conversation, bool>> Contain(FilterCondition condition)
        {
            return t => t.LastMessageSender.Name.Contains(condition.Value);
        }

        protected override Expression<Func<Conversation, bool>> Is(FilterCondition condition)
        {
            return t => t.LastMessageSender.Name == condition.Value;
        }

        protected override Expression<Func<Conversation, bool>> IsNot(FilterCondition condition)
        {
            return t => t.LastMessageSender.Name != condition.Value;
        }

        protected override Expression<Func<Conversation, bool>> NotContain(FilterCondition condition)
        {
            return t => !t.LastMessageSender.Name.Contains(condition.Value);
        }
    }
}
