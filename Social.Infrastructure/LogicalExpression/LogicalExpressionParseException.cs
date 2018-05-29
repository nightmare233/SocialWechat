using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Social.Infrastructure.LogicalExpression
{
    public class LogicalExpressionParseException : Exception
    {
        public LogicalExpressionParseException() : base()
        {

        }

        public LogicalExpressionParseException(string message) : base(message)
        {

        }

        public LogicalExpressionParseException(string message, Exception innerException) : base(message, innerException)
        {

        }
    }
}
