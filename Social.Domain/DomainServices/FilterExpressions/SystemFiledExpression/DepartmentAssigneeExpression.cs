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
    public class DepartmentAssigneeExpression : OptionExpression
    {
        public DepartmentAssigneeExpression() : base("Department Assignee", "")
        {
        }

        protected override Expression<Func<Conversation, bool>> Is(FilterCondition condition)
        {
            List<int> values = MatchValue(condition);
            if(values == null)
            {
                return t => !t.DepartmentId.HasValue;
            }
            else
            {
                var predicate = PredicateBuilder.New<Conversation>();
                foreach(int value in values)
                {
                    predicate.Or(t => t.DepartmentId.HasValue && t.DepartmentId.Value == value);
                }
                return predicate;
            }
        }

        protected override Expression<Func<Conversation, bool>> IsNot(FilterCondition condition)
        {
            List<int> values = MatchValue(condition);
            if (values == null)
            {
                return t => t.DepartmentId.HasValue;
            }
            else
            {
                var predicate = PredicateBuilder.New<Conversation>();
                foreach (int value in values)
                {
                    predicate.And(t => t.DepartmentId.HasValue && t.DepartmentId.Value != value);
                }
                predicate.Or(t => !t.DepartmentId.HasValue);
                return predicate;
            }
        }

        protected override object GetValue(string rawValue)
        {
            return null;
        }

        private List<int> MatchValue(FilterCondition condition)
        {
            List<int> values = new List<int>();
            if (condition.Value == "@My Department")
            {
                values = GetMyDepartments().ToList();
            }
            else if (condition.Value == "Blank")
            {
                return null;
            }
            else
            {
                values.Add(int.Parse(condition.Value));
            }
            return values;
        }
    }
}
