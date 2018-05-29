using Framework.Core;
using Moq;
using Social.Domain.DomainServices;
using Social.Domain.Entities;
using Social.Infrastructure;
using Social.Infrastructure.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Social.UnitTest.DomainServices
{
    public class ConversationServiceTest
    {
        private Mock<IAgentService> agentService = new Mock<IAgentService>();
        private Mock<IDepartmentService> departmentService = new Mock<IDepartmentService>();
        private Mock<IRepository<Filter>> filterRepo = new Mock<IRepository<Filter>>();
        private Mock<IFilterExpressionFactory> filterExpressionFactory = new Mock<IFilterExpressionFactory>();
        private Mock<IRepository<ConversationLog>> logRepo = new Mock<IRepository<ConversationLog>>();
        [Fact]
        public void CheckFindBySource()
        {
            // Arrange
            var conversationRepo = new Mock<IRepository<Conversation>>();
            conversationRepo.Setup(t => t.FindAll()).Returns(new List<Conversation>
            {
                new Conversation { Id = 1, Subject = "Conversation Test 1", IsDeleted = false,IsHidden = false,Source = ConversationSource.FacebookMessage},
                new Conversation { Id = 1, Subject = "Conversation Test 2", IsDeleted = false,IsHidden = false,Source = ConversationSource.FacebookVisitorPost}
            }.AsQueryable());

            var conversationService = new ConversationService(agentService.Object, departmentService.Object, filterRepo.Object, filterExpressionFactory.Object, logRepo.Object);
            conversationService.Repository = conversationRepo.Object;

            // Act
            var conversation = conversationService.Find(1,ConversationSource.FacebookVisitorPost);
            // Assert
            Assert.Equal("Conversation Test 2",conversation.Subject);
        }

        [Fact]
        public void CheckFindBySources()
        {
            // Arrange
            var conversationRepo = new Mock<IRepository<Conversation>>();
            conversationRepo.Setup(t => t.FindAll()).Returns(new List<Conversation>
            {
                new Conversation { Id = 1, Subject = "Conversation Test 1", IsDeleted = false,IsHidden = false,Source = ConversationSource.FacebookMessage},
                new Conversation { Id = 1, Subject = "Conversation Test 2", IsDeleted = false,IsHidden = false,Source = ConversationSource.FacebookVisitorPost}
            }.AsQueryable());

            var conversationService = new ConversationService(agentService.Object, departmentService.Object, filterRepo.Object, filterExpressionFactory.Object, logRepo.Object);
            conversationService.Repository = conversationRepo.Object;

            // Act
            var conversation = conversationService.Find(1,new ConversationSource[] { ConversationSource.FacebookVisitorPost ,ConversationSource.FacebookWallPost});
            // Assert
            Assert.Equal("Conversation Test 2", conversation.Subject);
        }

        [Fact]
        public void CheckFindAllByFilter()
        {
            // Arrange
            var conversationRepo = new Mock<IRepository<Conversation>>();
            conversationRepo.Setup(t => t.FindAll()).Returns(new List<Conversation>
            {
                new Conversation { Id = 1, Subject = "Conversation Test 1", IsDeleted = false,IfRead = false,IsHidden = false,Source = ConversationSource.FacebookMessage},
                new Conversation { Id = 1, Subject = "Conversation Test 2", IsDeleted = false,IfRead = false,IsHidden = false,Source = ConversationSource.FacebookVisitorPost}
            }.AsQueryable());
            Filter filter = new Filter {Id = 1};
            filterExpressionFactory.Setup(t => t.Create(filter)).Returns(t => t.Subject == "Conversation Test 2");
            var conversationService = new ConversationService(agentService.Object, departmentService.Object, filterRepo.Object, filterExpressionFactory.Object, logRepo.Object);
            conversationService.Repository = conversationRepo.Object;

            // Act
            var conversations = conversationService.FindAll(filter).ToList();
            // Assert
            Assert.Equal(1, conversations.Count());
        }

        [Fact]
        public void CheckGetTwitterDirectMessageConversation()
        {
            // Arrange
            var conversationRepo = new Mock<IRepository<Conversation>>();
            SocialUser sender = new SocialUser {Id = 1 };
            SocialUser recipient = new SocialUser { Id = 2};
            conversationRepo.Setup(t => t.FindAll()).Returns(new List<Conversation>
            {
                new Conversation { Id = 1, IsDeleted = false,IfRead = false,IsHidden = false,Status = ConversationStatus.OnHold,Source = ConversationSource.FacebookMessage ,Messages = new List<Message>{ new Message { ReceiverId =3,SenderId = 4} } },
                new Conversation { Id = 2, IsDeleted = false,IfRead = false,IsHidden = false,Status = ConversationStatus.OnHold,Source = ConversationSource.TwitterDirectMessage,Messages = new List<Message>{ new Message{ReceiverId = 2,SenderId = 1 } }}
            }.AsQueryable());
            
            var conversationService = new ConversationService(agentService.Object, departmentService.Object, filterRepo.Object, filterExpressionFactory.Object, logRepo.Object);
            conversationService.Repository = conversationRepo.Object;

            // Act
            var conversation = conversationService.GetTwitterDirectMessageConversation(sender, recipient);
            // Assert
            Assert.Equal(2, conversation.Id);
        }

        [Fact]
        public void CheckGetTwitterTweetConversation()
        {
            // Arrange
            var conversationRepo = new Mock<IRepository<Conversation>>();
            conversationRepo.Setup(t => t.FindAll()).Returns(new List<Conversation>
            {
                new Conversation { Id = 1, IsDeleted = false,IfRead = false,IsHidden = false,Status = ConversationStatus.OnHold,Source = ConversationSource.FacebookMessage ,Messages = new List<Message>{ new Message { OriginalId = "123" } } },
                new Conversation { Id = 2, IsDeleted = false,IfRead = false,IsHidden = false,Status = ConversationStatus.OnHold,Source = ConversationSource.TwitterTweet,Messages = new List<Message>{ new Message{OriginalId = "1234" } }}
            }.AsQueryable());

            var conversationService = new ConversationService(agentService.Object, departmentService.Object, filterRepo.Object, filterExpressionFactory.Object, logRepo.Object);
            conversationService.Repository = conversationRepo.Object;

            // Act
            var conversation = conversationService.GetTwitterTweetConversation("1234");
            // Assert
            Assert.Equal(2, conversation.Id);
        }

        [Fact]
        public void CheckIfExistsWorks()
        {
            // Arrange
            var conversationRepo = new Mock<IRepository<Conversation>>();
            conversationRepo.Setup(t => t.FindAll()).Returns(new List<Conversation> { new Conversation { Id = 1, Subject = "Conversation Test 1",IsDeleted = true } }.AsQueryable());

            var conversationService = new ConversationService(agentService.Object,departmentService.Object, filterRepo.Object, filterExpressionFactory.Object, logRepo.Object);
            conversationService.Repository = conversationRepo.Object;

            // Act
            Action action = () => { conversationService.CheckIfExists(1); };
            // Assert
            Assert.Throws<ExceptionWithCode>( action);
        }

        public void ShouldApplyKeywordWorks()
        {
            // Arrange
            var conversationService = new ConversationService(agentService.Object, departmentService.Object, filterRepo.Object, filterExpressionFactory.Object, logRepo.Object);
            var rawConversations =  new List<Conversation>
            {
                new Conversation { Id = 1, Subject = "Conversation Test 1" },
                new Conversation { Id = 2, Subject ="",Note= "Conversation2" }
            }.AsQueryable();
            // Act
            var conversations = conversationService.ApplyKeyword(rawConversations, "Conversation").DefaultIfEmpty().ToList();
            // Assert
            Assert.Equal(2, conversations.Count());
        }

        [Fact]
        public void ShouldSenderOrReceiverIdWorks()
        {
            // Arrange
            var conversationService = new ConversationService(agentService.Object, departmentService.Object, filterRepo.Object, filterExpressionFactory.Object, logRepo.Object);
            var rawConversations = new List<Conversation>
            {
                new Conversation { Id = 1, Subject = "Conversation Test 1" ,Messages =new List<Message>{ new Message { SenderId = 1 } }},
                new Conversation { Id = 2, Note = "Conversation2" ,Messages =new List<Message>{ new Message {ReceiverId = 1} }}
            }.AsQueryable();
            // Act
            var conversations = conversationService.ApplySenderOrReceiverId(rawConversations, 1).DefaultIfEmpty().ToList();
            // Assert
            Assert.Equal(2, conversations.Count());
        }

        [Fact]
        public void ShouldApplyFilterWorks()
        {
            // Arrange
             var conversationService = new ConversationService(agentService.Object, departmentService.Object, filterRepo.Object, filterExpressionFactory.Object, logRepo.Object);
            var rawConversations = new List<Conversation>
            {
                new Conversation { Id = 1, Subject = "Conversation Test 1" },
                new Conversation { Id = 2, Subject = "test2"}
            }.AsQueryable();
            Filter filter = new Filter
            {
                Id = 1,
                IfPublic = true,
                Index = 1,
                Type = FilterType.All,
                Conditions = new List<FilterCondition>
                {
                    new FilterCondition{FilterId = 1,MatchType = ConditionMatchType.Is ,Value = "Conversation"}
                }
            };
            filterExpressionFactory.Setup(t => t.Create(filter)).Returns(t => t.Subject.Contains("Conversation"));
            // Act
            var conversations = conversationService.ApplyFilter(rawConversations, filter).DefaultIfEmpty().ToList();
            // Assert
            Assert.Equal(1, conversations.Count());
        }

        [Fact]
        public void CheckGetUnClosedConversation()
        {
            // Arrange
            var conversationRepo = new Mock<IRepository<Conversation>>();
            conversationRepo.Setup(t => t.FindAll()).Returns(new List<Conversation>
            {
                new Conversation { Id = 1, IsDeleted = false,IfRead = false,IsHidden = false,Status = ConversationStatus.Closed,Source = ConversationSource.FacebookMessage , OriginalId = "1234" },
                new Conversation { Id = 2, IsDeleted = false,IfRead = false,IsHidden = false,Status = ConversationStatus.OnHold,Source = ConversationSource.TwitterTweet,OriginalId = "1234" }
            }.AsQueryable());

            var conversationService = new ConversationService(agentService.Object, departmentService.Object, filterRepo.Object, filterExpressionFactory.Object, logRepo.Object);
            conversationService.Repository = conversationRepo.Object;
           
            // Act
            var conversation = conversationService.GetUnClosedConversation("1234");
            // Assert
            Assert.Equal(2, conversation.Id);
        }
    }
}
