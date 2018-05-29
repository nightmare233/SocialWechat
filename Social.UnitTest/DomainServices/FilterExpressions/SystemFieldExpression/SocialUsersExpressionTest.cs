using Social.Domain.DomainServices.FilterExpressions.SystemFiledExpression;
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
    public class SocialUsersExpressionTest
    {
        [Fact]
        public void ShouldFilterByIsCondition()
        {
            FilterCondition condition = new FilterCondition
            {
                Field = new ConversationField { Name = "Social User", DataType = FieldDataType.String },
                MatchType = ConditionMatchType.Is,
                Value = "a"
            };

            var expression = new SocialUsersExpression().Build(condition);

            var conditions = new List<Conversation>
            {
                new Conversation{Id=1,Messages = new List<Message> { new Message { Sender  = new SocialUser{Name = "a"} } } },
                new Conversation{Id=2,Messages = new List<Message> { new Message { Sender  = new SocialUser{Name = "b"} } }}
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
                Field = new ConversationField { Name = "Social User", DataType = FieldDataType.String },
                MatchType = ConditionMatchType.IsNot,
                Value = "a"
            };

            var expression = new SocialUsersExpression().Build(condition);

            var conditions = new List<Conversation>
            {
                new Conversation{Id=1,Messages = new List<Message> { new Message { Sender = new SocialUser { Name = "a" } } } },
                new Conversation{Id=2,Messages = new List<Message> { new Message { Sender = new SocialUser { Name = "b" } } }}
            }.AsQueryable();

            var result = conditions.Where(expression).ToList();

            Assert.Equal(1, result.Count);
            Assert.Equal(2, result.First().Id);
        }

        [Fact]
        public void ShouldFilterByContainCondition()
        {
            FilterCondition condition = new FilterCondition
            {
                Field = new ConversationField { Name = "Social User", DataType = FieldDataType.String },
                MatchType = ConditionMatchType.Contain,
                Value = "a"
            };

            var expression = new SocialUsersExpression().Build(condition);

            var conditions = new List<Conversation>
            {
                new Conversation{Id=1,Messages = new List<Message> { new Message { Sender = new SocialUser { Name = "abc" } } } },
                new Conversation{Id=2,Messages = new List<Message> { new Message { Sender = new SocialUser { Name = "bbc" } } }}
            }.AsQueryable();

            var result = conditions.Where(expression).ToList();

            Assert.Equal(1, result.Count);
            Assert.Equal(1, result.First().Id);
        }

        [Fact]
        public void ShouldFilterByNotContainCondition()
        {
            FilterCondition condition = new FilterCondition
            {
                Field = new ConversationField { Name = "Social User", DataType = FieldDataType.String },
                MatchType = ConditionMatchType.NotContain,
                Value = "a"
            };

            var expression = new SocialUsersExpression().Build(condition);

            var conditions = new List<Conversation>
            {
                new Conversation{Id=1,Messages = new List<Message> { new Message { Sender = new SocialUser { Name = "abc" } } } },
                new Conversation{Id=2,Messages = new List<Message> { new Message { Sender = new SocialUser { Name = "bbc" } } }}
            }.AsQueryable();

            var result = conditions.Where(expression).ToList();

            Assert.Equal(1, result.Count);
            Assert.Equal(2, result.First().Id);
        }
    }
}
