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

namespace Social.UnitTest.DomainServices.Facebook
{
    public class PullJobServiceTest_PullVisitorPostsFromFeed : PullJobServiceTestBase
    {
        [Fact]
        public async Task ShouldPullVisitorPostFromFeed()
        {
            // Arrange
            var account = MakeSocialAccount();
            var socialUserServiceMock = MockSocialUserService(account);
            var fbClientMock = MockFbClient(account.SocialUser.OriginalId, account.Token, MakeSinglePostPagingData());

            var dependencyResolver = new DependencyResolverBuilder()
                .WithSocialUserService(socialUserServiceMock.Object)
                .WithFacebookClient(fbClientMock.Object)
                .Build();

            var pullJobService = dependencyResolver.Resolve<PullJobService>();

            // Act
            FacebookProcessResult processResult = await pullJobService.PullVisitorPostsFromFeed(account);

            // Assert
            fbClientMock.Verify(t => t.GetVisitorPosts(account.SocialUser.OriginalId, account.Token));
        }

        [Fact]
        public async Task ShouldCreateConversationForWallPost()
        {
            // Arrange
            var account = MakeSocialAccount();
            var conversationServiceMock = new Mock<IConversationService>();
            var messageService = new Mock<IMessageService>().Object;
            var socialUserServiceMock = new Mock<ISocialUserService>();
            socialUserServiceMock
                .Setup(t => t.GetOrCreateSocialUsers(account.Token, It.IsAny<List<FbUser>>()))
                .ReturnsAsync(
                    new List<SocialUser> {
                        new SocialUser { Id = 1, OriginalId = account.SocialUser.OriginalId }
                    }
                );

            var wallPostData = MakeSinglePostPagingData();
            wallPostData.data.First().from = new FbUser
            {
                id = account.SocialUser.OriginalId
            };
            var fbClientMock = MockFbClient(account.SocialUser.OriginalId, account.Token, wallPostData);

            var dependencyResolver = new DependencyResolverBuilder()
                .WithConversationService(conversationServiceMock.Object)
                .WithSocialUserService(socialUserServiceMock.Object)
                .WithFacebookClient(fbClientMock.Object)
                .Build();

            var pullJobService = dependencyResolver.Resolve<PullJobService>();


            // Act
            FacebookProcessResult processResult = await pullJobService.PullVisitorPostsFromFeed(account);

            // Assert
            var conversation = processResult.NewConversations.First();
            Assert.NotNull(conversation);
            conversationServiceMock.Verify(t => t.InsertAsync(conversation));
            Assert.Equal("post_1", conversation.OriginalId);
            Assert.Equal(1, conversation.LastMessageSenderId);
            Assert.Equal(new DateTime(2000, 1, 1, 1, 1, 1, DateTimeKind.Utc), conversation.LastMessageSentTime);
            Assert.Equal(ConversationSource.FacebookWallPost, conversation.Source);
            Assert.Equal(ConversationPriority.Normal, conversation.Priority);
            Assert.Equal(ConversationStatus.New, conversation.Status);
            var message = conversation.Messages.First();
            Assert.Equal("test_content", message.Content);
            Assert.Equal(MessageSource.FacebookPost, message.Source);
            Assert.Equal(1, message.SenderId);
            Assert.Equal(888, message.ReceiverId);
            Assert.Equal(new DateTime(2000, 1, 1, 1, 1, 1, DateTimeKind.Utc), message.SendTime);
        }

        [Fact]
        public async Task ShouldNotCreateConversationIfDisabledIfConvertWallPostToConversation()
        {
            // Arrange
            var account = MakeSocialAccount();
            account.IfConvertWallPostToConversation = false;
            var conversationServiceMock = new Mock<IConversationService>();
            var messageService = new Mock<IMessageService>().Object;
            var socialUserServiceMock = new Mock<ISocialUserService>();
            socialUserServiceMock
                .Setup(t => t.GetOrCreateSocialUsers(account.Token, It.IsAny<List<FbUser>>()))
                .ReturnsAsync(
                    new List<SocialUser> {
                        new SocialUser { Id = 1, OriginalId = account.SocialUser.OriginalId }
                    }
                );

            var wallPostData = MakeSinglePostPagingData();
            wallPostData.data.First().from = new FbUser
            {
                id = account.SocialUser.OriginalId
            };
            var fbClientMock = MockFbClient(account.SocialUser.OriginalId, account.Token, wallPostData);

            var dependencyResolver = new DependencyResolverBuilder()
                .WithConversationService(conversationServiceMock.Object)
                .WithSocialUserService(socialUserServiceMock.Object)
                .WithFacebookClient(fbClientMock.Object)
                .Build();

            var pullJobService = dependencyResolver.Resolve<PullJobService>();


            // Act
            FacebookProcessResult processResult = await pullJobService.PullVisitorPostsFromFeed(account);

            // Assert
            Assert.Equal(0, processResult.NewConversations.Count());
        }
    }
}
