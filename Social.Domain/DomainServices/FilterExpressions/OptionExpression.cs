using Social.Domain.Entities;
using Social.Infrastructure.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Social.Domain.DomainServices
{
    public abstract class OptionExpression : ConditionExpression
    {
        private string _propertyName;
        private string _FieldName;

        public OptionExpression(string FieldName, string propertyName)
        {
            _FieldName = FieldName;
            _propertyName = propertyName;
        }

        public override bool IsMatch(FilterCondition condition)
        {
            if (condition == null || condition.Field == null)
            {
                return false;
            }

            return condition.Field.DataType == FieldDataType.Option && condition.Field.Name == _FieldName;
        }

        public override Expression<Func<Conversation, bool>> Build(FilterCondition condition)
        {
            ParameterExpression Parameter = Expression.Parameter(typeof(Conversation), "c");
            if (GetValue(condition.Value) != null)
            {
                Expression left = Expression.Property(Parameter, typeof(Conversation).GetProperty(_propertyName));
                Expression right = Expression.Constant(GetValue(condition.Value));

                if (condition.MatchType == ConditionMatchType.Is)
                {
                    return Expression.Lambda<Func<Conversation, bool>>(Expression.Equal(left, right), new ParameterExpression[] { Parameter });
                }
                if (condition.MatchType == ConditionMatchType.IsNot)
                {
                    return Expression.Lambda<Func<Conversation, bool>>(Expression.NotEqual(left, right), new ParameterExpression[] { Parameter });
                }
            }
            else
            {
                if (condition.MatchType == ConditionMatchType.Is)
                {
                    return Is(condition);
                }
                if (condition.MatchType == ConditionMatchType.IsNot)
                {
                    return IsNot(condition);
                }

            }

            return null;
        }
        protected virtual Expression<Func<Conversation, bool>> Is(FilterCondition condition)
        {
            return null;
        }
        protected virtual Expression<Func<Conversation, bool>> IsNot(FilterCondition condition)
        {
            return null;
        }
        protected abstract object GetValue(string value);
    }
}
