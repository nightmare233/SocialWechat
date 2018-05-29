using Moq;
using Social.Domain.DomainServices;
using Social.Domain.Entities;
using Social.Infrastructure.Enum;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Social.UnitTest.DomainService.FilterExpressions.SystemFieldExpression
{
    public class AgentAssigneeStatusExpressionTest
    {
        [Fact]
        public void ShouldFilterByIsCondition()
        {
            var agentServiceMock = new Mock<IAgentService>();
            agentServiceMock.Setup(t => t.GetMatchedStatusAgents(1)).Returns(new[] { 1, 3});


            FilterCondition condition = new FilterCondition
            {
                Field = new ConversationField { Name = "Agent Assignee Status", DataType = FieldDataType.Option },
                MatchType = ConditionMatchType.Is,
                Value = "1"
            };
            var expression = new AgentAssigneeStatusExpression(agentServiceMock.Object).Build(condition);

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
            var agentServiceMock = new Mock<IAgentService>();
            agentServiceMock.Setup(t => t.GetMatchedStatusAgents(1)).Returns(new[] { 1, 3 });


            FilterCondition condition = new FilterCondition
            {
                Field = new ConversationField { Name = "Agent Assignee Status", DataType = FieldDataType.Option },
                MatchType = ConditionMatchType.IsNot,
                Value = "1"
            };
            var expression = new AgentAssigneeStatusExpression(agentServiceMock.Object).Build(condition);

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
