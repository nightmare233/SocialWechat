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
    public class SocialAccountsExpressionTest
    {
        [Fact]
        public void ShouldFilterByIsCondition()
        {
            FilterCondition condition = new FilterCondition
            {
                Field = new ConversationField { Name = "Social Page/Account", DataType = FieldDataType.Option },
                MatchType = ConditionMatchType.Is,
                Value = "1"
            };

            var expression = new SocialAccountsExpression().Build(condition);

            var conditions = new List<Conversation>
            {
                new Conversation{Id=1,Messages = new List<Message> { new Message{ SenderId = 1} } },
                new Conversation{Id=2,Messages = new List<Message> { new Message{ SenderId = 2} } }
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
                Field = new ConversationField { Name = "Social Page/Account", DataType = FieldDataType.Option },
                MatchType = ConditionMatchType.IsNot,
                Value = "1"
            };

            var expression = new SocialAccountsExpression().Build(condition);

            var conditions = new List<Conversation>
            {
                new Conversation{Id=1,Messages = new List<Message> { new Message{ SenderId = 1} } },
                new Conversation{Id=2,Messages = new List<Message> { new Message{ SenderId = 2} } }
            }.AsQueryable();

            var result = conditions.Where(expression).ToList();

            Assert.Equal(1, result.Count);
            Assert.Equal(2, result.First().Id);
        }
    }
}
