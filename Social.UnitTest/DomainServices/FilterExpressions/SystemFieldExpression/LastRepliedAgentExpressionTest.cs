using Social.Domain.DomainServices;
using Social.Domain.Entities;
using Social.Infrastructure.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Social.UnitTest.DomainService.FilterExpressions.SystemFieldExpression
{
    public class LastRepliedAgentExpressionTest
    {
        [Fact]
        public void ShouldFilterByIsCondition()
        {
            FilterCondition condition = new FilterCondition
            {
                Field = new ConversationField { Name = "Last Replied Agent", DataType = FieldDataType.Option },
                MatchType = ConditionMatchType.Is,
                Value = "1"
            };

            var expression = new LastRepliedAgentExpression().Build(condition);

            var conditions = new List<Conversation>
            {
                new Conversation{Id=1,LastRepliedAgentId = 1 },
                new Conversation{Id=2,LastRepliedAgentId = 2 }
            }.AsQueryable();

            var result = conditions.Where(expression).ToList();

            Assert.Equal(1, result.Count);
            Assert.Equal(1, result.First().Id);
        }

        [Fact]
        public void ShouldFilterByIsNotCondition()
        {
            FilterCondition condition = new FilterCondition
            {
                Field = new ConversationField { Name = "Last Replied Agent", DataType = FieldDataType.Option },
                MatchType = ConditionMatchType.IsNot,
                Value = "1"
            };

            var expression = new LastRepliedAgentExpression().Build(condition);

            var conditions = new List<Conversation>
            {
                new Conversation{Id=1,LastRepliedAgentId = 1 },
                new Conversation{Id=2,LastRepliedAgentId = 2 }
            }.AsQueryable();

            var result = conditions.Where(expression).ToList();

            Assert.Equal(1, result.Count);
            Assert.Equal(2, result.First().Id);
        }
    }
}
