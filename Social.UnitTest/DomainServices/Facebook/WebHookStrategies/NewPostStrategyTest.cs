using Framework.Core;
using Framework.Core.UnitOfWork;
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
    public class NewPostStrategyTest
    {
        [Fact]
        public void ShouldMatchStrategyIfVisitorAddTextPost()
        {
            // Arrange
            var dependencyResolverMock = new Mock<IDependencyResolver>();
            var strategy = new NewPostStrategy(dependencyResolverMock.Object);
            var fbHookChange = new FbHookChange
            {
                Field = "feed",
                Value = new FbHookChangeValue
                {
                    Item = "post",
                    PostId = "123",
                    Verb = "add"
                }
            };

            // Act
            bool isMatch = strategy.IsMatch(fbHookChange);

            // Assert
            Assert.True(isMatch);
        }

        [Fact]
        public void ShouldMatchStrategyIfVisitorAddPhotoOrVideoPost()
        {
            // Arrange
            var dependencyResolverMock = new Mock<IDependencyResolver>();
            var strategy = new NewPostStrategy(dependencyResolverMock.Object);
            var fbHookChange = new FbHookChange
            {
                Field = "feed",
                Value = new FbHookChangeValue
                {
                    PostId = "123",
                    Verb = "add",
                    IsPublished = true,
                    Link = "http://www.abc.com/test.jpg"
                }
            };

            // Act
            bool isMatch = strategy.IsMatch(fbHookChange);

            // Assert
            Assert.True(isMatch);
        }

        [Fact]
        public void ShouldMatchStrategyIfWallPost()
        {
            // Arrange
            var dependencyResolverMock = new Mock<IDependencyResolver>();
            var strategy = new NewPostStrategy(dependencyResolverMock.Object);
            var fbHookChange = new FbHookChange
            {
                Field = "feed",
                Value = new FbHookChangeValue
                {
                    Item = "status",
                    PostId = "123",
                    Verb = "add",
                    IsPublished = true,
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
            var strategy = new NewPostStrategy(dependencyResolverMock.Object);
            var fbHookChange = new FbHookChange
            {
                Field = "conversation",
            };

            // Act
            bool isMatch = strategy.IsMatch(fbHookChange);

            // Assert
            Assert.False(isMatch);
        }

        [Fact]
        public async Task ShouldIgnoreWallPostIfDisabledConvertWallPostToConversation()
        {
            // Arrange
            var dependencyResolverMock = new Mock<IDependencyResolver>();
            var strategy = new NewPostStrategy(dependencyResolverMock.Object);
            var fbHookChange = new FbHookChange
            {
                Field = "feed",
                Value = new FbHookChangeValue
                {
                    Item = "status",
                    PostId = "123",
                    Verb = "add",
                    IsPublished = true,
                }
            };
            var socialAccount = new SocialAccount
            {
                IfConvertWallPostToConversation = false
            };

            // Act
            var processResult = await strategy.Process(socialAccount, fbHookChange);

            // Assert
            Assert.False(processResult.NewMessages.Any());
            Assert.False(processResult.NewConversations.Any());
            Assert.False(processResult.UpdatedConversations.Any());
        }

        [Fact]
        public async Task ShouldIgnoreVisitorPostIfDisabledConvertPostToConversation()
        {
            // Arrange
            var dependencyResolverMock = new Mock<IDependencyResolver>();
            var strategy = new NewPostStrategy(dependencyResolverMock.Object);
            var fbHookChange = new FbHookChange
            {
                Field = "feed",
                Value = new FbHookChangeValue
                {
                    Item = "post",
                    PostId = "123",
                    Verb = "add"
                }
            };
            var socialAccount = new SocialAccount
            {
                IfConvertVisitorPostToConversation = false
            };

            // Act
            var processResult = await strategy.Process(socialAccount, fbHookChange);

            // Assert
            Assert.False(processResult.NewMessages.Any());
            Assert.False(processResult.NewConversations.Any());
            Assert.False(processResult.UpdatedConversations.Any());
        }

        [Fact]
        public async Task ShouldIgnoreDuplicatedMessage()
        {
            // Arrange
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
            var fbPost = new FbPost
            {
                id = "post_1",
                from = new FbUser { id = "user_1", name = "test_sender" }
            };
            var sender = new SocialUser { Id = 1, OriginalId = "user_1" };

            var dependencyResolverMock = MockDependencyResolver(fbHookChange, fbPost, sender);
            var messageServiceMock = new Mock<IMessageService>();
            // make duplicate message
            messageServiceMock.Setup(t => t.IsDuplicatedMessage(MessageSource.FacebookPost, "post_1")).Returns(true);
            dependencyResolverMock.Setup(t => t.Resolve<IMessageService>()).Returns(messageServiceMock.Object);
            var strategy = new NewPostStrategy(dependencyResolverMock.Object);
            var socialAccount = new SocialAccount { Token = "token", IfConvertVisitorPostToConversation = true };

            // Act
            var processResult = await strategy.Process(socialAccount, fbHookChange);

            // Assert
            Assert.False(processResult.NewMessages.Any());
            Assert.False(processResult.NewConversations.Any());
            Assert.False(processResult.UpdatedConversations.Any());
        }

        [Fact]
        public async Task ShouldNotifyNewConversation()
        {
            // Arrange
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
            var fbPost = new FbPost
            {
                id = "post_1",
                from = new FbUser { id = "user_1", name = "test_sender" },
                created_time = DateTime.UtcNow,
                message = "Test_Message"
            };
            var sender = new SocialUser { Id = 1, OriginalId = "user_1" };

            var dependencyResolverMock = MockDependencyResolver(fbHookChange, fbPost, sender);
            var strategy = new NewPostStrategy(dependencyResolverMock.Object);
            strategy.UnitOfWorkManager = MockUnitOfWorkManager().Object;
            var socialAccount = new SocialAccount { Id = 888, Token = "token", IfConvertVisitorPostToConversation = true };

            // Act
            var processResult = await strategy.Process(socialAccount, fbHookChange);

            // Assert
            Assert.Equal(1, processResult.NewConversations.Count());
            Assert.Equal(0, processResult.UpdatedConversations.Count());
            Assert.Equal(0, processResult.NewMessages.Count());
        }

        [Fact]
        public async Task ShouldCreateConversationForVisitorPost()
        {
            // Arrange
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
            var fbPost = new FbPost
            {
                id = "post_1",
                from = new FbUser { id = "user_1", name = "test_sender" },
                created_time = DateTime.UtcNow,
                message = "Test_Message"
            };
            var sender = new SocialUser { Id = 1, OriginalId = "user_1" };

            var dependencyResolverMock = MockDependencyResolver(fbHookChange, fbPost, sender);
            var strategy = new NewPostStrategy(dependencyResolverMock.Object);
            strategy.UnitOfWorkManager = MockUnitOfWorkManager().Object;
            var socialAccount = new SocialAccount { Id = 888, Token = "token", IfConvertVisitorPostToConversation = true };

            // Act
            var processResult = await strategy.Process(socialAccount, fbHookChange);

            // Assert
            var conversation = processResult.NewConversations.First();
            Assert.NotNull(conversation);
            Assert.Equal(ConversationSource.FacebookVisitorPost, conversation.Source);
            Assert.Equal(ConversationStatus.New, conversation.Status);
            Assert.Equal("post_1", conversation.OriginalId);
            Assert.Equal(ConversationPriority.Normal, conversation.Priority);
            Assert.Equal(1, conversation.LastMessageSenderId);
            Assert.Equal(fbPost.created_time, conversation.LastMessageSentTime);
            Assert.Equal("Test_Message", conversation.Subject);
        }

        [Fact]
        public async Task ShouldInsertAndSaveChangesWhenCreateConversation()
        {
            // Arrange
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
            var fbPost = new FbPost
            {
                id = "post_1",
                from = new FbUser { id = "user_1", name = "test_sender" },
                created_time = DateTime.UtcNow,
                message = "Test_Message"
            };
            var sender = new SocialUser { Id = 1, OriginalId = "user_1" };

            var dependencyResolverMock = MockDependencyResolver(fbHookChange, fbPost, sender);
            var conversationServiceMock = new Mock<IConversationService>();
            dependencyResolverMock.Setup(t => t.Resolve<IConversationService>()).Returns(conversationServiceMock.Object);
            var strategy = new NewPostStrategy(dependencyResolverMock.Object);
            var uowMock = new Mock<IUnitOfWork>();
            var uowManagerMock = new Mock<IUnitOfWorkManager>();
            uowManagerMock.Setup(t => t.Current).Returns(uowMock.Object);
            strategy.UnitOfWorkManager = uowManagerMock.Object;
            var socialAccount = new SocialAccount { Id = 888, Token = "token", IfConvertVisitorPostToConversation = true };

            // Act
            var processResult = await strategy.Process(socialAccount, fbHookChange);

            // Assert
            conversationServiceMock.Verify(t => t.InsertAsync(It.IsAny<Conversation>()), "Should add conversation to db.");
            uowMock.Verify(t => t.SaveChangesAsync(), "Should save changes.");
        }

        [Fact]
        public async Task ShouldCreateMessageForPost()
        {
            // Arrange
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
            var fbPost = new FbPost
            {
                id = "post_1",
                from = new FbUser { id = "user_1", name = "test_sender" },
                created_time = DateTime.UtcNow,
                message = "Test_Message"
            };
            var sender = new SocialUser { Id = 1, OriginalId = "user_1" };

            var dependencyResolverMock = MockDependencyResolver(fbHookChange, fbPost, sender);
            var strategy = new NewPostStrategy(dependencyResolverMock.Object);
            strategy.UnitOfWorkManager = MockUnitOfWorkManager().Object;
            var socialAccount = new SocialAccount { Id = 888, Token = "token", IfConvertVisitorPostToConversation = true };

            // Act
            var processResult = await strategy.Process(socialAccount, fbHookChange);

            // Assert
            var message = processResult.NewConversations.First().Messages.FirstOrDefault(t => t.OriginalId == "post_1");
            Assert.NotNull(message);
            Assert.Equal(MessageSource.FacebookPost, message.Source);
            Assert.Equal(fbPost.id, message.OriginalId);
            Assert.Equal(sender.Id, message.SenderId);
            Assert.Equal(socialAccount.Id, message.ReceiverId);
            Assert.Equal(fbPost.message, message.Content);
        }

        [Fact]
        public async Task ShouldCreateConversationForWallPost()
        {
            // Arrange
            var fbHookChange = new FbHookChange
            {
                Field = "feed",
                Value = new FbHookChangeValue
                {
                    Item = "status",
                    PostId = "post_1",
                    Verb = "add",
                    IsPublished = true
                }
            };
            var fbPost = new FbPost
            {
                id = "post_1",
                from = new FbUser { id = "user_1", name = "test_sender" },
                created_time = DateTime.UtcNow,
                message = "Test_Message"
            };
            var sender = new SocialUser { Id = 888, OriginalId = "user_1" };

            var dependencyResolverMock = MockDependencyResolver(fbHookChange, fbPost, sender);
            var strategy = new NewPostStrategy(dependencyResolverMock.Object);
            strategy.UnitOfWorkManager = MockUnitOfWorkManager().Object;
            var socialAccount = new SocialAccount { Id = 888, Token = "token", IfConvertWallPostToConversation = true };

            // Act
            var processResult = await strategy.Process(socialAccount, fbHookChange);

            // Assert
            var conversation = processResult.NewConversations.First();
            Assert.NotNull(conversation);
            Assert.Equal(true, conversation.IsHidden);
            Assert.Equal(ConversationSource.FacebookWallPost, conversation.Source);
            Assert.Equal(ConversationStatus.New, conversation.Status);
            Assert.Equal("post_1", conversation.OriginalId);
            Assert.Equal(ConversationPriority.Normal, conversation.Priority);
            Assert.Equal(888, conversation.LastMessageSenderId);
            Assert.Equal(fbPost.created_time, conversation.LastMessageSentTime);
            Assert.Equal("Test_Message", conversation.Subject);
        }

        private Mock<IDependencyResolver> MockDependencyResolver(FbHookChange change, FbPost fbPost, SocialUser sender)
        {
            var fbClientMock = new Mock<IFbClient>();
            fbClientMock.Setup(t => t.GetPost("token", change.Value.PostId)).ReturnsAsync(fbPost);
            var messageServiceMock = new Mock<IMessageService>();
            messageServiceMock.Setup(t => t.FindAll()).Returns(new List<Message>().AsQueryable());

            var socialUserServiceMock = new Mock<ISocialUserService>();
            socialUserServiceMock.Setup(t => t.GetOrCreateFacebookUser("token", fbPost.from.id)).ReturnsAsync(sender);

            var conversationServiceMock = new Mock<IConversationService>();

            var dependencyResolverMock = new Mock<IDependencyResolver>();
            dependencyResolverMock.Setup(t => t.Resolve<IFbClient>()).Returns(fbClientMock.Object);
            dependencyResolverMock.Setup(t => t.Resolve<IMessageService>()).Returns(messageServiceMock.Object);
            dependencyResolverMock.Setup(t => t.Resolve<ISocialUserService>()).Returns(socialUserServiceMock.Object);
            dependencyResolverMock.Setup(t => t.Resolve<IConversationService>()).Returns(conversationServiceMock.Object);

            return dependencyResolverMock;
        }

        private Mock<IUnitOfWorkManager> MockUnitOfWorkManager()
        {
            var uowMock = new Mock<IUnitOfWork>();
            var uowManagerMock = new Mock<IUnitOfWorkManager>();
            uowManagerMock.Setup(t => t.Current).Returns(uowMock.Object);
            return uowManagerMock;
        }
    }
}
