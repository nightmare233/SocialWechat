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
    public class CreatedExpression : DateTimeExpression
    {
        public CreatedExpression() : base("Date Created")
        {
        }

        protected override Expression<Func<Conversation, bool>> After(string date)
        {
            DateTime value;
            if (!DateTime.TryParse(date, out value))
            {
                throw SocialExceptions.BadRequest("The value of date time is invalid");
            }
            return t => t.CreatedTime > value;
        }

        protected override Expression<Func<Conversation, bool>> Before(string date)
        {
            DateTime value;
            if (!DateTime.TryParse(date, out value))
            {
                throw SocialExceptions.BadRequest("The value of date time is invalid");
            }
            return t => t.CreatedTime < value;
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
            return t => t.CreatedTime >= DateTime1 && t.CreatedTime <= DateTime2;
        }

        protected override Expression<Func<Conversation, bool>> Is(string date)
        {
            DateTime value;
            if (!DateTime.TryParse(date, out value))
            {
                throw SocialExceptions.BadRequest("The value of date time is invalid");
            }
            return t => t.CreatedTime.Year == value.Year
                       && t.CreatedTime.Month == value.Month
                       && t.CreatedTime.Day == value.Day;
        }
    }
}
