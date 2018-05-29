using Social.Domain.DomainServices.FilterExpressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Social.Domain.Entities;
using System.Linq.Expressions;

namespace Social.Domain.DomainServices
{
    public class ConversationIDExpression : NumberExpression
    {
        public ConversationIDExpression() : base("Conversation ID")
        {
        }

        protected override Expression<Func<Conversation, bool>> Is(FilterCondition condition)
        {
            int value = int.Parse(condition.Value);
            return t =>t.Id == value;
        }

        protected override Expression<Func<Conversation, bool>> IsLessThan(FilterCondition condition)
        {
            int value = int.Parse(condition.Value);
            return t => t.Id < value;
        }

        protected override Expression<Func<Conversation, bool>> IsMoreThan(FilterCondition condition)
        {
            int value = int.Parse(condition.Value);
            return t => t.Id > value;
        }

        protected override Expression<Func<Conversation, bool>> IsNot(FilterCondition condition)
        {
            int value = int.Parse(condition.Value);
            return t => t.Id != value;
        }
    }
}
