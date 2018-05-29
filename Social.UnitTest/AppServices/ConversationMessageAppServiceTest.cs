//using Moq;
//using Social.Application.AppServices;
//using Social.Application.Dto;
//using Social.Domain.DomainServices;
//using Social.Domain.Entities;
//using Social.Infrastructure;
//using Social.Infrastructure.Enum;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Xunit;

//namespace Social.UnitTest.AppServices
//{
//    public class ConversationMessageAppServiceTest : TestBase
//    {
//        [Fact]
//        public void ShouldGetFacebookDirectMessages()
//        {
//            //Arrange
//            var conversationService = new Mock<IConversationService>();
//            var agentService = new Mock<IAgentService>();
//            var messageService = new Mock<IMessageService>();

//            ConversationMessageAppService conversationMessageAppService = new ConversationMessageAppService(conversationService.Object,
//              agentService.Object, messageService.Object, null, null);

//            ConversationAppServiceTest conversationAppServiceTest = new ConversationAppServiceTest();

//            conversationService.Setup(t => t.Find(1, ConversationSource.FacebookMessage)).Returns(conversationAppServiceTest.MakeConversationEntity(1));
//            messageService.Setup(t => t.FindAllByConversationId(1)).Returns(new List<Message> { MakeMessageEntity(1, MessageSource.FacebookMessage, null, null) }.AsQueryable());

//            //Act
//            IList<FacebookMessageDto> facebookMessageDtos = conversationMessageAppService.GetFacebookDirectMessages(1);
//            //Assert
//            Assert.True(facebookMessageDtos.Any());
//            //agentService.Verify(t => t.FillAgentName(It.Is<IEnumerable<IHaveSendAgent>>(r => r.First().SendAgentId == 1)));
//            AssertDtoEqualToEntity(MakeMessageEntity(1, MessageSource.FacebookMessage, null, null), facebookMessageDtos[0]);
//        }
//        [Fact]
//        public void ShouldGetNullFacebookDirectMessages()
//        {
//            //Arrange
//            var conversationService = new Mock<IConversationService>();
//            ConversationMessageAppService conversationMessageAppService = new ConversationMessageAppService(conversationService.Object,
//              null, null, null, null);
//            conversationService.Setup(t => t.Find(1, ConversationSource.FacebookMessage)).Returns<Conversation>(null);
//            //Act
//            IList<FacebookMessageDto> facebookMessageDtos = conversationMessageAppService.GetFacebookDirectMessages(1);
//            //Assert
//            Assert.False(facebookMessageDtos.Any());
//        }

//        [Fact]
//        public void ShouldGetFacebookDirectMessage()
//        {
//            //Arrange
//            var agentService = new Mock<IAgentService>();
//            var messageService = new Mock<IMessageService>();

//            ConversationMessageAppService conversationMessageAppService = new ConversationMessageAppService(null,
//              agentService.Object, messageService.Object, null, null);

//            ConversationAppServiceTest conversationAppServiceTest = new ConversationAppServiceTest();

//            messageService.Setup(t => t.Find(1)).Returns(MakeMessageEntity(1, MessageSource.FacebookMessage, null, null));
//            agentService.Setup(t => t.GetDisplayName(1)).Returns("a");
//            //Act
//            FacebookMessageDto facebookMessageDto = conversationMessageAppService.GetFacebookDirectMessage(1);
//            //Assert
//            Assert.NotNull(facebookMessageDto);
//            Assert.Equal("a", facebookMessageDto.SendAgentName);
//            AssertDtoEqualToEntity(MakeMessageEntity(1, MessageSource.FacebookMessage, null, null), facebookMessageDto);
//        }

//        [Fact]
//        public void ShouldGetFacebookPostMessages()
//        {
//            //Arrange
//            var conversationService = new Mock<IConversationService>();
//            var agentService = new Mock<IAgentService>();
//            var messageService = new Mock<IMessageService>();

//            ConversationAppServiceTest conversationAppServiceTest = new ConversationAppServiceTest();
//            conversationService.Setup(t => t.Find(1, new[] { ConversationSource.FacebookVisitorPost, ConversationSource.FacebookWallPost })).Returns(conversationAppServiceTest.MakeConversationEntity(1));
//            messageService.Setup(t => t.FindAllByConversationId(1)).Returns(new List<Message>
//            {
//                MakeMessageEntity(1,MessageSource.FacebookPost,null,null),
//                MakeMessageEntity(2,MessageSource.FacebookPostComment,1,null),
//                MakeMessageEntity(3,MessageSource.FacebookPostReplyComment,2,null)
//            }.AsQueryable());
//            ConversationMessageAppService conversationMessageAppService = new ConversationMessageAppService(conversationService.Object,
//              agentService.Object, messageService.Object, null, null);

//            //Act
//            FacebookPostMessageDto facebookPostMessageDto = conversationMessageAppService.GetFacebookPostMessages(1);
//            //Assert
//            Assert.NotNull(facebookPostMessageDto);
//            AssertDtoEqualToEntity(MakeMessageEntity(2, MessageSource.FacebookPostComment, 1, null), facebookPostMessageDto.Comments.FirstOrDefault());
//            AssertDtoEqualToEntity(MakeMessageEntity(3, MessageSource.FacebookPostReplyComment, 2, null), facebookPostMessageDto.Comments.FirstOrDefault().ReplyComments.FirstOrDefault());
//            //agentService.Verify(t => t.FillAgentName(It.Is<IEnumerable<IHaveSendAgent>>(r => r.Any(m => m.SendAgentId == 1))));
//        }

//        [Fact]
//        public void ShouldGetFacebookPostMessagesWhenConversationIsNotFound()
//        {
//            //Arrange
//            var conversationService = new Mock<IConversationService>();
//            ConversationAppServiceTest conversationAppServiceTest = new ConversationAppServiceTest();
//            conversationService.Setup(t => t.Find(1, new[] { ConversationSource.FacebookVisitorPost, ConversationSource.FacebookWallPost })).Returns<Conversation>(null);
//            ConversationMessageAppService conversationMessageAppService = new ConversationMessageAppService(conversationService.Object,
//              null, null, null, null);

//            //Act
//            FacebookPostMessageDto facebookPostMessageDto = conversationMessageAppService.GetFacebookPostMessages(1);
//            //Assert
//            Assert.Null(facebookPostMessageDto);
//        }

//        [Fact]
//        public void ShouldGetFacebookPostMessagesWhenPostMessageIsNotFound()
//        {
//            //Arrange
//            var conversationService = new Mock<IConversationService>();
//            var messageService = new Mock<IMessageService>();

//            ConversationAppServiceTest conversationAppServiceTest = new ConversationAppServiceTest();
//            conversationService.Setup(t => t.Find(1, new[] { ConversationSource.FacebookVisitorPost, ConversationSource.FacebookWallPost })).Returns(conversationAppServiceTest.MakeConversationEntity(1));
//            messageService.Setup(t => t.FindAllByConversationId(1)).Returns(new List<Message>
//            {
//                MakeMessageEntity(2,MessageSource.FacebookPostComment,1,null),
//            }.AsQueryable());
//            ConversationMessageAppService conversationMessageAppService = new ConversationMessageAppService(conversationService.Object,
//              null, messageService.Object, null, null);

//            //Act
//            FacebookPostMessageDto facebookPostMessageDto = conversationMessageAppService.GetFacebookPostMessages(1);
//            //Assert
//            Assert.Null(facebookPostMessageDto);
//        }

//        [Fact]
//        public void ShouldGetFacebookPostCommentMessage()
//        {
//            //Arrange
//            var agentService = new Mock<IAgentService>();
//            var messageService = new Mock<IMessageService>();

//            messageService.Setup(t => t.Find(1)).Returns(MakeMessageEntity(1, MessageSource.FacebookPost, null, null));
//            agentService.Setup(t => t.GetDisplayName(1)).Returns("a");
//            ConversationMessageAppService conversationMessageAppService = new ConversationMessageAppService(null,
//              agentService.Object, messageService.Object, null, null);

//            //Act
//            FacebookPostCommentMessageDto facebookPostCommentMessageDto = conversationMessageAppService.GetFacebookPostCommentMessage(1);
//            //Assert
//            Assert.NotNull(facebookPostCommentMessageDto);
//            Assert.Equal("a", facebookPostCommentMessageDto.SendAgentName);
//            AssertDtoEqualToEntity(MakeMessageEntity(1, MessageSource.FacebookPost, null, null), facebookPostCommentMessageDto);
//        }

//        [Fact]
//        public void ShouldGetTwitterDirectMessages()
//        {
//            //Arrange
//            var conversationService = new Mock<IConversationService>();
//            var agentService = new Mock<IAgentService>();
//            var messageService = new Mock<IMessageService>();

//            ConversationAppServiceTest conversationAppServiceTest = new ConversationAppServiceTest();
//            conversationService.Setup(t => t.Find(1, ConversationSource.TwitterDirectMessage)).Returns(conversationAppServiceTest.MakeConversationEntity(1));
//            messageService.Setup(t => t.FindAllByConversationId(1)).Returns(new List<Message>
//            {
//                MakeMessageEntity(1,MessageSource.TwitterDirectMessage,null,null)
//            }.AsQueryable());
//            ConversationMessageAppService conversationMessageAppService = new ConversationMessageAppService(conversationService.Object,
//              agentService.Object, messageService.Object, null, null);

//            //Act
//            IList<TwitterDirectMessageDto> twitterDirectMessageDtos = conversationMessageAppService.GetTwitterDirectMessages(1);
//            //Assert
//            Assert.True(twitterDirectMessageDtos.Any());
//            AssertDtoEqualToEntity(MakeMessageEntity(1, MessageSource.TwitterDirectMessage, null, null), twitterDirectMessageDtos.FirstOrDefault());
//            //agentService.Verify(t => t.FillAgentName(It.Is<IEnumerable<IHaveSendAgent>>(r => r.Any(m => m.SendAgentId == 1))));
//        }

//        [Fact]
//        public void ShouldGetTwitterDirectMessagesWhenConversationIsNotFound()
//        {
//            //Arrange
//            var conversationService = new Mock<IConversationService>();
//            conversationService.Setup(t => t.Find(1, ConversationSource.TwitterDirectMessage)).Returns<Conversation>(null);

//            ConversationMessageAppService conversationMessageAppService = new ConversationMessageAppService(conversationService.Object,
//              null, null, null, null);

//            //Act
//            IList<TwitterDirectMessageDto> twitterDirectMessageDtos = conversationMessageAppService.GetTwitterDirectMessages(1);
//            //Assert
//            Assert.False(twitterDirectMessageDtos.Any());
//        }

//        [Fact]
//        public void ShouldGetTwitterDirectMessage()
//        {
//            //Arrange
//            var agentService = new Mock<IAgentService>();
//            var messageService = new Mock<IMessageService>();

//            messageService.Setup(t => t.Find(1)).Returns(MakeMessageEntity(1, MessageSource.TwitterDirectMessage, null, null));
//            agentService.Setup(t => t.GetDisplayName(1)).Returns("a");
//            ConversationMessageAppService conversationMessageAppService = new ConversationMessageAppService(null,
//              agentService.Object, messageService.Object, null, null);

//            //Act
//            TwitterDirectMessageDto twitterDirectMessageDto = conversationMessageAppService.GetTwitterDirectMessage(1);
//            //Assert
//            Assert.NotNull(twitterDirectMessageDto);
//            Assert.Equal("a", twitterDirectMessageDto.SendAgentName);
//            AssertDtoEqualToEntity(MakeMessageEntity(1, MessageSource.TwitterDirectMessage, null, null), twitterDirectMessageDto);
//        }

//        [Fact]
//        public void ShouldGetTwitterTweetMessages()
//        {
//            //Arrange
//            var conversationService = new Mock<IConversationService>();
//            var agentService = new Mock<IAgentService>();
//            var messageService = new Mock<IMessageService>();
//            var twitterService = new Mock<ITwitterService>();
//            var socialAccountService = new Mock<ISocialAccountService>();

//            ConversationAppServiceTest conversationAppServiceTest = new ConversationAppServiceTest();
//            conversationService.Setup(t => t.Find(1, ConversationSource.TwitterTweet)).Returns(conversationAppServiceTest.MakeConversationEntity(1));
//            messageService.Setup(t => t.FindAllByConversationId(1)).Returns(new List<Message>
//            {
//                MakeMessageEntity(1,MessageSource.TwitterQuoteTweet,null,"123")
//            }.AsQueryable());
//            var message = MakeMessageEntity(1, MessageSource.TwitterQuoteTweet, null, "123");
//            message.Receiver = new SocialUser { Id = 1, Type = SocialUserType.IntegrationAccount };
//            messageService.Setup(t => t.Find(1)).Returns(message);
//            var user = new SocialAccount { Id = 1 };
//            socialAccountService.Setup(t => t.Find(1)).Returns(user);
//            twitterService.Setup(t => t.GetTweetMessage(user, 123)).Returns(MakeMessageEntity(2, MessageSource.TwitterQuoteTweet, null, "123"));
//            ConversationMessageAppService conversationMessageAppService = new ConversationMessageAppService(conversationService.Object,
//              agentService.Object, messageService.Object, twitterService.Object, socialAccountService.Object);

//            //Act
//            IList<TwitterTweetMessageDto> twitterTweetMessageDtos = conversationMessageAppService.GetTwitterTweetMessages(1);
//            //Assert
//            Assert.True(twitterTweetMessageDtos.Any());
//            Assert.NotNull(twitterTweetMessageDtos.FirstOrDefault().QuoteTweet);
//            Assert.Equal(-1, twitterTweetMessageDtos.FirstOrDefault().ParentId);
//            AssertDtoEqualToEntity(MakeMessageEntity(1, MessageSource.TwitterQuoteTweet, null, "123"), twitterTweetMessageDtos.FirstOrDefault());
//            //agentService.Verify(t => t.FillAgentName(It.Is<IEnumerable<IHaveSendAgent>>(r => r.Any(m => m.SendAgentId == 1))));
//        }

//        [Fact]
//        public void ShouldGetTwitterTweetMessagesWhenConversationIsNotFound()
//        {
//            //Arrange
//            var conversationService = new Mock<IConversationService>();
//            conversationService.Setup(t => t.Find(1, ConversationSource.TwitterTweet)).Returns<Conversation>(null);
//            ConversationMessageAppService conversationMessageAppService = new ConversationMessageAppService(conversationService.Object,
//              null, null, null, null);

//            //Act
//            IList<TwitterTweetMessageDto> twitterTweetMessageDtos = conversationMessageAppService.GetTwitterTweetMessages(1);
//            //Assert
//            Assert.False(twitterTweetMessageDtos.Any());
//        }

//        [Fact]
//        public void ShouldGetTwitterTweetMessagesWhenMessageDtoWithQuoteIsNotFound()
//        {
//            //Arrange
//            var conversationService = new Mock<IConversationService>();
//            var agentService = new Mock<IAgentService>();
//            var messageService = new Mock<IMessageService>();

//            ConversationAppServiceTest conversationAppServiceTest = new ConversationAppServiceTest();
//            conversationService.Setup(t => t.Find(1, ConversationSource.TwitterTweet)).Returns(conversationAppServiceTest.MakeConversationEntity(1));
//            messageService.Setup(t => t.FindAllByConversationId(1)).Returns(new List<Message>
//            {
//                MakeMessageEntity(1,MessageSource.TwitterQuoteTweet,null,null)
//            }.AsQueryable());
//            ConversationMessageAppService conversationMessageAppService = new ConversationMessageAppService(conversationService.Object,
//              agentService.Object, messageService.Object, null, null);

//            //Act
//            IList<TwitterTweetMessageDto> twitterTweetMessageDtos = conversationMessageAppService.GetTwitterTweetMessages(1);
//            //Assert
//            Assert.True(twitterTweetMessageDtos.Any());
//            Assert.Null(twitterTweetMessageDtos.FirstOrDefault().QuoteTweet);
//            Assert.Equal(-1, twitterTweetMessageDtos.FirstOrDefault().ParentId);
//        }

//        [Fact]
//        public void ShouldGetTwitterTweetMessagesWhenSocialAccountIsNotFound()
//        {
//            //Arrange
//            var conversationService = new Mock<IConversationService>();
//            var agentService = new Mock<IAgentService>();
//            var messageService = new Mock<IMessageService>();
//            var socialAccountService = new Mock<ISocialAccountService>();

//            ConversationAppServiceTest conversationAppServiceTest = new ConversationAppServiceTest();
//            conversationService.Setup(t => t.Find(1, ConversationSource.TwitterTweet)).Returns(conversationAppServiceTest.MakeConversationEntity(1));
//            messageService.Setup(t => t.FindAllByConversationId(1)).Returns(new List<Message>
//            {
//                MakeMessageEntity(1,MessageSource.TwitterQuoteTweet,null,"123")
//            }.AsQueryable());
//            var message = MakeMessageEntity(1, MessageSource.TwitterQuoteTweet, null, "123");
//            message.Receiver = new SocialUser { Id = 1, Type = SocialUserType.IntegrationAccount };
//            messageService.Setup(t => t.Find(1)).Returns(message);
//            socialAccountService.Setup(t => t.Find(1)).Returns<SocialAccount>(null);
//            ConversationMessageAppService conversationMessageAppService = new ConversationMessageAppService(conversationService.Object,
//              agentService.Object, messageService.Object, null, socialAccountService.Object);

//            //Act
//            IList<TwitterTweetMessageDto> twitterTweetMessageDtos = conversationMessageAppService.GetTwitterTweetMessages(1);
//            //Assert
//            Assert.True(twitterTweetMessageDtos.Any());
//            Assert.Null(twitterTweetMessageDtos.FirstOrDefault().QuoteTweet);
//            Assert.Equal(-1, twitterTweetMessageDtos.FirstOrDefault().ParentId);
//        }

//        [Fact]
//        public void ShouldGetTwitterTweetMessage()
//        {
//            //Arrange
//            var agentService = new Mock<IAgentService>();
//            var messageService = new Mock<IMessageService>();

//            messageService.Setup(t => t.Find(1)).Returns(MakeMessageEntity(1, MessageSource.TwitterTypicalTweet, null, null));
//            agentService.Setup(t => t.GetDisplayName(1)).Returns("a");
//            ConversationMessageAppService conversationMessageAppService = new ConversationMessageAppService(null,
//              agentService.Object, messageService.Object, null, null);

//            //Act
//            TwitterTweetMessageDto twitterDirectMessageDto = conversationMessageAppService.GetTwitterTweetMessage(1);
//            //Assert
//            Assert.NotNull(twitterDirectMessageDto);
//            Assert.Equal("a", twitterDirectMessageDto.SendAgentName);
//            AssertDtoEqualToEntity(MakeMessageEntity(1, MessageSource.TwitterTypicalTweet, null, null), twitterDirectMessageDto);
//        }

//        [Fact]
//        public async Task ShouldReplyTwitterTweetMessage()
//        {
//            //Arrange
//            var agentService = new Mock<IAgentService>();
//            var messageService = new Mock<IMessageService>();

//            messageService.Setup(t => t.ReplyTwitterTweetMessage(1, 1, "123", false)).Returns(MakeMessageEntity(1, MessageSource.TwitterTypicalTweet, null, null));
//            agentService.Setup(t => t.GetDisplayName(1)).Returns("a");
//            ConversationMessageAppService conversationMessageAppService = new ConversationMessageAppService(null,
//              agentService.Object, messageService.Object, null, null);

//            //Act
//            TwitterTweetMessageDto twitterDirectMessageDto = await conversationMessageAppService.ReplyTwitterTweetMessage(1, 1, "123", false);
//            //Assert
//            Assert.NotNull(twitterDirectMessageDto);
//            Assert.Equal("a", twitterDirectMessageDto.SendAgentName);
//            AssertDtoEqualToEntity(MakeMessageEntity(1, MessageSource.TwitterTypicalTweet, null, null), twitterDirectMessageDto);
//        }

//        [Fact]
//        public async Task ShouldReplyTwitterDirectMessage()
//        {
//            //Arrange
//            var agentService = new Mock<IAgentService>();
//            var messageService = new Mock<IMessageService>();

//            messageService.Setup(t => t.ReplyTwitterDirectMessage(1, "123", false)).Returns(MakeMessageEntity(1, MessageSource.TwitterTypicalTweet, null, null));
//            agentService.Setup(t => t.GetDisplayName(1)).Returns("a");
//            ConversationMessageAppService conversationMessageAppService = new ConversationMessageAppService(null,
//              agentService.Object, messageService.Object, null, null);

//            //Act
//            TwitterDirectMessageDto twitterDirectMessageDto = await conversationMessageAppService.ReplyTwitterDirectMessage(1, "123", false);
//            //Assert
//            Assert.NotNull(twitterDirectMessageDto);
//            Assert.Equal("a", twitterDirectMessageDto.SendAgentName);
//            AssertDtoEqualToEntity(MakeMessageEntity(1, MessageSource.TwitterTypicalTweet, null, null), twitterDirectMessageDto);
//        }

//        [Fact]
//        public async Task ShouldReplyFacebookMessage()
//        {
//            //Arrange
//            var agentService = new Mock<IAgentService>();
//            var messageService = new Mock<IMessageService>();

//            messageService.Setup(t => t.ReplyFacebookMessage(1, "123", false)).Returns(MakeMessageEntity(1, MessageSource.FacebookMessage, null, null));
//            agentService.Setup(t => t.GetDisplayName(1)).Returns("a");
//            ConversationMessageAppService conversationMessageAppService = new ConversationMessageAppService(null,
//              agentService.Object, messageService.Object, null, null);

//            //Act
//            FacebookMessageDto twitterDirectMessageDto = await conversationMessageAppService.ReplyFacebookMessage(1, "123", false);
//            //Assert
//            Assert.NotNull(twitterDirectMessageDto);
//            Assert.Equal("a", twitterDirectMessageDto.SendAgentName);
//            AssertDtoEqualToEntity(MakeMessageEntity(1, MessageSource.FacebookMessage, null, null), twitterDirectMessageDto);
//        }

//        [Fact]
//        public async Task ShouldReplyFacebookPostOrComment()
//        {
//            //Arrange
//            var agentService = new Mock<IAgentService>();
//            var messageService = new Mock<IMessageService>();

//            messageService.Setup(t => t.ReplyFacebookPostOrComment(1, 1, "123", false)).Returns(MakeMessageEntity(1, MessageSource.FacebookPostReplyComment, null, null));
//            agentService.Setup(t => t.GetDisplayName(1)).Returns("a");
//            ConversationMessageAppService conversationMessageAppService = new ConversationMessageAppService(null,
//              agentService.Object, messageService.Object, null, null);

//            //Act
//            FacebookPostCommentMessageDto twitterDirectMessageDto = await conversationMessageAppService.ReplyFacebookPostOrComment(1, 1, "123", false);
//            //Assert
//            Assert.NotNull(twitterDirectMessageDto);
//            Assert.Equal("a", twitterDirectMessageDto.SendAgentName);
//            AssertDtoEqualToEntity(MakeMessageEntity(1, MessageSource.FacebookPostReplyComment, null, null), twitterDirectMessageDto);
//        }

//        private void AssertDtoEqualToEntity(Message entity, FacebookMessageDto dto)
//        {
//            Assert.NotNull(dto);
//            Assert.Equal(entity.Id, dto.Id);
//            Assert.Equal(entity.Source, dto.Source);
//            Assert.Equal(entity.SenderId, dto.UserId);
//            Assert.Equal(entity.SendAgentId, dto.SendAgentId);
//            Assert.Equal(entity.Source, dto.Source);
//        }

//        private void AssertDtoEqualToEntity(Message entity, FacebookPostCommentMessageDto dto)
//        {
//            Assert.NotNull(dto);
//            Assert.Equal(entity.Id, dto.Id);
//            Assert.Equal(entity.Source, dto.Source);
//            Assert.Equal(entity.SenderId, dto.UserId);
//            Assert.Equal(entity.Source, dto.Source);
//        }

//        private void AssertDtoEqualToEntity(Message entity, TwitterDirectMessageDto dto)
//        {
//            Assert.NotNull(dto);
//            Assert.Equal(entity.Id, dto.Id);
//            Assert.Equal(entity.Source, dto.Source);
//            Assert.Equal(entity.SenderId, dto.UserId);
//            Assert.Equal(entity.Source, dto.Source);
//        }

//        private void AssertDtoEqualToEntity(Message entity, TwitterTweetMessageDto dto)
//        {
//            Assert.NotNull(dto);
//            Assert.Equal(entity.Id, dto.Id);
//            Assert.Equal(entity.Source, dto.Source);
//            Assert.Equal(entity.SenderId, dto.UserId);
//            Assert.Equal(entity.Source, dto.Source);
//        }

//        private Message MakeMessageEntity(int id, MessageSource messageSource, int? parentId, string quoteTweetId)
//        {
//            return new Message
//            {
//                Id = id,
//                Source = messageSource,
//                SenderId = 1,
//                Sender = new SocialUser { Id = 1, Type = SocialUserType.Customer },
//                OriginalId = "1",
//                SendAgentId = 1,
//                ParentId = parentId,
//                QuoteTweetId = quoteTweetId
//            };
//        }

//    }
//}
