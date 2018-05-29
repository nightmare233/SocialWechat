using Social.Domain.DomainServices.FilterExpressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Social.Domain.Entities;
using System.Linq.Expressions;
using Social.Infrastructure;

namespace Social.Domain.DomainServices
{
    public class LastModifiedExpression : DateTimeExpression
    {
        public LastModifiedExpression() : base("Last Modified Date")
        {
        }

        protected override Expression<Func<Conversation, bool>> After(string date)
        {
            DateTime value;
            if (!DateTime.TryParse(date, out value))
            {
                throw SocialExceptions.BadRequest("The value of date time is invalid");
            }
            return t => t.ModifiedTime.HasValue && t.ModifiedTime.Value > value;
        }

        protected override Expression<Func<Conversation, bool>> Before(string date)
        {
            DateTime value;
            if (!DateTime.TryParse(date, out value))
            {
                throw SocialExceptions.BadRequest("The value of date time is invalid");
            }
            return t => t.ModifiedTime.HasValue && t.ModifiedTime.Value < value;
        }

        protected override Expression<Func<Conversation, bool>> Between(string date)
        {
            string[] value = date.Split('|');
            DateTime DateTime1;
            if (!DateTime.TryParse(value[0], out DateTime1))
            {
                throw SocialExceptions.BadRequest("The value of date time is invalid");
            }
            DateTime DateTime2;
            if (!DateTime.TryParse(value[1], out DateTime2))
            {
                throw SocialExceptions.BadRequest("The value of date time is invalid");
            }
            return t => t.ModifiedTime.HasValue && t.ModifiedTime.Value >= DateTime1 && t.ModifiedTime.Value <= DateTime2;
        }

        protected override Expression<Func<Conversation, bool>> Is(string date)
        {
            DateTime value;
            if (!DateTime.TryParse(date, out value))
            {
                throw SocialExceptions.BadRequest("The value of date time is invalid");
            }
            return t => t.ModifiedTime.HasValue && ((DateTime)t.ModifiedTime.Value).Year == value.Year
                       && ((DateTime)t.ModifiedTime.Value).Month == value.Month
                       && ((DateTime)t.ModifiedTime.Value).Day == value.Day;
        }
    }
}
