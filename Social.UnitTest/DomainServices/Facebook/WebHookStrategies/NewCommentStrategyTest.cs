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
    public class NewCommentStrategyTest
    {
        [Fact]
        public void ShouldMatchStrategy()
        {
            // Arrange
            var dependencyResolverMock = new Mock<IDependencyResolver>();
            var strategy = new NewCommentStrategy(dependencyResolverMock.Object);
            var fbHookChange = new FbHookChange
            {
                Field = "feed",
                Value = new FbHookChangeValue
                {
                    Item = "comment",
                    CommentId = "comment_1",
                    PostId = "post_1",
                    Verb = "add"
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
            var strategy = new NewCommentStrategy(dependencyResolverMock.Object);
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
        public async Task ShouldIgnoreDuplicatedMessage()
        {
            // Arrange
            var fbHookChange = MakeFbHootChangeForComment();
            var fbComment = MakeFbComment();
            var sender = new SocialUser { Id = 1, OriginalId = "user_1" };

            var dependencyResolverMock = MockDependencyResolver(fbHookChange, fbComment, sender);
            var messageServiceMock = new Mock<IMessageService>();
            // make duplicate message
            messageServiceMock.Setup(t => t.FindAll()).Returns(new List<Message> {
                new Message {OriginalId=fbComment.id}
            }.AsQueryable());
            dependencyResolverMock.Setup(t => t.Resolve<IMessageService>()).Returns(messageServiceMock.Object);
            var strategy = new NewCommentStrategy(dependencyResolverMock.Object);
            var socialAccount = new SocialAccount { Token = "token", IfConvertVisitorPostToConversation = true };

            // Act
            var processResult = await strategy.Process(socialAccount, fbHookChange);

            // Assert
            Assert.False(processResult.NewMessages.Any());
            Assert.False(processResult.NewConversations.Any());
            Assert.False(processResult.UpdatedConversations.Any());
        }

        [Fact]
        public async Task ShouldIngoreIfConversationNotExists()
        {
            // Arrange
            var fbHookChange = MakeFbHootChangeForComment();
            var fbComment = MakeFbComment();
            var sender = new SocialUser { Id = 1, OriginalId = "user_1" };

            var dependencyResolverMock = MockDependencyResolver(fbHookChange, fbComment, sender);
            var conversationServiceMock = new Mock<IConversationService>();
            // make duplicate message
            conversationServiceMock.Setup(t => t.FindAll()).Returns(new List<Conversation>().AsQueryable());
            dependencyResolverMock.Setup(t => t.Resolve<IConversationService>()).Returns(conversationServiceMock.Object);
            var strategy = new NewCommentStrategy(dependencyResolverMock.Object);
            var socialAccount = new SocialAccount { Token = "token", IfConvertVisitorPostToConversation = true };

            // Act
            var processResult = await strategy.Process(socialAccount, fbHookChange);

            // Assert
            Assert.False(processResult.NewMessages.Any());
            Assert.False(processResult.NewConversations.Any());
            Assert.False(processResult.UpdatedConversations.Any());
        }

        [Fact]
        public async Task ShouldNotifyNewMessage()
        {
            // Arrange
            var fbHookChange = MakeFbHootChangeForComment();
            var fbComment = MakeFbComment();
            var sender = new SocialUser { Id = 1, OriginalId = "user_1" };

            var dependencyResolverMock = MockDependencyResolver(fbHookChange, fbComment, sender);
            var strategy = new NewCommentStrategy(dependencyResolverMock.Object);
            strategy.UnitOfWorkManager = MockUnitOfWorkManager().Object;
            var socialAccount = new SocialAccount { Token = "token", IfConvertVisitorPostToConversation = true };

            // Act
            var processResult = await strategy.Process(socialAccount, fbHookChange);

            // Assert
            Assert.Equal(0, processResult.NewConversations.Count());
            Assert.Equal(1, processResult.UpdatedConversations.Count());
            Assert.Equal(1, processResult.NewMessages.Count());
        }

        [Fact]
        public async Task ShouldUpdateConversation()
        {
            // Arrange
            var fbHookChange = MakeFbHootChangeForComment();
            var fbComment = MakeFbComment();
            var sender = new SocialUser { Id = 1, OriginalId = "user_1" };

            var dependencyResolverMock = MockDependencyResolver(fbHookChange, fbComment, sender);
            var strategy = new NewCommentStrategy(dependencyResolverMock.Object);
            strategy.UnitOfWorkManager = MockUnitOfWorkManager().Object;
            var socialAccount = new SocialAccount { Token = "token", IfConvertVisitorPostToConversation = true };

            // Act
            var processResult = await strategy.Process(socialAccount, fbHookChange);

            // Assert
            var conversation = processResult.UpdatedConversations.First();
            Assert.NotNull(conversation);
            Assert.Equal(sender.Id, conversation.LastMessageSenderId);
            Assert.Equal(fbComment.created_time, conversation.LastMessageSentTime);
            Assert.Equal(ConversationStatus.PendingInternal, conversation.Status);
        }

        [Fact]
        public async Task ShouldUpdateConversationToVisibleForWallPost()
        {
            // Arrange
            var fbHookChange = MakeFbHootChangeForComment();
            var fbComment = MakeFbComment();
            var sender = new SocialUser { Id = 1, OriginalId = "user_1" };

            var dependencyResolverMock = MockDependencyResolver(fbHookChange, fbComment, sender);
            var conversationServiceMock = new Mock<IConversationService>();
            conversationServiceMock.Setup(t => t.FindAll()).Returns(new List<Conversation>
            {
                new Conversation{
                    OriginalId =fbComment.PostId,
                    Source =ConversationSource.FacebookWallPost,
                    IsHidden =true
                }
            }.AsQueryable());
            dependencyResolverMock.Setup(t => t.Resolve<IConversationService>()).Returns(conversationServiceMock.Object);
            var strategy = new NewCommentStrategy(dependencyResolverMock.Object);
            strategy.UnitOfWorkManager = MockUnitOfWorkManager().Object;
            var socialAccount = new SocialAccount { Token = "token", IfConvertVisitorPostToConversation = true };

            // Act
            var processResult = await strategy.Process(socialAccount, fbHookChange);

            // Assert
            var conversation = processResult.UpdatedConversations.First();
            Assert.False(conversation.IsHidden);
        }

        [Fact]
        public async Task ShouldCreateMessageForComment()
        {
            // Arrange
            var fbHookChange = MakeFbHootChangeForComment();
            var fbComment = MakeFbComment();
            var sender = new SocialUser { Id = 1, OriginalId = "user_1" };

            var dependencyResolverMock = MockDependencyResolver(fbHookChange, fbComment, sender);
            var strategy = new NewCommentStrategy(dependencyResolverMock.Object);
            strategy.UnitOfWorkManager = MockUnitOfWorkManager().Object;
            var socialAccount = new SocialAccount { Token = "token", IfConvertVisitorPostToConversation = true };

            // Act
            var processResult = await strategy.Process(socialAccount, fbHookChange);

            // Assert
            var message = processResult.UpdatedConversations.First().Messages.First(t => t.OriginalId == "comment_1");
            Assert.NotNull(message);
            Assert.Equal(MessageSource.FacebookPostComment, message.Source);
            Assert.Equal(fbComment.id, message.OriginalId);
            Assert.Equal(sender.Id, message.SenderId);
            Assert.Equal(fbComment.message, message.Content);
        }

        [Fact]
        public async Task ShouldSetParentForComment()
        {
            // Arrange
            var fbHookChange = MakeFbHootChangeForComment();
            var fbComment = MakeFbComment();
            fbComment.parent = null;
            var sender = new SocialUser { Id = 1, OriginalId = "user_1" };

            var dependencyResolverMock = MockDependencyResolver(fbHookChange, fbComment, sender);
            var messageServiceMock = new Mock<IMessageService>();
            // mock parent data
            messageServiceMock.Setup(t => t.FindByOriginalId(MessageSource.FacebookPost, fbComment.PostId)).Returns(new Message { Id = 111, SenderId = 222 });
            dependencyResolverMock.Setup(t => t.Resolve<IMessageService>()).Returns(messageServiceMock.Object);
            var strategy = new NewCommentStrategy(dependencyResolverMock.Object);
            strategy.UnitOfWorkManager = MockUnitOfWorkManager().Object;
            var socialAccount = new SocialAccount { Token = "token", IfConvertVisitorPostToConversation = true };

            // Act
            var processResult = await strategy.Process(socialAccount, fbHookChange);

            // Assert
            var message = processResult.UpdatedConversations.First().Messages.First(t => t.OriginalId == "comment_1");
            Assert.Equal(111, message.ParentId);
            Assert.Equal(222, message.ReceiverId);
        }

        [Fact]
        public async Task ShouldCreateMessageForReplyComment()
        {
            // Arrange
            var fbHookChange = MakeFbHootChangeForComment();
            var fbComment = MakeFbComment();
            fbComment.parent = new FbComment { PostId = fbComment.PostId, id = "parent_comment_id" };
            var sender = new SocialUser { Id = 1, OriginalId = "user_1" };

            var dependencyResolverMock = MockDependencyResolver(fbHookChange, fbComment, sender);
            var messageServiceMock = new Mock<IMessageService>();
            // mock parent data
            messageServiceMock.Setup(t => t.FindByOriginalId(MessageSource.FacebookPostComment, "parent_comment_id")).Returns(
                new Message { Id = 111, SenderId = 222, ParentId = 333, Parent = new Message { Id = 333, OriginalId = "post_id" } }
                );
            dependencyResolverMock.Setup(t => t.Resolve<IMessageService>()).Returns(messageServiceMock.Object);
            var strategy = new NewCommentStrategy(dependencyResolverMock.Object);
            strategy.UnitOfWorkManager = MockUnitOfWorkManager().Object;
            var socialAccount = new SocialAccount { Token = "token", IfConvertVisitorPostToConversation = true };

            // Act
            var processResult = await strategy.Process(socialAccount, fbHookChange);
            // Assert
            var message = processResult.UpdatedConversations.First().Messages.First(t => t.OriginalId == "comment_1");
            Assert.NotNull(message);
            Assert.Equal(MessageSource.FacebookPostReplyComment, message.Source);
            Assert.Equal(fbComment.id, message.OriginalId);
            Assert.Equal(sender.Id, message.SenderId);
            Assert.Equal(fbComment.message, message.Content);
            Assert.Equal(111, message.ParentId);
            Assert.Equal(222, message.ReceiverId);
        }

        [Fact]
        public async Task ShouldUpdateAndSaveChanges()
        {
            // Arrange
            var fbHookChange = MakeFbHootChangeForComment();
            var fbComment = MakeFbComment();
            var sender = new SocialUser { Id = 1, OriginalId = "user_1" };

            var dependencyResolverMock = MockDependencyResolver(fbHookChange, fbComment, sender);
            var conversationServiceMock = new Mock<IConversationService>();
            conversationServiceMock.Setup(t => t.FindAll()).Returns(new List<Conversation>
            {
                new Conversation{ OriginalId=fbComment.PostId }
            }.AsQueryable());
            dependencyResolverMock.Setup(t => t.Resolve<IConversationService>()).Returns(conversationServiceMock.Object);
            var strategy = new NewCommentStrategy(dependencyResolverMock.Object);
            var uowMock = new Mock<IUnitOfWork>();
            var uowManagerMock = new Mock<IUnitOfWorkManager>();
            uowManagerMock.Setup(t => t.Current).Returns(uowMock.Object);
            strategy.UnitOfWorkManager = uowManagerMock.Object;
            var socialAccount = new SocialAccount { Token = "token", IfConvertVisitorPostToConversation = true };

            // Act
            var processResult = await strategy.Process(socialAccount, fbHookChange);

            // Assert
            conversationServiceMock.Verify(t => t.UpdateAsync(It.IsAny<Conversation>()), "Should update conversation to db.");
            uowMock.Verify(t => t.SaveChangesAsync(), "Should save changes.");
        }

        private FbHookChange MakeFbHootChangeForComment()
        {
            return new FbHookChange
            {
                Field = "feed",
                Value = new FbHookChangeValue
                {
                    Item = "comment",
                    CommentId = "comment_1",
                    PostId = "post_1",
                    Verb = "add"
                }
            };
        }

        private FbComment MakeFbComment()
        {
            return new FbComment
            {
                id = "comment_1",
                PostId = "post_1",
                from = new FbUser { id = "user_1", name = "test_sender" },
                created_time = DateTime.UtcNow,
                message = "Test"
            };
        }

        private Mock<IDependencyResolver> MockDependencyResolver(FbHookChange change, FbComment fbComment, SocialUser sender)
        {
            var fbClientMock = new Mock<IFbClient>();
            fbClientMock.Setup(t => t.GetComment("token", change.Value.CommentId)).Returns(fbComment);
            var messageServiceMock = new Mock<IMessageService>();
            messageServiceMock.Setup(t => t.FindAll()).Returns(new List<Message>().AsQueryable());

            var socialUserServiceMock = new Mock<ISocialUserService>();
            socialUserServiceMock.Setup(t => t.GetOrCreateFacebookUser("token", fbComment.from.id)).ReturnsAsync(sender);

            var conversationServiceMock = new Mock<IConversationService>();
            conversationServiceMock.Setup(t => t.FindAll()).Returns(new List<Conversation>
            {
                new Conversation{ OriginalId=fbComment.PostId }
            }.AsQueryable());

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
