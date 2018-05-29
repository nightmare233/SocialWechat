using LinqKit;
using Social.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Social.Domain.DomainServices
{ 
    public class RepliedAgentsExpression : OptionExpression
    {
        public RepliedAgentsExpression():base("Replied Agent", "")
        {
        }

        protected override Expression<Func<Conversation, bool>> Is(FilterCondition condition)
        {
            int? value = default(int);
            if (condition.Value == "@My Department Member")
            {
                int[] members = GetMyDepartmentMembers();
                var predicate = PredicateBuilder.New<Conversation>();
                foreach (int member in members)
                {
                    Expression<Func<Conversation, bool>> b = t => t.Messages.Any(m => m.SendAgentId.HasValue && m.SendAgentId.Value == value);
                    predicate = predicate.Or(b);
                }
                return predicate;
            }
            else
            {
                value = MatchValue(condition);
            }
            if (value == null)
            {
                return t => t.Messages.All(m => !m.SendAgentId.HasValue);
            }
            else
            {
                return t => t.Messages.Any(m => m.SendAgentId.HasValue && m.SendAgentId.Value == value);
            }
        }

        protected override Expression<Func<Conversation, bool>> IsNot(FilterCondition condition)
        {
            int? value = default(int);
            if (condition.Value == "@My Department Member")
            {
                int[] members = GetMyDepartmentMembers();
                var predicate = PredicateBuilder.New<Conversation>();
                foreach (int member in members)
                {
                    Expression<Func<Conversation, bool>> b = t => t.Messages.All(m => !m.SendAgentId.HasValue ||m.SendAgentId.Value != member);
                    predicate = predicate.And(b);
                }
                return predicate;
            }
            else
            {
                value = MatchValue(condition);
            }

            if (value == null)
            {
                return t => t.Messages.Any(m => m.SendAgentId.HasValue);
            }
            else
            {
                return t => t.Messages.All(m => !m.SendAgentId.HasValue || m.SendAgentId.Value != value);
            }
        }

        protected override object GetValue(string value)
        {
            return null;
        }

        private int? MatchValue(FilterCondition condition)
        {
            int? value = default(int);
            if (condition.Value == "@Me")
            {
                value = GetUserId();
            }
            else if (condition.Value == "Blank")
            {
                value = null;
            }
            else
            {
                value = int.Parse(condition.Value);
            }
            return value;
        }
    }
}
