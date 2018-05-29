using Framework.Core;
using Social.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Social.Domain.DomainServices
{
    public interface IConditionExpression : ITransient
    {
        bool IsMatch(FilterCondition condition);
        Expression<Func<Conversation, bool>> Build(FilterCondition condition);
        IConditionExpression SetOptions(ExpressionBuildOptions options);
    }
}
