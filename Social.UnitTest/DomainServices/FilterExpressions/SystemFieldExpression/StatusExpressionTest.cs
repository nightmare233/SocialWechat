using Social.Domain.DomainServices;
using Social.Domain.Entities;
using Social.Infrastructure.Enum;
using System.Collections.Generic;
using System.Linq;
using Xunit;


namespace Social.UnitTest.DomainService.FilterExpressions.SystemFieldExpression
{
    public class StatusExpressionTest
    {
        [Fact]
        public void ShouldFilterByIsCondition()
        {
            FilterCondition condition = new FilterCondition
            {
                Field = new ConversationField { Name = "Status", DataType = FieldDataType.Option },
                MatchType = ConditionMatchType.Is,
                Value = "0"
            };
            var expression = new StatusExpression().Build(condition);

            var conditions = new List<Conversation>
            {
                new Conversation{Id=1,Status = ConversationStatus.New},
                new Conversation{Id=2,Status = ConversationStatus.Closed}
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
                Field = new ConversationField { Name = "Status", DataType = FieldDataType.Option },
                MatchType = ConditionMatchType.IsNot,
                Value = "0"
            };
            var expression = new StatusExpression().Build(condition);

            var conditions = new List<Conversation>
            {
                new Conversation{Id=1,Status = ConversationStatus.New},
                new Conversation{Id=2,Status = ConversationStatus.Closed}
            }.AsQueryable();

            var result = conditions.Where(expression).ToList();

            Assert.Equal(1, result.Count);
            Assert.Equal(2, result.First().Id);
        }
    }
}
