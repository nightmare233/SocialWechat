using Social.Domain.DomainServices.FilterExpressions;
using System;
using Social.Domain.Entities;
using System.Linq.Expressions;
using Social.Infrastructure;

namespace Social.Domain.DomainServices
{
    public class LastMessageSentExpression : DateTimeExpression
    {
        public LastMessageSentExpression() : base("Last Message Sent")
        {
        }

        protected override Expression<Func<Conversation, bool>> After(string date)
        {
            DateTime value;
            if (!DateTime.TryParse(date, out value))
            {
                throw SocialExceptions.BadRequest("The value of date time is invalid");
            }
            return t => t.LastMessageSentTime > value;
        }

        protected override Expression<Func<Conversation, bool>> Before(string date)
        {
            DateTime value;
            if (!DateTime.TryParse(date, out value))
            {
                throw SocialExceptions.BadRequest("The value of date time is invalid");
            }
            return t => t.LastMessageSentTime < value;
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
            return t => t.LastMessageSentTime >= DateTime1 && t.LastMessageSentTime <= DateTime2;
        }

        protected override Expression<Func<Conversation, bool>> Is(string date)
        {
            DateTime value;
            if (!DateTime.TryParse(date, out value))
            {
                throw SocialExceptions.BadRequest("The value of date time is invalid");
            }
            return t => t.LastMessageSentTime.Year == value.Year
                       && t.LastMessageSentTime.Month == value.Month
                       && t.LastMessageSentTime.Day == value.Day;
        }
    }
}
