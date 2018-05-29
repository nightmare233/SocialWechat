using Framework.Core;
using Framework.Core.UnitOfWork;
using Moq;
using Social.Domain.DomainServices;
using Social.Domain.DomainServices.Facebook;
using Social.Domain.Entities;
using Social.Infrastructure;
using Social.Infrastructure.Enum;
using Social.Infrastructure.Facebook;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using Xunit;

namespace Social.UnitTest.DomainServices.Facebook
{
    public class PullJobServiceTest_PullTaggedVisitorPost : PullJobServiceTestBase
    {
        [Fact]
        public async Task ShouldPullTaggedVisitorPostFromFacebook()
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
            FacebookProcessResult processResult = await pullJobService.PullTaggedVisitorPosts(account);

            // Assert
            fbClientMock.Verify(t => t.GetTaggedVisitorPosts(account.SocialUser.OriginalId, account.Token));
        }

        [Fact]
        public async Task ShouldCreateConversationForSinglePost()
        {
            // Arrange
            var account = MakeSocialAccount();
            var conversationServiceMock = new Mock<IConversationService>();
            var messageService = new Mock<IMessageService>().Object;
            var socialUserServiceMock = MockSocialUserService(account);
            var fbClientMock = MockFbClient(account.SocialUser.OriginalId, account.Token, MakeSinglePostPagingData());

            var dependencyResolver = new DependencyResolverBuilder()
                .WithConversationService(conversationServiceMock.Object)
                .WithSocialUserService(socialUserServiceMock.Object)
                .WithFacebookClient(fbClientMock.Object)
                .Build();

            var pullJobService = dependencyResolver.Resolve<PullJobService>();


            // Act
            FacebookProcessResult processResult = await pullJobService.PullTaggedVisitorPosts(account);

            // Assert
            var conversation = processResult.NewConversations.First();
            Assert.NotNull(conversation);
            conversationServiceMock.Verify(t => t.InsertAsync(conversation));
            Assert.Equal("post_1", conversation.OriginalId);
            Assert.Equal(1, conversation.LastMessageSenderId);
            Assert.Equal(new DateTime(2000, 1, 1, 1, 1, 1, DateTimeKind.Utc), conversation.LastMessageSentTime);
            Assert.Equal(ConversationSource.FacebookVisitorPost, conversation.Source);
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
        public async Task ShouldIngoreDuplicatePost()
        {
            // Arrange
            var account = MakeSocialAccount();
            var conversationServiceMock = new Mock<IConversationService>();
            // make duplicate data
            var duplicateConversations = new List<Conversation>
            {
                new Conversation{
                    Source = ConversationSource.FacebookVisitorPost,
                    OriginalId ="post_1"
                }
            };
            conversationServiceMock.Setup(t => t.FindAll()).Returns(duplicateConversations.AsQueryable());

            var socialUserServiceMock = MockSocialUserService(account);
            var fbClientMock = MockFbClient(account.SocialUser.OriginalId, account.Token, MakeSinglePostPagingData());
            var dependencyResolver = new DependencyResolverBuilder()
                .WithConversationService(conversationServiceMock.Object)
                .WithSocialUserService(socialUserServiceMock.Object)
                .WithFacebookClient(fbClientMock.Object)
                .Build();

            var pullJobService = dependencyResolver.Resolve<PullJobService>();


            // Act
            FacebookProcessResult processResult = await pullJobService.PullTaggedVisitorPosts(account);

            // Assert
            Assert.Equal(0, processResult.NewConversations.Count());
        }

        [Fact]
        public async Task ShouldSetDefaultAssigneeWhenCreatePost()
        {
            // Arrange
            var account = MakeSocialAccount();
            account.ConversationPriority = ConversationPriority.High;
            account.ConversationDepartmentId = 10;
            account.ConversationAgentId = 100;
            var socialUserServiceMock = MockSocialUserService(account);
            var fbClientMock = MockFbClient(account.SocialUser.OriginalId, account.Token, MakeSinglePostPagingData());
            var dependencyResolver = new DependencyResolverBuilder()
                .WithSocialUserService(socialUserServiceMock.Object)
                .WithFacebookClient(fbClientMock.Object)
                .Build();

            var pullJobService = dependencyResolver.Resolve<PullJobService>();


            // Act
            FacebookProcessResult processResult = await pullJobService.PullTaggedVisitorPosts(account);

            // Assert
            var conversation = processResult.NewConversations.First();
            Assert.Equal(10, conversation.DepartmentId);
            Assert.Equal(100, conversation.AgentId);
            Assert.Equal(ConversationPriority.High, conversation.Priority);
        }

        [Fact]
        public async Task ShouldSetRecipientWhenCreatePost()
        {
            // Arrange
            var account = MakeSocialAccount();
            var socialUserServiceMock = MockSocialUserService(account);
            var allAccounts = new List<SocialUser> { account.SocialUser };
            socialUserServiceMock.Setup(t => t.FindAll()).Returns(allAccounts.AsQueryable());

            var postPagingData = MakeSinglePostPagingData();
            postPagingData.data.First().to = new FbData<FbUser>()
            {
                data = new List<FbUser>
                {
                  new FbUser{id=account.SocialUser.OriginalId}
                }
            };
            var fbClientMock = MockFbClient(account.SocialUser.OriginalId, account.Token, postPagingData);
            var dependencyResolver = new DependencyResolverBuilder()
                .WithSocialUserService(socialUserServiceMock.Object)
                .WithFacebookClient(fbClientMock.Object)
                .Build();

            var pullJobService = dependencyResolver.Resolve<PullJobService>();


            // Act
            FacebookProcessResult processResult = await pullJobService.PullTaggedVisitorPosts(account);

            // Assert
            var message = processResult.NewConversations.First().Messages.First();
            Assert.Equal(account.Id, message.ReceiverId);
        }

        [Fact]
        public async Task ShouldIgnoreIfRecipientIsOtherIntegrationAccountWhenCreatePost()
        {
            // Arrange
            var account = MakeSocialAccount();
            var socialUserServiceMock = MockSocialUserService(account);
            var allAccounts = new List<SocialUser> {
                account.SocialUser,
                new SocialUser
                {
                    Id = 999,
                    Source = SocialUserSource.Facebook,
                    Type = SocialUserType.IntegrationAccount,
                    OriginalId = "test_page_id2"
                }
            };
            socialUserServiceMock.Setup(t => t.FindAll()).Returns(allAccounts.AsQueryable());
            var postPagingData = MakeSinglePostPagingData();
            postPagingData.data.First().to = new FbData<FbUser>
            {
                data = new List<FbUser>
                {
                    new FbUser{id="test_page_id2"}
                }
            };
            var fbClientMock = MockFbClient(account.SocialUser.OriginalId, account.Token, postPagingData);
            var dependencyResolver = new DependencyResolverBuilder()
                .WithSocialUserService(socialUserServiceMock.Object)
                .WithFacebookClient(fbClientMock.Object)
                .Build();

            var pullJobService = dependencyResolver.Resolve<PullJobService>();


            // Act
            FacebookProcessResult processResult = await pullJobService.PullTaggedVisitorPosts(account);

            // Assert
            Assert.Equal(0, processResult.NewConversations.Count());
        }

        [Fact]
        public async Task ShouldCreateConversationsForMultiplePost()
        {
            // Arrange
            var account = MakeSocialAccount();
            var conversationServiceMock = new Mock<IConversationService>();
            var socialUserServiceMock = MockSocialUserService(account);
            var testDataFromFb = new FbPagingData<FbPost>
            {
                data = new List<FbPost>
                {
                    new FbPost{
                        id ="post_1",
                        from = new FbUser{id="user_1"},
                        created_time =new DateTime(2000,1,1,1,1,1,DateTimeKind.Utc),
                        message="test_content 1"
                        },
                    new FbPost{
                        id ="post_2",
                        from = new FbUser{id="user_1"},
                        created_time =new DateTime(2001,1,1,1,1,1,DateTimeKind.Utc),
                        message="test_content 2"
                        },
                }
            };
            var fbClientMock = MockFbClient(account.SocialUser.OriginalId, account.Token, testDataFromFb);
            var dependencyResolver = new DependencyResolverBuilder()
                .WithConversationService(conversationServiceMock.Object)
                .WithSocialUserService(socialUserServiceMock.Object)
                .WithFacebookClient(fbClientMock.Object)
                .Build();

            var pullJobService = dependencyResolver.Resolve<PullJobService>();

            // Act
            FacebookProcessResult processResult = await pullJobService.PullTaggedVisitorPosts(account);

            // Assert
            conversationServiceMock.Verify(t => t.InsertAsync(It.IsAny<Conversation>()), Times.Exactly(2));
        }

        [Fact]
        public async Task ShouldCreateConversationForNextPageDataFromFacebook()
        {
            // Arrange
            var account = MakeSocialAccount();
            var conversationServiceMock = new Mock<IConversationService>();
            var socialUserServiceMock = MockSocialUserService(account);
            var firstPageData = new FbPagingData<FbPost>
            {
                data = new List<FbPost>
                {
                    new FbPost{
                        id ="post_1",
                        from = new FbUser{id="user_1"},
                        created_time =new DateTime(2000,1,1,1,1,1,DateTimeKind.Utc),
                        message="test_content 1"
                        }
                },
                paging = new FbPaging
                {
                    next = "http://api.facebook.com/122312"
                }
            };
            var secondPageData = new FbPagingData<FbPost>
            {
                data = new List<FbPost>
                {
                    new FbPost{
                        id ="post_2",
                        from = new FbUser{id="user_1"},
                        created_time =new DateTime(2001,1,1,1,1,1,DateTimeKind.Utc),
                        message="test_content 2"
                        }
                }
            };
            var fbClientMock = MockFbClient(account.SocialUser.OriginalId, account.Token, firstPageData);

            fbClientMock.Setup(t => t.GetPagingData<FbPost>("http://api.facebook.com/122312"))
                .ReturnsAsync(secondPageData);

            var dependencyResolver = new DependencyResolverBuilder()
                .WithConversationService(conversationServiceMock.Object)
                .WithSocialUserService(socialUserServiceMock.Object)
                .WithFacebookClient(fbClientMock.Object)
                .Build();

            var pullJobService = dependencyResolver.Resolve<PullJobService>();

            // Act
            FacebookProcessResult processResult = await pullJobService.PullTaggedVisitorPosts(account);

            // Assert
            conversationServiceMock.Verify(t => t.InsertAsync(It.IsAny<Conversation>()), Times.Exactly(2));
        }

        [Fact]
        public async Task ShouldCreateMessageForComments()
        {
            // Arrange
            var account = MakeSocialAccount();
            var conversationServiceMock = new Mock<IConversationService>();
            var existingConversations = new List<Conversation>();
            existingConversations.Add(new Conversation { Id = 1, OriginalId = "post_1" });
            conversationServiceMock.Setup(t => t.FindAll()).Returns(existingConversations.AsQueryable());
            var messageServiceMock = new Mock<IMessageService>();
            var existingMessages = new List<Message>();
            existingMessages.Add(new Message { Id = 1, ConversationId = 1, OriginalId = "post_1", SenderId = 777 });
            messageServiceMock.Setup(t => t.FindAll()).Returns(existingMessages.AsQueryable());

            var socialUserServiceMock = MockSocialUserService(account);
            var postWithComments = MakePostWithCommentsPaingData();
            var fbClientMock = MockFbClient(account.SocialUser.OriginalId, account.Token, postWithComments);

            var dependencyResolver = new DependencyResolverBuilder()
                .WithConversationService(conversationServiceMock.Object)
                .WithMessageService(messageServiceMock.Object)
                .WithSocialUserService(socialUserServiceMock.Object)
                .WithFacebookClient(fbClientMock.Object)
                .Build();

            var pullJobService = dependencyResolver.Resolve<PullJobService>();

            // Act
            FacebookProcessResult processResult = await pullJobService.PullTaggedVisitorPosts(account);

            // Assert
            Assert.Equal(1, processResult.NewMessages.Count());
            var message = processResult.NewMessages.First();
            Assert.Equal("test_comment_content1", message.Content);
            Assert.Equal(MessageSource.FacebookPostComment, message.Source);
            Assert.Equal(1, message.SenderId);
            Assert.Equal(1, message.ConversationId);
            Assert.Equal(777, message.ReceiverId);
            Assert.Equal(new DateTime(2000, 1, 2, 1, 1, 1, DateTimeKind.Utc), message.SendTime);
        }

        [Fact]
        public async Task ShouldNotCreateMessageForCommentsIfConversationNotExists()
        {
            // Arrange
            var account = MakeSocialAccount();
            var conversationServiceMock = new Mock<IConversationService>();
            var existingConversations = new List<Conversation>(); // to make sure conversation not found
            conversationServiceMock.Setup(t => t.FindAll()).Returns(existingConversations.AsQueryable());
            var messageServiceMock = new Mock<IMessageService>();
            var existingMessages = new List<Message>();
            existingMessages.Add(new Message { Id = 1, ConversationId = 1, OriginalId = "post_1", SenderId = 777 });
            messageServiceMock.Setup(t => t.FindAll()).Returns(existingMessages.AsQueryable());

            var socialUserServiceMock = MockSocialUserService(account);
            var postWithComments = MakePostWithCommentsPaingData();
            var fbClientMock = MockFbClient(account.SocialUser.OriginalId, account.Token, postWithComments);

            var dependencyResolver = new DependencyResolverBuilder()
                .WithConversationService(conversationServiceMock.Object)
                .WithMessageService(messageServiceMock.Object)
                .WithSocialUserService(socialUserServiceMock.Object)
                .WithFacebookClient(fbClientMock.Object)
                .Build();

            var pullJobService = dependencyResolver.Resolve<PullJobService>();

            // Act
            FacebookProcessResult processResult = await pullJobService.PullTaggedVisitorPosts(account);

            // Assert
            Assert.Equal(0, processResult.NewMessages.Count());
        }

        [Fact]
        public async Task ShouldNotCreateMessageForCommentsIfParentMessageNotExists()
        {
            // Arrange
            var account = MakeSocialAccount();
            var conversationServiceMock = new Mock<IConversationService>();
            var existingConversations = new List<Conversation>();
            existingConversations.Add(new Conversation { Id = 1, OriginalId = "post_1" });
            conversationServiceMock.Setup(t => t.FindAll()).Returns(existingConversations.AsQueryable());
            var messageServiceMock = new Mock<IMessageService>();
            var existingMessages = new List<Message>();// make sure parent message not exists.
            messageServiceMock.Setup(t => t.FindAll()).Returns(existingMessages.AsQueryable());

            var socialUserServiceMock = MockSocialUserService(account);
            var postWithComments = MakePostWithCommentsPaingData();
            var fbClientMock = MockFbClient(account.SocialUser.OriginalId, account.Token, postWithComments);

            var dependencyResolver = new DependencyResolverBuilder()
                .WithConversationService(conversationServiceMock.Object)
                .WithMessageService(messageServiceMock.Object)
                .WithSocialUserService(socialUserServiceMock.Object)
                .WithFacebookClient(fbClientMock.Object)
                .Build();

            var pullJobService = dependencyResolver.Resolve<PullJobService>();

            // Act
            FacebookProcessResult processResult = await pullJobService.PullTaggedVisitorPosts(account);

            // Assert
            Assert.Equal(0, processResult.NewMessages.Count());
        }

        [Fact]
        public async Task ShouldCreateMessageForNexPageComments()
        {
            // Arrange
            var account = MakeSocialAccount();
            var conversationServiceMock = new Mock<IConversationService>();
            var existingConversations = new List<Conversation>();
            existingConversations.Add(new Conversation { Id = 1, OriginalId = "post_1" });
            conversationServiceMock.Setup(t => t.FindAll()).Returns(existingConversations.AsQueryable());
            var messageServiceMock = new Mock<IMessageService>();
            var existingMessages = new List<Message>();
            existingMessages.Add(new Message { Id = 1, ConversationId = 1, OriginalId = "post_1", SenderId = 777 });
            messageServiceMock.Setup(t => t.FindAll()).Returns(existingMessages.AsQueryable());

            var socialUserServiceMock = MockSocialUserService(account);
            var postWithComments = MakePostWithCommentsPaingData();
            postWithComments.data.First().comments.paging =
                new FbPaging { next = "http://api.facebook.com/next-page-comment/121123" };
            var fbClientMock = MockFbClient(account.SocialUser.OriginalId, account.Token, postWithComments);

            var dependencyResolver = new DependencyResolverBuilder()
                .WithConversationService(conversationServiceMock.Object)
                .WithMessageService(messageServiceMock.Object)
                .WithSocialUserService(socialUserServiceMock.Object)
                .WithFacebookClient(fbClientMock.Object)
                .Build();

            var pullJobService = dependencyResolver.Resolve<PullJobService>();

            // Act
            FacebookProcessResult processResult = await pullJobService.PullTaggedVisitorPosts(account);

            // Assert
            fbClientMock.Verify(t => t.GetPagingData<FbComment>("http://api.facebook.com/next-page-comment/121123"));
        }

        [Fact]
        public async Task ShouldCreateMessageForReplyComments()
        {
            // Arrange
            var account = MakeSocialAccount();
            var conversationServiceMock = new Mock<IConversationService>();
            var existingConversations = new List<Conversation>();
            existingConversations.Add(new Conversation { Id = 1, OriginalId = "post_1" });
            conversationServiceMock.Setup(t => t.FindAll()).Returns(existingConversations.AsQueryable());
            var messageServiceMock = new Mock<IMessageService>();
            var existingMessages = new List<Message>();
            existingMessages.Add(new Message { Id = 1, ConversationId = 1, OriginalId = "post_1", SenderId = 777, Source = MessageSource.FacebookPost });
            existingMessages.Add(new Message { Id = 2, ConversationId = 1, OriginalId = "comment_1", SenderId = 777, ParentId = 1, Source = MessageSource.FacebookPostComment });
            messageServiceMock.Setup(t => t.FindAll()).Returns(existingMessages.AsQueryable());

            var socialUserServiceMock = MockSocialUserService(account);
            var postWithReplyComments = MakePostWithReplyCommentsPaingData();
            var fbClientMock = MockFbClient(account.SocialUser.OriginalId, account.Token, postWithReplyComments);

            var dependencyResolver = new DependencyResolverBuilder()
                .WithConversationService(conversationServiceMock.Object)
                .WithMessageService(messageServiceMock.Object)
                .WithSocialUserService(socialUserServiceMock.Object)
                .WithFacebookClient(fbClientMock.Object)
                .Build();

            var pullJobService = dependencyResolver.Resolve<PullJobService>();

            // Act
            FacebookProcessResult processResult = await pullJobService.PullTaggedVisitorPosts(account);

            // Assert
            Assert.Equal(1, processResult.NewMessages.Count());
            var message = processResult.NewMessages.First();
            Assert.Equal("test_reply_comment_content1", message.Content);
            Assert.Equal(MessageSource.FacebookPostReplyComment, message.Source);
            Assert.Equal(1, message.SenderId);
            Assert.Equal(1, message.ConversationId);
            Assert.Equal(777, message.ReceiverId);
            Assert.Equal(new DateTime(2000, 1, 2, 1, 1, 1, DateTimeKind.Utc), message.SendTime);
        }

        [Fact]
        public async Task ShouldCreateMessageForNextPageReplyComments()
        {
            // Arrange
            var account = MakeSocialAccount();
            var conversationServiceMock = new Mock<IConversationService>();
            var existingConversations = new List<Conversation>();
            existingConversations.Add(new Conversation { Id = 1, OriginalId = "post_1" });
            conversationServiceMock.Setup(t => t.FindAll()).Returns(existingConversations.AsQueryable());
            var messageServiceMock = new Mock<IMessageService>();
            var existingMessages = new List<Message>();
            existingMessages.Add(new Message { Id = 1, ConversationId = 1, OriginalId = "post_1", SenderId = 777, Source = MessageSource.FacebookPost });
            existingMessages.Add(new Message { Id = 2, ConversationId = 1, OriginalId = "comment_1", SenderId = 777, ParentId = 1, Source = MessageSource.FacebookPostComment });
            messageServiceMock.Setup(t => t.FindAll()).Returns(existingMessages.AsQueryable());

            var socialUserServiceMock = MockSocialUserService(account);
            var postWithReplyComments = MakePostWithReplyCommentsPaingData();
            postWithReplyComments.data.First().comments.data.First().comments.paging
                = new FbPaging { next = "http://api.facebook.com/next-page-reply-comment/121123" };
            var fbClientMock = MockFbClient(account.SocialUser.OriginalId, account.Token, postWithReplyComments);

            var dependencyResolver = new DependencyResolverBuilder()
                .WithConversationService(conversationServiceMock.Object)
                .WithMessageService(messageServiceMock.Object)
                .WithSocialUserService(socialUserServiceMock.Object)
                .WithFacebookClient(fbClientMock.Object)
                .Build();

            var pullJobService = dependencyResolver.Resolve<PullJobService>();

            // Act
            FacebookProcessResult processResult = await pullJobService.PullTaggedVisitorPosts(account);

            // Assert
            fbClientMock.Verify(t => t.GetPagingData<FbComment>("http://api.facebook.com/next-page-reply-comment/121123"));
        }

        [Fact]
        public async Task ShouldNotCreateMessageForReplyCommentsIfConversationNotExists()
        {
            // Arrange
            var account = MakeSocialAccount();
            var conversationServiceMock = new Mock<IConversationService>();
            var existingConversations = new List<Conversation>(); // make sure conversation not exists.
            conversationServiceMock.Setup(t => t.FindAll()).Returns(existingConversations.AsQueryable());
            var messageServiceMock = new Mock<IMessageService>();
            var existingMessages = new List<Message>();
            existingMessages.Add(new Message { Id = 1, ConversationId = 1, OriginalId = "post_1", SenderId = 777, Source = MessageSource.FacebookPost });
            existingMessages.Add(new Message { Id = 2, ConversationId = 1, OriginalId = "comment_1", SenderId = 777, ParentId = 1, Source = MessageSource.FacebookPostComment });
            messageServiceMock.Setup(t => t.FindAll()).Returns(existingMessages.AsQueryable());

            var socialUserServiceMock = MockSocialUserService(account);
            var postWithReplyComments = MakePostWithReplyCommentsPaingData();
            var fbClientMock = MockFbClient(account.SocialUser.OriginalId, account.Token, postWithReplyComments);

            var dependencyResolver = new DependencyResolverBuilder()
                .WithConversationService(conversationServiceMock.Object)
                .WithMessageService(messageServiceMock.Object)
                .WithSocialUserService(socialUserServiceMock.Object)
                .WithFacebookClient(fbClientMock.Object)
                .Build();

            var pullJobService = dependencyResolver.Resolve<PullJobService>();

            // Act
            FacebookProcessResult processResult = await pullJobService.PullTaggedVisitorPosts(account);

            // Assert
            Assert.Equal(0, processResult.NewMessages.Count());
        }
    }
}
