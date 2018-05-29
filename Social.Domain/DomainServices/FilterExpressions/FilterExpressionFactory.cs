using Framework.Core;
using LinqKit;
using Social.Domain.Entities;
using Social.Infrastructure.Enum;
using Social.Infrastructure.LogicalExpression;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Social.Domain.DomainServices.FilterExpressions
{
    public class FilterExpressionFactory : IFilterExpressionFactory
    {
        private IList<IConditionExpression> _conditionExpressions;
        private IDependencyResolver _dependencyResolver;

        public FilterExpressionFactory(IDependencyResolver dependencyResover)
        {
            _conditionExpressions = dependencyResover.ResolveAll<IConditionExpression>();
            _dependencyResolver = dependencyResover;
        }

        public Expression<Func<Conversation, bool>> Create(Filter filter)
        {
            return Create(filter, new ExpressionBuildOptions(_dependencyResolver));
        }

        public Expression<Func<Conversation, bool>> Create(Filter filter, ExpressionBuildOptions options)
        {
            var expressions = filter.Conditions.Select(t => GetConditionExpression(t, options)).Where(t => t != null).ToList();
            if (expressions.Count() == 0)
            {
                return t => true;
            }
            if (filter.Type == FilterType.All)
            {
                var predicate = PredicateBuilder.New<Conversation>();
                foreach (var expression in expressions)
                {
                    predicate = predicate.And(expression);
                }

                return predicate;
            }

            if (filter.Type == FilterType.Any)
            {
                var predicate = PredicateBuilder.New<Conversation>();
                foreach (var expression in expressions)
                {
                    predicate = predicate.Or(expression);
                }

                return predicate;
            }

            if (filter.Type == FilterType.LogicalExpression)
            {
                var predicate = PredicateBuilder.New<Conversation>();
                var expressionDic = filter.Conditions.ToDictionary(t => t.Index, t => GetConditionExpression(t, options));

                var buildResult = LogicalExpressionBuilder.Build(expressionDic, filter.LogicalExpression);
                if (buildResult.IsSuccess)
                {
                    return predicate.And(buildResult.Expression);
                }
            }

            return t => true;
        }

        private Expression<Func<Conversation, bool>> GetConditionExpression(FilterCondition condition, ExpressionBuildOptions options)
        {
            foreach (var expression in _conditionExpressions)
            {
                if (expression.IsMatch(condition))
                {
                    return expression.SetOptions(options).Build(condition);
                }
            }

            return null;
        }
    }
}
