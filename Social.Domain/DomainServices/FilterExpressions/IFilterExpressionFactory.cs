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
    public interface IFilterExpressionFactory : ITransient
    {
        Expression<Func<Conversation, bool>> Create(Filter filter);
        Expression<Func<Conversation, bool>> Create(Filter filter, ExpressionBuildOptions options);
    }
}
