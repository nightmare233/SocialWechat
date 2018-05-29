using Social.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Social.Domain.DomainServices
{
    public class SocialAccountsExpression: OptionExpression
    {
        public SocialAccountsExpression():base("Social Page/Account", "")
        {
        }

        protected override Expression<Func<Conversation, bool>> Is(FilterCondition condition)
        {
            int value = int.Parse(condition.Value);
            return  t => t.Messages.Any(m => m.SenderId == value || (m.ReceiverId.HasValue && m.ReceiverId.Value == value));
        }

        protected override Expression<Func<Conversation, bool>> IsNot(FilterCondition condition)
        {
            int value = int.Parse(condition.Value);
            return t => t.Messages.Any(m => m.SenderId != value && (m.ReceiverId.HasValue &&m.ReceiverId.Value != value||!m.ReceiverId.HasValue));
        }
        protected override object GetValue(string rawValue)
        {
            return null;
        }
    }
}
