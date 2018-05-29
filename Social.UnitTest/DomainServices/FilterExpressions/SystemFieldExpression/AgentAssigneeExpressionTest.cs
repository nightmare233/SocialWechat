using Framework.Core;
using Moq;
using Social.Domain.DomainServices;
using Social.Domain.Entities;
using Social.Infrastructure.Enum;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Social.UnitTest.DomainService.FilterExpressions.SystemFieldExpression
{
    public class AgentAssigneeExpressionTest
    {
        [Fact]
        public void ShouldFilterByIsCondition()
        {
            FilterCondition condition = new FilterCondition
            {
                Field = new ConversationField { Name = "Agent Assignee", DataType = FieldDataType.Option },
                MatchType = ConditionMatchType.Is,
                Value = "@Me"
            };

            var options = new ExpressionBuildOptions
            {
                UserId = 1,
                MyDepartmentMembers = new[] { 1, 2, 3 }
            };
            var expression = new AgentAssigneeExpression().SetOptions(options).Build(condition);

            var conditions = new List<Conversation>
            {
                new Conversation{Id=1,AgentId=1},
                new Conversation{Id=2,AgentId=null}
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
                Field = new ConversationField { Name = "Agent Assignee", DataType = FieldDataType.Option },
                MatchType = ConditionMatchType.IsNot,
                Value = "@Me"
            };

            var options = new ExpressionBuildOptions
            {
                UserId = 1,
                MyDepartmentMembers = new[] { 1, 2, 3 }
            };
            var expression = new AgentAssigneeExpression().SetOptions(options).Build(condition);

            var conditions = new List<Conversation>
            {
                new Conversation{Id=1,AgentId=1},
                new Conversation{Id=2,AgentId=null}
            }.AsQueryable();

            var result = conditions.Where(expression).ToList();

            Assert.Equal(1, result.Count);
            Assert.Equal(2, result.First().Id);
        }
    }
}
