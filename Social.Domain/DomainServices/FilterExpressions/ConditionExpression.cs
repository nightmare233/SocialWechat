using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Social.Domain.Entities;
using Framework.Core;

namespace Social.Domain.DomainServices
{
    public abstract class ConditionExpression : IConditionExpression
    {
        private ExpressionBuildOptions _options { get; set; }

        public IConditionExpression SetOptions(ExpressionBuildOptions options)
        {
            _options = options;
            return this;
        }

        public abstract Expression<Func<Conversation, bool>> Build(FilterCondition condition);

        public abstract bool IsMatch(FilterCondition condition);

        protected int GetUserId()
        {
            return _options.GetUserId();
        }

        protected int[] GetMyDepartmentMembers()
        {
            return _options.GetMyDepartmentMembers();
        }

        protected int[] GetMyDepartments()
        {
            return _options.GetMyDepartments();
        }
    }
}
