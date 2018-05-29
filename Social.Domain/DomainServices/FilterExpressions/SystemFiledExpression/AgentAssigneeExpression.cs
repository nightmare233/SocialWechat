using Framework.Core;
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
    public class AgentAssigneeExpression : OptionExpression
    {
        public AgentAssigneeExpression() : base("Agent Assignee", "")
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
                    Expression<Func<Conversation, bool>> b = t => t.AgentId.HasValue && t.AgentId.Value == member;
                    predicate = predicate.Or(b);
                }
                return predicate;
            }
            else
            {
                value = MatchValue(condition);
            }
            if(value == null)
            {
                return t => !t.AgentId.HasValue;
            }
            else
            {
                return t => t.AgentId.HasValue && t.AgentId.Value == value;
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
                    Expression<Func<Conversation, bool>> b = t => !t.AgentId.HasValue || t.AgentId.Value != member;
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
                return t => t.AgentId.HasValue;
            }
            else
            {
                return t => !t.AgentId.HasValue || t.AgentId.Value != value;
            }
        }

        protected override object GetValue(string rawValue)
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
