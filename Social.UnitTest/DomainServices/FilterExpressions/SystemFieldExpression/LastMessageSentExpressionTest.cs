using Social.Domain.DomainServices;
using Social.Domain.Entities;
using Social.Infrastructure;
using Social.Infrastructure.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Social.UnitTest.DomainService.FilterExpressions.SystemFieldExpression
{
    public class LastMessageSentExpressionTest
    {
        [Fact]
        public void ShouldFilterByIsCondition()
        {
            FilterCondition condition = new FilterCondition
            {
                Field = new ConversationField { Name = "Last Message Sent", DataType = FieldDataType.DateTime },
                MatchType = ConditionMatchType.Is,
                Value = "@Today"
            };

            var expression = new LastMessageSentExpression().Build(condition);

            var conditions = new List<Conversation>
            {
                new Conversation{Id=1,LastMessageSentTime=DateTime.UtcNow},
                new Conversation{Id=2,LastMessageSentTime=DateTime.UtcNow.AddDays(-2)}
            }.AsQueryable();

            var result = conditions.Where(expression).ToList();

            Assert.Equal(1, result.Count);
            Assert.Equal(1, result.First().Id);
        }

        [Fact]
        public void ShouldFilterByBeforeCondition()
        {
            FilterCondition condition = new FilterCondition
            {
                Field = new ConversationField { Name = "Last Message Sent", DataType = FieldDataType.DateTime },
                MatchType = ConditionMatchType.Before,
                Value = "@Today"
            };

            var expression = new LastMessageSentExpression().Build(condition);

            var conditions = new List<Conversation>
            {
                new Conversation{Id=1,LastMessageSentTime=DateTime.UtcNow},
                new Conversation{Id=2,LastMessageSentTime=DateTime.UtcNow.AddDays(-2)}
            }.AsQueryable();

            var result = conditions.Where(expression).ToList();

            Assert.Equal(1, result.Count);
            Assert.Equal(2, result.First().Id);
        }

        [Fact]
        public void ShouldFilterByAfterCondition()
        {
            FilterCondition condition = new FilterCondition
            {
                Field = new ConversationField { Name = "Last Message Sent", DataType = FieldDataType.DateTime },
                MatchType = ConditionMatchType.After,
                Value = "@Yesterday"
            };

            var expression = new LastMessageSentExpression().Build(condition);

            var conditions = new List<Conversation>
            {
                new Conversation{Id=1,LastMessageSentTime=DateTime.UtcNow},
                new Conversation{Id=2,LastMessageSentTime=DateTime.UtcNow.AddDays(-2)}
            }.AsQueryable();

            var result = conditions.Where(expression).ToList();

            Assert.Equal(1, result.Count);
            Assert.Equal(1, result.First().Id);
        }

        [Fact]
        public void ShouldFilterByBetweenCondition()
        {
            FilterCondition condition = new FilterCondition
            {
                Field = new ConversationField { Name = "Last Message Sent", DataType = FieldDataType.DateTime },
                MatchType = ConditionMatchType.Between,
                Value = DateTime.UtcNow.AddDays(-3).ToString("yyyy-MM-dd hh:mm:ss") + '|' + DateTime.UtcNow.AddDays(-1).ToString("yyyy-MM-dd hh:mm:ss")
            };

            var expression = new LastMessageSentExpression().Build(condition);

            var conditions = new List<Conversation>
            {
                new Conversation{Id=1,LastMessageSentTime=DateTime.UtcNow},
                new Conversation{Id=2,LastMessageSentTime=DateTime.UtcNow.AddDays(-2)}
            }.AsQueryable();

            var result = conditions.Where(expression).ToList();

            Assert.Equal(1, result.Count);
            Assert.Equal(2, result.First().Id);
        }

        [Fact]
        public void ShouldCheckValueType()
        {
            FilterCondition condition = new FilterCondition
            {
                Field = new ConversationField { Name = "Last Message Sent", DataType = FieldDataType.DateTime },
                MatchType = ConditionMatchType.Is,
                Value = "0"
            };
            try
            {
                var expression = new LastMessageSentExpression().Build(condition);
            }
            catch(Exception ex)
            {
                Assert.Equal("The value of date time is invalid", ex.Message);
            }
        }
    }
}
