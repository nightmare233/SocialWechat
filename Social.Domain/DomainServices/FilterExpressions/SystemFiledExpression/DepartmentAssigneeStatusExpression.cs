using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Social.Domain.Entities;
using Social.Domain.Entities.General;
using LinqKit;

namespace Social.Domain.DomainServices
{
    public class DepartmentAssigneeStatusExpression : OptionExpression
    {
        private IDepartmentService _departmentService;
        public DepartmentAssigneeStatusExpression(IDepartmentService departmentService) : base("Department Assignee Status", "")
        {
            _departmentService = departmentService;
        }

        protected override object GetValue(string value)
        {
            return null;
        }

        protected override Expression<Func<Conversation, bool>> Is(FilterCondition condition)
        {
            int[] departmentIds = _departmentService.GetMatchedStatusDepartments(int.Parse(condition.Value));
            return t => t.DepartmentId.HasValue && departmentIds.Contains(t.DepartmentId.Value);
        }

        protected override Expression<Func<Conversation, bool>> IsNot(FilterCondition condition)
        {
            int[] departmentIds = _departmentService.GetMatchedStatusDepartments(int.Parse(condition.Value));
            return t => t.DepartmentId.HasValue && !departmentIds.Contains(t.DepartmentId.Value) || !t.DepartmentId.HasValue;

        }
    }
}
