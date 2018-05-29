using Social.Domain.Entities;
using Social.Infrastructure.Enum;
using Social.Infrastructure.LogicalExpression;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Xunit;

namespace Social.UnitTest.Infrastructure.LogicalExpression
{
    public class LogicalExpressionBuilderTest
    {
        [Fact]
        public void ShouldBuildSuccessWithOneExpression()
        {
            // Arrange
            var expressions = new Dictionary<int, Expression<Func<Conversation, bool>>>();
            expressions.Add(1, t => t.Source == ConversationSource.FacebookMessage);

            // Act
            var result = LogicalExpressionBuilder.Build(expressions, "1");

            // Assert
            Assert.True(result.IsSuccess);

            string expect = "t => (Convert(t.Source) == 1)";
            Assert.Equal(expect, result.Expression.ToString());
        }


        [Fact]
        public void ShouldBuildSuccessWithMultipleExpressions()
        {
            // Arrange
            var expressions = new Dictionary<int, Expression<Func<Conversation, bool>>>();
            expressions.Add(1, t => t.Source == ConversationSource.FacebookMessage);
            expressions.Add(2, t => t.Priority == ConversationPriority.Low);
            expressions.Add(3, t => t.Status == ConversationStatus.Closed);

            // Act
            var result = LogicalExpressionBuilder.Build(expressions, "(1 AND 2) OR 3");

            // Assert
            Assert.True(result.IsSuccess);

            string expect = "t => (((Convert(t.Source) == 1) AndAlso (Convert(t.Priority) == 0)) OrElse (Convert(t.Status) == 4))";
            Assert.Equal(expect, result.Expression.ToString());
        }

        [Fact]
        public void ShouldBuildSuccessWithComplexExpression()
        {
            // Arrange
            var expressions = new Dictionary<int, Expression<Func<Conversation, bool>>>();
            expressions.Add(1, t => t.Source == ConversationSource.FacebookMessage);
            expressions.Add(2, t => t.Priority == ConversationPriority.Low);
            expressions.Add(3, t => t.Status == ConversationStatus.Closed);
            expressions.Add(4, t => t.Messages.Any(m => m.Sender.Name.Contains("xxx")));

            // Act
            var result = LogicalExpressionBuilder.Build(expressions, "(1 AND 2) OR (3 AND 4 AND 1 OR 2)");

            // Assert
            Assert.True(result.IsSuccess);

            string expect = "t => (((Convert(t.Source) == 1) AndAlso (Convert(t.Priority) == 0)) OrElse ((((Convert(t.Status) == 4) AndAlso t.Messages.Any(m => m.Sender.Name.Contains(\"xxx\"))) AndAlso (Convert(t.Source) == 1)) OrElse (Convert(t.Priority) == 0)))";
            Assert.Equal(expect, result.Expression.ToString());
        }

        [Fact]
        public void ShouldBuildFailedWhenLogicalExpressionIsInvalid()
        {
            // Arrange
            var expressions = new Dictionary<int, Expression<Func<Conversation, bool>>>();
            expressions.Add(1, t => t.Source == ConversationSource.FacebookMessage);

            // Act
            var result = LogicalExpressionBuilder.Build(expressions, "(a AND b) OR c");

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Invalid Expression.", result.ErrorMessage);
        }

        [Fact]
        public void ShouldBuildFailedWhenExpressionNotFound()
        {
            // Arrange
            var expressions = new Dictionary<int, Expression<Func<Conversation, bool>>>();
            expressions.Add(1, t => t.Source == ConversationSource.FacebookMessage);

            // Act
            var result = LogicalExpressionBuilder.Build(expressions, "(1 AND 2) OR 3");

            // Assert
            Assert.False(result.IsSuccess);
        }
    }
}
