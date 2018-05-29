using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Social.Infrastructure.LogicalExpression
{
    public static class LogicalExpressionBuilder
    {
        public static BuildResult<T> Build<T>(IDictionary<int, Expression<Func<T, bool>>> expressions, string logicalExpressions)
        {
            var input = new AntlrInputStream(logicalExpressions.ToUpper());
            var lexer = new SimpleLogicalExpressionLexer(input);
            var tokens = new CommonTokenStream(lexer);
            var parser = new SimpleLogicalExpressionParser(tokens);
            IParseTree tree = parser.prog();
            var visitor = new LogicalExpressionVisitor<T>(expressions);
            try
            {
                var expression = visitor.Visit(tree);
                return new BuildResult<T>(expression);
            }
            catch (LogicalExpressionParseException ex)
            {
                return new BuildResult<T>(ex.Message);
            }
        }
    }
}
