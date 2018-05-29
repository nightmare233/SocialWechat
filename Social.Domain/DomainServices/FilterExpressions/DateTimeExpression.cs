using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Social.Domain.Entities;
using Social.Infrastructure.Enum;

namespace Social.Domain.DomainServices.FilterExpressions
{
    public abstract class DateTimeExpression : ConditionExpression
    {
        private string _propertyName;

        public DateTimeExpression(string propertyName)
        {
            _propertyName = propertyName;
        }
        public override Expression<Func<Conversation, bool>> Build(FilterCondition condition)
        {
            string date = condition.Value;
            if (condition.Value == "@Today")
            {
                date = DateTime.UtcNow.ToString("yyyy-MM-dd");
            }
            else if (condition.Value == "@Yesterday")
            {
                date = DateTime.UtcNow.AddDays(-1).ToString("yyyy-MM-dd");
            }
            else if (condition.Value == "@7 Days Ago")
            {
                date = DateTime.UtcNow.AddDays(-7).ToString("yyyy-MM-dd");
            }
            else if (condition.Value == "@30 Days Ago")
            {
                date = DateTime.UtcNow.AddDays(-30).ToString("yyyy-MM-dd");
            }
            if (condition.MatchType == ConditionMatchType.Is)
            {
                return Is(date);
            }
            if (condition.MatchType == ConditionMatchType.Before)
            {
                return Before(date);
            }
            if (condition.MatchType == ConditionMatchType.After)
            {
                return After(date);
            }
            if (condition.MatchType == ConditionMatchType.Between)
            {
                return Between(date);
            }

            return null;
        }

        public override bool IsMatch(FilterCondition condition)
        {
            if (condition == null || condition.Field == null)
            {
                return false;
            }

            return condition.Field.DataType == FieldDataType.DateTime && condition.Field.Name == _propertyName;
        }

        protected abstract Expression<Func<Conversation, bool>> Is(string Date);
        protected abstract Expression<Func<Conversation, bool>> Before(string Date);
        protected abstract Expression<Func<Conversation, bool>> After(string Date);
        protected abstract Expression<Func<Conversation, bool>> Between(string Date);
    }
}
