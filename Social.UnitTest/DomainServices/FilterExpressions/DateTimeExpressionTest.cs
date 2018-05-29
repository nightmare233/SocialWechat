using Social.Domain.DomainServices.FilterExpressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Social.Domain.Entities;
using System.Linq.Expressions;
using Xunit;
using Social.Infrastructure.Enum;

namespace Social.UnitTest.DomainServices.FilterExpressions
{
    public class DateTimeExpressionTest : DateTimeExpression
    {
        public DateTimeExpressionTest() : base("test")
        {
        }

        protected override Expression<Func<Conversation, bool>> After(string Date)
        {
            throw new NotImplementedException();
        }

        protected override Expression<Func<Conversation, bool>> Before(string Date)
        {
            throw new NotImplementedException();
        }

        protected override Expression<Func<Conversation, bool>> Between(string Date)
        {
            throw new NotImplementedException();
        }

        protected override Expression<Func<Conversation, bool>> Is(string Date)
        {
            DateTime value = DateTime.Parse(Date);
            return t => t.CreatedTime == value;
        }

        [Fact]
        public void ShouldDateByTodayCondition()
        {
            FilterCondition condition = new FilterCondition
            {
                Field = new ConversationField { Name = "test", DataType = FieldDataType.DateTime },
                MatchType = ConditionMatchType.Is,
                Value = "@Today"
            };
            var expression = Build(condition);

            var conditions = new List<Conversation>
            {
                new Conversation{Id=1,CreatedTime = DateTime.Parse(DateTime.UtcNow.ToString("yyyy-MM-dd"))},
            }.AsQueryable();

            var result = conditions.Where(expression).ToList();

            Assert.Equal(1, result.Count);
            Assert.Equal(1, result.First().Id);
        }

        [Fact]
        public void ShouldDateByYesterdayCondition()
        {
            FilterCondition condition = new FilterCondition
            {
                Field = new ConversationField { Name = "test", DataType = FieldDataType.DateTime },
                MatchType = ConditionMatchType.Is,
                Value = "@Yesterday"
            };
            var expression = Build(condition);

            var conditions = new List<Conversation>
            {
                new Conversation{Id=1,CreatedTime = DateTime.Parse(DateTime.UtcNow.AddDays(-1).ToString("yyyy-MM-dd"))},
            }.AsQueryable();

            var result = conditions.Where(expression).ToList();

            Assert.Equal(1, result.Count);
            Assert.Equal(1, result.First().Id);
        }

        [Fact]
        public void ShouldDateBy7_Days_AgoCondition()
        {
            FilterCondition condition = new FilterCondition
            {
                Field = new ConversationField { Name = "test", DataType = FieldDataType.DateTime },
                MatchType = ConditionMatchType.Is,
                Value = "@7 Days Ago"
            };
            var expression = Build(condition);

            var conditions = new List<Conversation>
            {
                new Conversation{Id=1,CreatedTime = DateTime.Parse(DateTime.UtcNow.AddDays(-7).ToString("yyyy-MM-dd"))},
            }.AsQueryable();

            var result = conditions.Where(expression).ToList();

            Assert.Equal(1, result.Count);
            Assert.Equal(1, result.First().Id);
        }

        [Fact]
        public void ShouldDateBy30_Days_AgoCondition()
        {
            FilterCondition condition = new FilterCondition
            {
                Field = new ConversationField { Name = "test", DataType = FieldDataType.DateTime },
                MatchType = ConditionMatchType.Is,
                Value = "@30 Days Ago"
            };
            var expression = Build(condition);

            var conditions = new List<Conversation>
            {
                new Conversation{Id=1,CreatedTime = DateTime.Parse(DateTime.UtcNow.AddDays(-30).ToString("yyyy-MM-dd"))},
            }.AsQueryable();

            var result = conditions.Where(expression).ToList();

            Assert.Equal(1, result.Count);
            Assert.Equal(1, result.First().Id);
        }
    }
}
