using Framework.Core;
using Moq;
using Social.Domain.DomainServices;
using Social.Domain.DomainServices.Facebook;
using Social.Domain.Entities;
using Social.Infrastructure.Enum;
using Social.Infrastructure.Facebook;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Social.UnitTest.DomainServices.Facebook.WebHookStrategies
{
    public class RemovePostCommentStrategyTest
    {
        [Fact]
        public void ShouldMatchStrategy()
        {
            // Arrange
            var dependencyResolverMock = new Mock<IDependencyResolver>();
            var strategy = new RemovePostCommentStrategy(dependencyResolverMock.Object);
            var fbHookChange = new FbHookChange
            {
                Field = "feed",
                Value = new FbHookChangeValue
                {
                    Item = "comment",
                    PostId = "post_1",
                    CommentId = "comment_1",
                    Verb = "remove"
                }
            };

            // Act
            bool isMatch = strategy.IsMatch(fbHookChange);

            // Assert
            Assert.True(isMatch);
        }

        [Fact]
        public void ShouldNotMatchStrategy()
        {
            // Arrange
            var dependencyResolverMock = new Mock<IDependencyResolver>();
            var strategy = new RemovePostCommentStrategy(dependencyResolverMock.Object);
            var fbHookChange = new FbHookChange
            {
                Field = "feed",
                Value = new FbHookChangeValue
                {
                    Item = "post",
                    PostId = "post_1",
                    Verb = "add"
                }
            };

            // Act
            bool isMatch = strategy.IsMatch(fbHookChange);

            // Assert
            Assert.False(isMatch);
        }

        [Fact]
        public async Task ShouldIgnoreIfConversationNotExists()
        {
            // Arrange
            FbHookChange change = MakeFbHookChangeForRemovingComment();
            var dependencyResolverMock = new Mock<IDependencyResolver>();
            var messageServiceMock = new Mock<IMessageService>();
            messageServiceMock.Setup(
                t => t.FindByOriginalId(
                    new[] { MessageSource.FacebookPostComment, MessageSource.FacebookPostReplyComment },
                    change.Value.CommentId)
                    ).Returns<Message>(null);
            dependencyResolverMock.Setup(t => t.Resolve<IMessageService>()).Returns(messageServiceMock.Object);
            var strategry = new RemovePostCommentStrategy(dependencyResolverMock.Object);
            var socialAccount = new SocialAccount { Token = "token" };

            // Act
            var processResult = await strategry.Process(socialAccount, change);

            // Assert
            Assert.Equal(0, processResult.DeletedMessages.Count());
        }

        [Fact]
        public async Task ShouldRemoveConversation()
        {
            // Arrange
            FbHookChange change = MakeFbHookChangeForRemovingComment();
            var dependencyResolverMock = new Mock<IDependencyResolver>();
            var messageServiceMock = new Mock<IMessageService>();
            messageServiceMock.Setup(t =>
            t.FindByOriginalId(
                new[] { MessageSource.FacebookPostComment, MessageSource.FacebookPostReplyComment },
                change.Value.CommentId)
                ).Returns(new Message { Id = 111 });
            dependencyResolverMock.Setup(t => t.Resolve<IMessageService>()).Returns(messageServiceMock.Object);
            var strategry = new RemovePostCommentStrategy(dependencyResolverMock.Object);
            var socialAccount = new SocialAccount { Token = "token" };

            // Act
            var processResult = await strategry.Process(socialAccount, change);

            // Assert
            Assert.Equal(1, processResult.DeletedMessages.Count());
            var conversation = processResult.DeletedMessages.First();
            Assert.Equal(111, conversation.Id);
        }

        private FbHookChange MakeFbHookChangeForRemovingComment()
        {
            return new FbHookChange
            {
                Field = "feed",
                Value = new FbHookChangeValue
                {
                    Item = "comment",
                    PostId = "post_1",
                    CommentId = "comment_1",
                    Verb = "remove"
                }
            };
        }
    }
}
