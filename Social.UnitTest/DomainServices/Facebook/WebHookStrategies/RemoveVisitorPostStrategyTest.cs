using Framework.Core;
using Moq;
using Social.Domain.DomainServices;
using Social.Domain.DomainServices.Facebook;
using Social.Domain.Entities;
using Social.Infrastructure.Facebook;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Social.UnitTest.DomainServices.Facebook.WebHookStrategies
{
    public class RemoveVisitorPostStrategyTest
    {
        [Fact]
        public void ShouldMatchStrategy()
        {
            // Arrange
            var dependencyResolverMock = new Mock<IDependencyResolver>();
            var strategy = new RemoveVisitorPostStrategy(dependencyResolverMock.Object);
            var fbHookChange = new FbHookChange
            {
                Field = "feed",
                Value = new FbHookChangeValue
                {
                    Item = "post",
                    PostId = "post_1",
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
            var strategy = new RemoveVisitorPostStrategy(dependencyResolverMock.Object);
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
            FbHookChange change = MakeFbHookChangeForRemovingPost();
            var dependencyResolverMock = new Mock<IDependencyResolver>();
            var conversationServiceMock = new Mock<IConversationService>();
            conversationServiceMock.Setup(t => t.FindAll()).Returns(new List<Conversation>().AsQueryable());
            dependencyResolverMock.Setup(t => t.Resolve<IConversationService>()).Returns(conversationServiceMock.Object);
            var strategry = new RemoveVisitorPostStrategy(dependencyResolverMock.Object);
            var socialAccount = new SocialAccount { Token = "token" };

            // Act
            var processResult = await strategry.Process(socialAccount, change);

            // Assert
            Assert.Equal(0, processResult.DeletedConversations.Count());
        }

        [Fact]
        public async Task ShouldRemoveConversation()
        {
            // Arrange
            FbHookChange change = MakeFbHookChangeForRemovingPost();
            var dependencyResolverMock = new Mock<IDependencyResolver>();
            var conversationServiceMock = new Mock<IConversationService>();
            conversationServiceMock.Setup(t => t.FindAll()).Returns(new List<Conversation> {
                new Conversation{Id=1,OriginalId=change.Value.PostId}
            }.AsQueryable());
            dependencyResolverMock.Setup(t => t.Resolve<IConversationService>()).Returns(conversationServiceMock.Object);
            var strategry = new RemoveVisitorPostStrategy(dependencyResolverMock.Object);
            var socialAccount = new SocialAccount { Token = "token" };

            // Act
            var processResult = await strategry.Process(socialAccount, change);

            // Assert
            Assert.Equal(1, processResult.DeletedConversations.Count());
            var conversation = processResult.DeletedConversations.First();
            Assert.Equal(1, conversation.Id);
        }

        private FbHookChange MakeFbHookChangeForRemovingPost()
        {
            return new FbHookChange
            {
                Field = "feed",
                Value = new FbHookChangeValue
                {
                    Item = "post",
                    PostId = "post_1",
                    Verb = "remove"
                }
            };
        }
    }
}
