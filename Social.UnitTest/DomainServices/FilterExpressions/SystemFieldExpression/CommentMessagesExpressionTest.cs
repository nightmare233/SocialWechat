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
    public class CommentMessagesExpressionTest
    {
        [Fact]
        public void ShouldFilterByIsCondition()
        {
            FilterCondition condition = new FilterCondition
            {
                Field = new ConversationField { Name = "Comment/Messages", DataType = FieldDataType.String },
                MatchType = ConditionMatchType.Is,
                Value = "a"
            };

            var expression = new CommentMessagesExpression().Build(condition);

            var conditions = new List<Conversation>
            {
                new Conversation{Id=1,Messages = new List<Message> { new Message { Content  = "a"} } },
                new Conversation{Id=2,Messages = new List<Message> { new Message { Content  = "b"} }}
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
                Field = new ConversationField { Name = "Comment/Messages", DataType = FieldDataType.String },
                MatchType = ConditionMatchType.IsNot,
                Value = "a"
            };

            var expression = new CommentMessagesExpression().Build(condition);

            var conditions = new List<Conversation>
            {
                new Conversation{Id=1,Messages = new List<Message> { new Message { Content  = "a"} } },
                new Conversation{Id=2,Messages = new List<Message> { new Message { Content  = "b"} }}
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
                Field = new ConversationField { Name = "Comment/Messages", DataType = FieldDataType.String },
                MatchType = ConditionMatchType.Contain,
                Value = "a"
            };

            var expression = new CommentMessagesExpression().Build(condition);

            var conditions = new List<Conversation>
            {
                new Conversation{Id=1,Messages = new List<Message> { new Message { Content  = "abc"} } },
                new Conversation{Id=2,Messages = new List<Message> { new Message { Content  = "bbc"} }}
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
                Field = new ConversationField { Name = "Comment/Messages", DataType = FieldDataType.String },
                MatchType = ConditionMatchType.NotContain,
                Value = "a"
            };

            var expression = new CommentMessagesExpression().Build(condition);

            var conditions = new List<Conversation>
            {
                new Conversation{Id=1,Messages = new List<Message> { new Message { Content  = "abc"} } },
                new Conversation{Id=2,Messages = new List<Message> { new Message { Content  = "bbc"} }}
            }.AsQueryable();

            var result = conditions.Where(expression).ToList();

            Assert.Equal(1, result.Count);
            Assert.Equal(2, result.First().Id);
        }

    }
}
