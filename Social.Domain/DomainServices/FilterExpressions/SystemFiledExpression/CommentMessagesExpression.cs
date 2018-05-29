using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Social.Domain.Entities;
using System.Text.RegularExpressions;

namespace Social.Domain.DomainServices.FilterExpressions.SystemFiledExpression
{
    public class CommentMessagesExpression : StringExpression
    {
        public CommentMessagesExpression() : base("Comment/Messages")
        {
        }

        protected override Expression<Func<Conversation, bool>> Contain(FilterCondition condition)
        {
            return t => t.Messages.Any(m => m.Content != null && m.Content.Contains(condition.Value));
        }

        protected override Expression<Func<Conversation, bool>> NotContain(FilterCondition condition)
        {
            return t => t.Messages.Any(m => m.Content != null && !m.Content.Contains(condition.Value) || m.Content == null);
        }

        protected override Expression<Func<Conversation, bool>> Is(FilterCondition condition)
        {
            return t => t.Messages.Any(m => m.Content != null && m.Content == condition.Value);
        }

        protected override Expression<Func<Conversation, bool>> IsNot(FilterCondition condition)
        {
            return t => t.Messages.Any(m => m.Content != null && m.Content != condition.Value || m.Content == null);
        }
    }
}
