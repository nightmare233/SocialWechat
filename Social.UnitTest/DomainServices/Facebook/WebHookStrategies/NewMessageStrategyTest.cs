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
    public class NewMessageStrategyTest
    {
        [Fact]
        public void ShouldMatchStrategy()
        {
            // Arrange
            var dependencyResolverMock = new Mock<IDependencyResolver>();
            var strategy = new NewMessageStrategy(dependencyResolverMock.Object);
            var fbHookChange = new FbHookChange
            {
                Field = "conversations",
                Value = new FbHookChangeValue
                {
                    ThreadId = "123"
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
            var strategy = new NewMessageStrategy(dependencyResolverMock.Object);
            var fbHookChange = new FbHookChange
            {
                Field = "post",
                Value = new FbHookChangeValue
                {
                    ThreadId = "123"
                }
            };

            // Act
            bool isMatch = strategy.IsMatch(fbHookChange);

            // Assert
            Assert.False(isMatch);
        }

        [Fact]
        public async Task ShouldIgnore_IfDisabledConvertMessageToConversation()
        {
            // Arrange
            var dependencyResolverMock = new Mock<IDependencyResolver>();
            var strategy = new NewMessageStrategy(dependencyResolverMock.Object);
            var socialAccount = new SocialAccount
            {
                IfConvertMessageToConversation = false
            };

            // Act
            var processResult = await strategy.Process(socialAccount, new FbHookChange());

            // Assert
            Assert.False(processResult.NewMessages.Any());
            Assert.False(processResult.NewConversations.Any());
            Assert.False(processResult.UpdatedConversations.Any());
        }

        [Fact]
        public async Task ShouldIngoreDuplicatedMessages()
        {
            // Arrange
            var fbChange = new FbHookChange
            {
                Value = new FbHookChangeValue { ThreadId = "th_1" }
            };
            var fbClientMock = new Mock<IFbClient>();
            fbClientMock.Setup(t => t.GetMessagesFromConversationId("token", "th_1", 10)).ReturnsAsync(
                new List<FbMessage>
                {
                    new FbMessage{Id="fb_1",SenderId="user_1",ReceiverId="user_2"}
                });
            var messageServiceMock = new Mock<IMessageService>();
            messageServiceMock.Setup(t => t.FindAll()).Returns(new List<Message>
            {
                new Message{Source=MessageSource.FacebookMessage,OriginalId="fb_1"} // duplicate message
            }.AsQueryable());

            var dependencyResolverMock = new Mock<IDependencyResolver>();
            dependencyResolverMock.Setup(t => t.Resolve<IFbClient>()).Returns(fbClientMock.Object);
            dependencyResolverMock.Setup(t => t.Resolve<IMessageService>()).Returns(messageServiceMock.Object);

            var strategy = new NewMessageStrategy(dependencyResolverMock.Object);
            var socialAccount = new SocialAccount { Token = "token", IfConvertMessageToConversation = true };

            // Act
            var processResult = await strategy.Process(socialAccount, fbChange);

            // Assert
            Assert.False(processResult.NewMessages.Any());
            Assert.False(processResult.NewConversations.Any());
            Assert.False(processResult.UpdatedConversations.Any());
        }

        [Fact]
        public async Task ShouldIngore_IfSenderIsIntegrationAccountItself_WhenCreateNewConversation()
        {
            // Arrange
            var fbChange = new FbHookChange { Value = new FbHookChangeValue { ThreadId = "original_conversation_id" } };
            var fbMessage = new FbMessage { Id = "fb_1", Content = "Test_Message", SenderId = "user_1", ReceiverId = "user_2", SendTime = DateTime.UtcNow };
            var sender = new SocialUser { Id = 888, OriginalId = "account_1" };
            var receiver = new SocialUser { Id = 2, OriginalId = "user_2" };
            IDependencyResolver dependencyResolver = MockDependecyResolverForTestingCreateConversation(fbChange, fbMessage, sender, receiver).Object;
            var strategy = new NewMessageStrategy(dependencyResolver);
            strategy.UnitOfWorkManager = MockUnitOfWorkManager().Object;
            var socialAccount = new SocialAccount { Id = 888, Token = "token", IfConvertMessageToConversation = true };

            //  Act
            var processResult = await strategy.Process(socialAccount, fbChange);

            // Assert
            Assert.Equal(0, processResult.NewConversations.Count());
        }

        [Fact]
        public async Task ShouldSetValueForNewConversation()
        {
            // Arrange
            var fbChange = new FbHookChange { Value = new FbHookChangeValue { ThreadId = "original_conversation_id" } };
            var fbMessage = new FbMessage { Id = "fb_1", Content = "Test_Message", SenderId = "user_1", ReceiverId = "user_2", SendTime = DateTime.UtcNow };
            var sender = new SocialUser { Id = 1, OriginalId = "user_1" };
            var receiver = new SocialUser { Id = 2, OriginalId = "user_2" };
            IDependencyResolver dependencyResolver = MockDependecyResolverForTestingCreateConversation(fbChange, fbMessage, sender, receiver).Object;
            var strategy = new NewMessageStrategy(dependencyResolver);
            strategy.UnitOfWorkManager = MockUnitOfWorkManager().Object;
            var socialAccount = new SocialAccount { Token = "token", IfConvertMessageToConversation = true };

            // Act
            var processResult = await strategy.Process(socialAccount, fbChange);

            // Assert
            var converssation = processResult.NewConversations.First();
            Assert.Equal(ConversationSource.FacebookMessage, converssation.Source);
            Assert.Equal(ConversationStatus.New, converssation.Status);
            Assert.Equal("original_conversation_id", converssation.OriginalId);
            Assert.Equal(ConversationPriority.Normal, converssation.Priority);
            Assert.Equal(1, converssation.LastMessageSenderId);
            Assert.Equal(fbMessage.SendTime, converssation.LastMessageSentTime);
            Assert.Equal("Test_Message", converssation.Subject);
        }

        [Fact]
        public async Task ShouldAddMessageWhenCreateNewConversation()
        {
            // Arrange
            var fbChange = new FbHookChange { Value = new FbHookChangeValue { ThreadId = "original_conversation_id" } };
            var fbMessage = new FbMessage { Id = "fb_1", Link = "http://test.com", Content = "Test_Message", SenderId = "user_1", ReceiverId = "user_2", SendTime = DateTime.UtcNow };
            var sender = new SocialUser { Id = 1, OriginalId = "user_1" };
            var receiver = new SocialUser { Id = 2, OriginalId = "user_2" };
            IDependencyResolver dependencyResolver = MockDependecyResolverForTestingCreateConversation(fbChange, fbMessage, sender, receiver).Object;
            var strategy = new NewMessageStrategy(dependencyResolver);
            strategy.UnitOfWorkManager = MockUnitOfWorkManager().Object;
            var socialAccount = new SocialAccount { Token = "token", IfConvertMessageToConversation = true };

            // Act
            var processResult = await strategy.Process(socialAccount, fbChange);

            // Assert
            var messages = processResult.NewConversations.First().Messages;
            Assert.Equal(1, messages.Count());
            var message = messages.First();
            Assert.Equal(MessageSource.FacebookMessage, message.Source);
            Assert.Equal(fbMessage.Id, message.OriginalId);
            Assert.Equal(sender.Id, message.SenderId);
            Assert.Equal(receiver.Id, message.ReceiverId);
            Assert.Equal(fbMessage.Content, message.Content);
        }

        [Fact]
        public async Task ShouldInsertConversationAndSaveChangesWhenCreateNewConversation()
        {
            // Arrange
            var fbChange = new FbHookChange { Value = new FbHookChangeValue { ThreadId = "original_conversation_id" } };
            var fbMessage = new FbMessage { Id = "fb_1", Link = "http://test.com", Content = "Test_Message", SenderId = "user_1", ReceiverId = "user_2", SendTime = DateTime.UtcNow };
            var sender = new SocialUser { Id = 1, OriginalId = "user_1" };
            var receiver = new SocialUser { Id = 2, OriginalId = "user_2" };
            Mock<IDependencyResolver> dependencyResolverMock = MockDependecyResolverForTestingCreateConversation(fbChange, fbMessage, sender, receiver);
            var conversationServiceMock = new Mock<IConversationService>();
            // If we can't find a un closed conversation, then we will create a new conversation.
            conversationServiceMock.Setup(t => t.GetUnClosedConversation(fbChange.Value.ThreadId)).Returns<Conversation>(null);
            dependencyResolverMock.Setup(t => t.Resolve<IConversationService>()).Returns(conversationServiceMock.Object);
            var strategy = new NewMessageStrategy(dependencyResolverMock.Object);
            var uowMock = new Mock<IUnitOfWork>();
            var uowManagerMock = new Mock<IUnitOfWorkManager>();
            uowManagerMock.Setup(t => t.Current).Returns(uowMock.Object);
            strategy.UnitOfWorkManager = uowManagerMock.Object;
            var socialAccount = new SocialAccount { Token = "token", IfConvertMessageToConversation = true };

            // Act
            var processResult = await strategy.Process(socialAccount, fbChange);

            // Assert
            conversationServiceMock.Verify(t => t.InsertAsync(It.IsAny<Conversation>()), "Should add conversation to db.");
            uowMock.Verify(t => t.SaveChangesAsync(), "Should save changes.");
        }

        [Fact]
        public async Task ShouldNofityNewConversation()
        {
            // Arrange
            var fbChange = new FbHookChange { Value = new FbHookChangeValue { ThreadId = "original_conversation_id" } };
            var fbMessage = new FbMessage { Id = "fb_1", Content = "Test_Message", SenderId = "user_1", ReceiverId = "user_2", SendTime = DateTime.UtcNow };
            var sender = new SocialUser { Id = 1, OriginalId = "user_1" };
            var receiver = new SocialUser { Id = 2, OriginalId = "user_2" };
            IDependencyResolver dependencyResolver = MockDependecyResolverForTestingCreateConversation(fbChange, fbMessage, sender, receiver).Object;
            var strategy = new NewMessageStrategy(dependencyResolver);
            strategy.UnitOfWorkManager = MockUnitOfWorkManager().Object;
            var socialAccount = new SocialAccount { Token = "token", IfConvertMessageToConversation = true };

            // Act
            var processResult = await strategy.Process(socialAccount, fbChange);

            // Assert
            Assert.Equal(1, processResult.NewConversations.Count());
            Assert.Equal(0, processResult.UpdatedConversations.Count());
            Assert.Equal(0, processResult.NewMessages.Count());
        }

        [Fact]
        public async Task ShouldUpdateConversatonWhenAddNewMessageToUnClosedConversation()
        {
            // Arrange
            var fbChange = new FbHookChange { Value = new FbHookChangeValue { ThreadId = "original_conversation_id" } };
            var fbMessage = new FbMessage { Id = "fb_1", Content = "Test_Message", SenderId = "user_1", ReceiverId = "user_2", SendTime = DateTime.UtcNow };
            var sender = new SocialUser { Id = 1, OriginalId = "user_1" };
            var receiver = new SocialUser { Id = 2, OriginalId = "user_2" };
            var conversation = new Conversation { OriginalId = fbChange.Value.ThreadId };
            IDependencyResolver dependencyResolver = MockDependecyResolverForTestingUpdateConversation(conversation, fbChange, fbMessage, sender, receiver).Object;
            var strategy = new NewMessageStrategy(dependencyResolver);
            strategy.UnitOfWorkManager = MockUnitOfWorkManager().Object;
            var socialAccount = new SocialAccount { Token = "token", IfConvertMessageToConversation = true, SocialUser = new SocialUser { Id = 3 } };

            // Act
            var processResult = await strategy.Process(socialAccount, fbChange);

            // Assert
            Assert.Equal(1, processResult.UpdatedConversations.Count());
            Assert.Equal(false, conversation.IfRead);
            Assert.Equal(ConversationStatus.PendingInternal, conversation.Status);
            Assert.Equal(sender.Id, conversation.LastMessageSenderId);
            Assert.Equal(fbMessage.SendTime, conversation.LastMessageSentTime);
        }

        [Fact]
        public async Task ShouldUpdateConversatonAndSaveChanges()
        {
            // Arrange
            var fbChange = new FbHookChange { Value = new FbHookChangeValue { ThreadId = "original_conversation_id" } };
            var fbMessage = new FbMessage { Id = "fb_1", Content = "Test_Message", SenderId = "user_1", ReceiverId = "user_2", SendTime = DateTime.UtcNow };
            var sender = new SocialUser { Id = 1, OriginalId = "user_1" };
            var receiver = new SocialUser { Id = 2, OriginalId = "user_2" };
            var conversation = new Conversation { OriginalId = fbChange.Value.ThreadId };

            Mock<IDependencyResolver> dependencyResolverMock = MockDependecyResolverForTestingUpdateConversation(conversation, fbChange, fbMessage, sender, receiver);
            var conversationServiceMock = new Mock<IConversationService>();
            // If we can find a un-closed conversation, then we will add the new message to un-closed conversation.
            conversationServiceMock.Setup(t => t.GetUnClosedConversation(fbChange.Value.ThreadId)).Returns(conversation);
            dependencyResolverMock.Setup(t => t.Resolve<IConversationService>()).Returns(conversationServiceMock.Object);
            var strategy = new NewMessageStrategy(dependencyResolverMock.Object);
            var uowMock = new Mock<IUnitOfWork>();
            var uowManagerMock = new Mock<IUnitOfWorkManager>();
            uowManagerMock.Setup(t => t.Current).Returns(uowMock.Object);
            strategy.UnitOfWorkManager = uowManagerMock.Object;
            var socialAccount = new SocialAccount { Token = "token", IfConvertMessageToConversation = true, SocialUser = new SocialUser { Id = 3 } };

            // Act
            var processResult = await strategy.Process(socialAccount, fbChange);

            // Assert
            conversationServiceMock.Verify(t => t.UpdateAsync(It.IsAny<Conversation>()), "Should update conversation to db.");
            uowMock.Verify(t => t.SaveChangesAsync(), "Should save changes.");
        }

        [Fact]
        public async Task ShouldAddMessageToUnClosedConversation()
        {
            // Arrange
            var fbChange = new FbHookChange { Value = new FbHookChangeValue { ThreadId = "original_conversation_id" } };
            var fbMessage = new FbMessage { Id = "fb_1", Content = "Test_Message", SenderId = "user_1", ReceiverId = "user_2", SendTime = DateTime.UtcNow };
            var sender = new SocialUser { Id = 1, OriginalId = "user_1" };
            var receiver = new SocialUser { Id = 2, OriginalId = "user_2" };
            var conversation = new Conversation { OriginalId = fbChange.Value.ThreadId };
            IDependencyResolver dependencyResolver = MockDependecyResolverForTestingUpdateConversation(conversation, fbChange, fbMessage, sender, receiver).Object;
            var strategy = new NewMessageStrategy(dependencyResolver);
            strategy.UnitOfWorkManager = MockUnitOfWorkManager().Object;
            var socialAccount = new SocialAccount { Token = "token", IfConvertMessageToConversation = true, SocialUser = new SocialUser { Id = 3 } };

            // Act
            var processResult = await strategy.Process(socialAccount, fbChange);

            // Assert
            var message = processResult.UpdatedConversations.First().Messages.FirstOrDefault(t => t.OriginalId == fbMessage.Id);
            Assert.NotNull(message);
            Assert.Equal(MessageSource.FacebookMessage, message.Source);
            Assert.Equal(fbMessage.Id, message.OriginalId);
            Assert.Equal(sender.Id, message.SenderId);
            Assert.Equal(receiver.Id, message.ReceiverId);
            Assert.Equal(fbMessage.Content, message.Content);
        }

        [Fact]
        public async Task ShouldNofityNewMessage()
        {
            // Arrange
            var fbChange = new FbHookChange { Value = new FbHookChangeValue { ThreadId = "original_conversation_id" } };
            var fbMessage = new FbMessage { Id = "fb_1", Content = "Test_Message", SenderId = "user_1", ReceiverId = "user_2", SendTime = DateTime.UtcNow };
            var sender = new SocialUser { Id = 1, OriginalId = "user_1" };
            var receiver = new SocialUser { Id = 2, OriginalId = "user_2" };
            var conversation = new Conversation { OriginalId = fbChange.Value.ThreadId };
            IDependencyResolver dependencyResolver = MockDependecyResolverForTestingUpdateConversation(conversation, fbChange, fbMessage, sender, receiver).Object;
            var strategy = new NewMessageStrategy(dependencyResolver);
            strategy.UnitOfWorkManager = MockUnitOfWorkManager().Object;
            var socialAccount = new SocialAccount { Token = "token", IfConvertMessageToConversation = true, SocialUser = new SocialUser { Id = 3 } };

            // Act
            var processResult = await strategy.Process(socialAccount, fbChange);

            // Assert
            Assert.Equal(1, processResult.UpdatedConversations.Count());
            Assert.Equal(1, processResult.NewMessages.Count());
            Assert.Equal(0, processResult.NewConversations.Count());
        }

        [Fact]
        public async Task ShouldNotUpdateLastMessageIfMessageIsOlderThanExistingMessage()
        {
            // Arrange
            var fbChange = new FbHookChange { Value = new FbHookChangeValue { ThreadId = "original_conversation_id" } };
            var fbMessage = new FbMessage { Id = "fb_1", Content = "Test_Message", SenderId = "user_1", ReceiverId = "user_2", SendTime = new DateTime(1999, 1, 1, 0, 0, 0, DateTimeKind.Utc) };
            var sender = new SocialUser { Id = 1, OriginalId = "user_1" };
            var receiver = new SocialUser { Id = 2, OriginalId = "user_2" };
            var conversation = new Conversation { OriginalId = fbChange.Value.ThreadId, LastMessageSenderId = 888, LastMessageSentTime = new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc) };
            IDependencyResolver dependencyResolver = MockDependecyResolverForTestingUpdateConversation(conversation, fbChange, fbMessage, sender, receiver).Object;
            var strategy = new NewMessageStrategy(dependencyResolver);
            strategy.UnitOfWorkManager = MockUnitOfWorkManager().Object;
            var socialAccount = new SocialAccount { Token = "token", IfConvertMessageToConversation = true, SocialUser = new SocialUser { Id = 3 } };

            // Act
            var processResult = await strategy.Process(socialAccount, fbChange);

            // Assert
            Assert.Equal(888, conversation.LastMessageSenderId);
            Assert.Equal(new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc), conversation.LastMessageSentTime);
        }

        private Mock<IUnitOfWorkManager> MockUnitOfWorkManager()
        {
            var uowMock = new Mock<IUnitOfWork>();
            var uowManagerMock = new Mock<IUnitOfWorkManager>();
            uowManagerMock.Setup(t => t.Current).Returns(uowMock.Object);
            return uowManagerMock;
        }

        private Mock<IDependencyResolver> MockDependecyResolverForTestingCreateConversation(FbHookChange change, FbMessage fbMessage, SocialUser sender, SocialUser receiver)
        {
            var fbClientMock = new Mock<IFbClient>();
            fbClientMock.Setup(t => t.GetMessagesFromConversationId("token", change.Value.ThreadId, 10)).ReturnsAsync(
                new List<FbMessage>
                {
                    fbMessage
                });
            var messageServiceMock = new Mock<IMessageService>();
            messageServiceMock.Setup(t => t.FindAll()).Returns(new List<Message>().AsQueryable());

            var socialUserServiceMock = new Mock<ISocialUserService>();
            socialUserServiceMock.Setup(t => t.GetOrCreateFacebookUser("token", fbMessage.SenderId)).ReturnsAsync(sender);
            socialUserServiceMock.Setup(t => t.GetOrCreateFacebookUser("token", fbMessage.ReceiverId)).ReturnsAsync(receiver);

            var conversationServiceMock = new Mock<IConversationService>();
            // If we can't find a un closed conversation, then we will create a new conversation.
            conversationServiceMock.Setup(t => t.GetUnClosedConversation(change.Value.ThreadId)).Returns<Conversation>(null);

            var dependencyResolverMock = new Mock<IDependencyResolver>();
            dependencyResolverMock.Setup(t => t.Resolve<IFbClient>()).Returns(fbClientMock.Object);
            dependencyResolverMock.Setup(t => t.Resolve<IMessageService>()).Returns(messageServiceMock.Object);
            dependencyResolverMock.Setup(t => t.Resolve<ISocialUserService>()).Returns(socialUserServiceMock.Object);
            dependencyResolverMock.Setup(t => t.Resolve<IConversationService>()).Returns(conversationServiceMock.Object);

            return dependencyResolverMock;
        }

        private Mock<IDependencyResolver> MockDependecyResolverForTestingUpdateConversation(Conversation conversation, FbHookChange change, FbMessage fbMessage, SocialUser sender, SocialUser receiver)
        {
            var fbClientMock = new Mock<IFbClient>();
            fbClientMock.Setup(t => t.GetMessagesFromConversationId("token", change.Value.ThreadId, 10)).ReturnsAsync(
                new List<FbMessage>
                {
                    fbMessage
                });
            var messageServiceMock = new Mock<IMessageService>();
            messageServiceMock.Setup(t => t.FindAll()).Returns(new List<Message>().AsQueryable());

            var socialUserServiceMock = new Mock<ISocialUserService>();
            socialUserServiceMock.Setup(t => t.GetOrCreateFacebookUser("token", fbMessage.SenderId)).ReturnsAsync(sender);
            socialUserServiceMock.Setup(t => t.GetOrCreateFacebookUser("token", fbMessage.ReceiverId)).ReturnsAsync(receiver);

            var conversationServiceMock = new Mock<IConversationService>();
            // If we can find a un-closed conversation, then we will add the new message to un-closed conversation.
            conversationServiceMock.Setup(t => t.GetUnClosedConversation(change.Value.ThreadId)).Returns(conversation);

            var dependencyResolverMock = new Mock<IDependencyResolver>();
            dependencyResolverMock.Setup(t => t.Resolve<IFbClient>()).Returns(fbClientMock.Object);
            dependencyResolverMock.Setup(t => t.Resolve<IMessageService>()).Returns(messageServiceMock.Object);
            dependencyResolverMock.Setup(t => t.Resolve<ISocialUserService>()).Returns(socialUserServiceMock.Object);
            dependencyResolverMock.Setup(t => t.Resolve<IConversationService>()).Returns(conversationServiceMock.Object);

            return dependencyResolverMock;
        }
    }
}
