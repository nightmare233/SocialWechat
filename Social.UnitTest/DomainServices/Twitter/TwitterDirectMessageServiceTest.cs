using Moq;
using Social.Domain.DomainServices;
using Social.Domain.DomainServices.Twitter;
using Social.Domain.Entities;
using Social.Infrastructure.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tweetinvi.Models;
using Xunit;

namespace Social.UnitTest.DomainServices.Twitter
{
    public class TwitterDirectMessageServiceTest
    {
        [Fact]
        public async Task ShouldCreateConversationForDirectMessage()
        {
            // Arrange
            SocialAccount socialAccount = MakeSocialAccount();
            IMessage testMessage = MakeTestMessage(socialAccount);

            var socialUserServiceMock = new Mock<ISocialUserService>();
            socialUserServiceMock
                .Setup(t => t.GetOrCreateTwitterUser(testMessage.Sender))
                .ReturnsAsync(new SocialUser { Id = 1 });
            socialUserServiceMock
                .Setup(t => t.GetOrCreateTwitterUser(testMessage.Recipient))
                .ReturnsAsync(socialAccount.SocialUser);

            var dependencyResolver = new DependencyResolverBuilder()
                .WithSocialUserService(socialUserServiceMock.Object)
                .Build();
            var service = dependencyResolver.Resolve<TwitterDirectMessageService>();
            service.UnitOfWorkManager = DependencyResolverBuilder.MockUnitOfWorkManager().Object;

            // Act
            var processResult = await service.ProcessDirectMessage(socialAccount, testMessage);

            // Assert
            var conversatoin = processResult.NewConversations.First();
            Assert.NotNull(conversatoin);
            Assert.Equal(testMessage.Id.ToString(), conversatoin.OriginalId);
            Assert.Equal(ConversationSource.TwitterDirectMessage, conversatoin.Source);
            Assert.Equal(ConversationStatus.New, conversatoin.Status);
            Assert.Equal(ConversationPriority.Normal, conversatoin.Priority);
            Assert.Equal(testMessage.Text, conversatoin.Subject);
            Assert.Equal(testMessage.SenderId, conversatoin.LastMessageSenderId);
            Assert.Equal(testMessage.CreatedAt, conversatoin.LastMessageSentTime);
            var message = conversatoin.Messages.First();
            Assert.NotNull(message);
            Assert.Equal(testMessage.Text, message.Content);
            Assert.Equal(MessageSource.TwitterDirectMessage, message.Source);
            Assert.Equal(1, message.SenderId);
            Assert.Equal(socialAccount.Id, message.ReceiverId);
        }

        [Fact]
        public async Task ShouldNotCreateConversationIfNotAllowedConvertMessageToConversation()
        {
            // Arrange
            SocialAccount socialAccount = MakeSocialAccount();
            socialAccount.IfConvertMessageToConversation = false;
            IMessage testMessage = MakeTestMessage(socialAccount);

            var socialUserServiceMock = new Mock<ISocialUserService>();
            socialUserServiceMock
                .Setup(t => t.GetOrCreateTwitterUser(testMessage.Sender))
                .ReturnsAsync(new SocialUser { Id = 1 });
            socialUserServiceMock
                .Setup(t => t.GetOrCreateTwitterUser(testMessage.Recipient))
                .ReturnsAsync(socialAccount.SocialUser);

            var dependencyResolver = new DependencyResolverBuilder()
                .WithSocialUserService(socialUserServiceMock.Object)
                .Build();
            var service = dependencyResolver.Resolve<TwitterDirectMessageService>();
            service.UnitOfWorkManager = DependencyResolverBuilder.MockUnitOfWorkManager().Object;

            // Act
            var processResult = await service.ProcessDirectMessage(socialAccount, testMessage);

            // Assert
            Assert.Equal(0, processResult.NewConversations.Count());
        }

        [Fact]
        public async Task ShouldNotCreateConversationIfDuplicatedMessageHaveBeenFound()
        {
            // Arrange
            SocialAccount socialAccount = MakeSocialAccount();
            IMessage testMessage = MakeTestMessage(socialAccount);

            var socialUserServiceMock = new Mock<ISocialUserService>();
            socialUserServiceMock
                .Setup(t => t.GetOrCreateTwitterUser(testMessage.Sender))
                .ReturnsAsync(new SocialUser { Id = 1 });
            socialUserServiceMock
                .Setup(t => t.GetOrCreateTwitterUser(testMessage.Recipient))
                .ReturnsAsync(socialAccount.SocialUser);

            var messageServiceMock = new Mock<IMessageService>();
            messageServiceMock
                .Setup(t => t.IsDuplicatedMessage(
                    MessageSource.TwitterDirectMessage, testMessage.Id.ToString()))
                .Returns(true);

            var dependencyResolver = new DependencyResolverBuilder()
                .WithMessageService(messageServiceMock.Object)
                .WithSocialUserService(socialUserServiceMock.Object)
                .Build();
            var service = dependencyResolver.Resolve<TwitterDirectMessageService>();
            service.UnitOfWorkManager = DependencyResolverBuilder.MockUnitOfWorkManager().Object;

            // Act
            var processResult = await service.ProcessDirectMessage(socialAccount, testMessage);

            // Assert
            Assert.Equal(0, processResult.NewConversations.Count());
        }

        [Fact]
        public async Task ShouldAddMessageToExistingConversation()
        {
            // Arrange
            SocialAccount socialAccount = MakeSocialAccount();
            IMessage testMessage = MakeTestMessage(socialAccount);

            var socialUserServiceMock = new Mock<ISocialUserService>();
            socialUserServiceMock
                .Setup(t => t.GetOrCreateTwitterUser(testMessage.Sender))
                .ReturnsAsync(new SocialUser { Id = 1 });
            socialUserServiceMock
                .Setup(t => t.GetOrCreateTwitterUser(testMessage.Recipient))
                .ReturnsAsync(socialAccount.SocialUser);

            var existingConversation = new Conversation { Id = 10 };
            var conversationServiceMock = new Mock<IConversationService>();
            conversationServiceMock.Setup(t => t.GetTwitterDirectMessageConversation(
                It.Is<SocialUser>(r => r.Id == 1),
                It.Is<SocialUser>(r => r.Id == socialAccount.Id)
                )).Returns(existingConversation);

            var dependencyResolver = new DependencyResolverBuilder()
                .WithConversationService(conversationServiceMock.Object)
                .WithSocialUserService(socialUserServiceMock.Object)
                .Build();
            var service = dependencyResolver.Resolve<TwitterDirectMessageService>();
            service.UnitOfWorkManager = DependencyResolverBuilder.MockUnitOfWorkManager().Object;

            // Act
            var processResult = await service.ProcessDirectMessage(socialAccount, testMessage);

            // Assert
            conversationServiceMock.Verify(t => t.Update(existingConversation));
            var message = processResult.NewMessages.First();
            Assert.Equal(testMessage.Text, message.Content);
            Assert.Equal(existingConversation.Id, message.ConversationId);
            Assert.Equal(MessageSource.TwitterDirectMessage, message.Source);
            Assert.Equal(1, message.SenderId);
            Assert.Equal(socialAccount.Id, message.ReceiverId);
        }

        protected IMessage MakeTestMessage(SocialAccount account)
        {
            var messageMock = new Mock<IMessage>();
            messageMock.SetupGet(t => t.Id).Returns(1000);
            messageMock.SetupGet(t => t.CreatedAt).Returns(DateTime.UtcNow);
            messageMock.SetupGet(t => t.Text).Returns("This is a test message.");
            messageMock.SetupGet(t => t.SenderId).Returns(1);
            messageMock.SetupGet(t => t.Sender).Returns(MakeTestUser(1, "test_sender"));
            messageMock.SetupGet(t => t.RecipientId).Returns(account.Id);
            messageMock.SetupGet(t => t.Recipient).Returns(MakeTestUser(account.Id, account.SocialUser.Name));
            return messageMock.Object;
        }

        protected IUser MakeTestUser(long id, string name)
        {
            var userMock = new Mock<IUser>();
            userMock.Setup(t => t.Id).Returns(id);
            userMock.Setup(t => t.Name).Returns(name);
            return userMock.Object;
        }

        protected SocialAccount MakeSocialAccount()
        {
            return new SocialAccount
            {
                Id = 888,
                Token = "test_token",
                SocialUser = new SocialUser
                {
                    Id = 888,
                    Source = SocialUserSource.Twitter,
                    Type = SocialUserType.IntegrationAccount,
                    OriginalId = "test_twitter+_user_id"
                },
                IfConvertMessageToConversation = true
            };
        }
    }
}
