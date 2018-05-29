using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Social.Infrastructure.LogicalExpression
{
    public class BuildResult<T>
    {
        public BuildResult(Expression<Func<T, bool>> expression)
        {
            Expression = expression;
        }

        public BuildResult(string errorMessage)
        {
            ErrorMessage = errorMessage;
        }

        public string ErrorMessage { get; set; }

        public bool IsSuccess { get { return string.IsNullOrWhiteSpace(ErrorMessage); } }

        public Expression<Func<T, bool>> Expression { get; set; }
    }
}
