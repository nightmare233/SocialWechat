using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using LinqKit;

namespace Social.Infrastructure.LogicalExpression
{
    public class LogicalExpressionVisitor<T> : SimpleLogicalExpressionBaseVisitor<Expression<Func<T, bool>>>
    {
        private IDictionary<int, Expression<Func<T, bool>>> _expressions;

        public LogicalExpressionVisitor(IDictionary<int, Expression<Func<T, bool>>> expressions)
        {
            _expressions = expressions;
        }


        public override Expression<Func<T, bool>> VisitInt([NotNull] SimpleLogicalExpressionParser.IntContext context)
        {
            int index;
            if (int.TryParse(context.INT().GetText(), out index))
            {
                if (_expressions.ContainsKey(index))
                {
                    return _expressions[index];
                }
                else
                {
                    throw new LogicalExpressionParseException($"Expression with index {index} has not been found.");
                }
            }
            else
            {
                throw new LogicalExpressionParseException($"{context.INT().GetText()} is not a integer.");
            }
        }

        public override Expression<Func<T, bool>> VisitAND([NotNull] SimpleLogicalExpressionParser.ANDContext context)
        {
            var left = Visit(context.expr(0));
            var right = Visit(context.expr(1));

            if (left == null || right == null)
            {
                return t => false;
            }

            return left.And(right);
        }

        public override Expression<Func<T, bool>> VisitOR([NotNull] SimpleLogicalExpressionParser.ORContext context)
        {
            var left = Visit(context.expr(0));
            var right = Visit(context.expr(1));

            if (left == null || right == null)
            {
                return t => false;
            }

            return left.Or(right);
        }

        public override Expression<Func<T, bool>> VisitParens([NotNull] SimpleLogicalExpressionParser.ParensContext context)
        {
            return base.Visit(context.expr());
        }

        public override Expression<Func<T, bool>> VisitErrorNode(IErrorNode node)
        {
            throw new LogicalExpressionParseException($"Invalid Expression.");
        }
    }
}
