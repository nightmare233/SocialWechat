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
    public class SocialUsersExpression : StringExpression
    {
        public SocialUsersExpression() : base("Social User")
        {
        }

        protected override Expression<Func<Conversation, bool>> Contain(FilterCondition condition)
        {
            return t => t.Messages.Any(m => m.Sender.Name.Contains(condition.Value)|| (m.Sender.Email != null && m.Sender.Email.Contains(condition.Value)));
        }

        protected override Expression<Func<Conversation, bool>> NotContain(FilterCondition condition)
        {
            return t => t.Messages.Any(m => !m.Sender.Name.Contains(condition.Value) && (m.Sender.Email != null && ! m.Sender.Email.Contains(condition.Value)|| m.Sender.Email == null));
        }

        protected override Expression<Func<Conversation, bool>> Is(FilterCondition condition)
        {
            return t => t.Messages.Any(m => m.Sender.Name == condition.Value || (m.Sender.Email != null && m.Sender.Email == condition.Value));
        }

        protected override Expression<Func<Conversation, bool>> IsNot(FilterCondition condition)
        {
            return t => t.Messages.Any(m => m.Sender.Name != condition.Value && (m.Sender.Email != null && m.Sender.Email != condition.Value || m.Sender.Email == null));
        }

    }
}
