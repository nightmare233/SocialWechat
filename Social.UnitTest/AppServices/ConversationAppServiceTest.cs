//using Framework.Core;
//using Framework.Core.UnitOfWork;
//using Moq;
//using Social.Application.AppServices;
//using Social.Application.Dto;
//using Social.Domain.DomainServices;
//using Social.Domain.Entities;
//using Social.Domain.Entities.General;
//using Social.Infrastructure;
//using Social.Infrastructure.Enum;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using Xunit;

//namespace Social.UnitTest.AppServices
//{
//    public class ConversationAppServiceTest : TestBase
//    {
//        public ConversationAppServiceTest()
//        {

//        }
//        [Fact]
//        public void ShouldGetDtoById()
//        {
//            // Arrange
//            var fakeConversationEntity = MakeConversationEntity(1);
//            var conversationService = new Mock<IConversationService>();
//            conversationService.Setup(t => t.CheckIfExists(1)).Returns(fakeConversationEntity);
//            var messageService = MakeFakeMessageService(fakeConversationEntity.Messages);
//            ConversationAppService appSerice = new ConversationAppService(
//                conversationService.Object,
//                messageService,
//                new Mock<IAgentService>().Object,
//                new Mock<IDepartmentService>().Object
//                , null, null
//                //);

//            // Act
//            ConversationDto conversationDto = appSerice.Find(1);

//            // Assert
//            Assert.NotNull(conversationDto);
//            AssertDtoEqualToEntity(fakeConversationEntity, conversationDto);
//        }

//        [Fact]
//        public void ShouldGetDtoList()
//        {
//            // Arrange
//            var fakeConversationEntityList = new List<Conversation>
//            {
//                MakeConversationEntity(1),
//                MakeConversationEntity(2)
//            }.AsQueryable();
//            var conversationService = new Mock<IConversationService>();
//            conversationService.Setup(t => t.FindAll()).Returns(fakeConversationEntityList);
//            conversationService.Setup(t => t.ApplyFilter(fakeConversationEntityList, It.IsAny<int?>())).Returns(fakeConversationEntityList);
//            conversationService.Setup(t => t.ApplyKeyword(fakeConversationEntityList, It.IsAny<string>())).Returns(fakeConversationEntityList);
//            conversationService.Setup(t => t.ApplySenderOrReceiverId(fakeConversationEntityList, It.IsAny<int?>())).Returns(fakeConversationEntityList);
//            var messageService = new Mock<IMessageService>();
//            messageService.Setup(t => t.FindAllByConversationIds(It.IsAny<int[]>())).Returns(
//             new List<Message> {
//                 fakeConversationEntityList.First(t => t.Id == 1).Messages.FirstOrDefault(),
//                 fakeConversationEntityList.First(t => t.Id == 2).Messages.FirstOrDefault()
//             }.AsQueryable()
//            );
//            ConversationAppService appSerice = new ConversationAppService(
//                conversationService.Object,
//                messageService.Object,
//                FakeServices.MakeAgentService(),
//                FakeServices.MakeDepartmentService(), null, null);

//            // Act
//            IList<ConversationDto> conversationDtoList = appSerice.Find(new ConversationSearchDto { });

//            // Assert
//            Assert.True(conversationDtoList.Any());
//            AssertDtoEqualToEntity(fakeConversationEntityList.First(t => t.Id == 2), conversationDtoList[0]);
//            AssertDtoEqualToEntity(fakeConversationEntityList.First(t => t.Id == 1), conversationDtoList[1]);
//        }

//        [Fact]
//        public void ShouldInsert()
//        {
//            // Arrange
//            var fakeConversationEntity = MakeConversationEntity(1);
//            var conversationService = new Mock<IConversationService>();
//            conversationService.Setup(t => t.Insert(It.IsAny<Conversation>())).Returns(fakeConversationEntity);
//            var messageService = MakeFakeMessageService(fakeConversationEntity.Messages);
//            ConversationAppService appSerice = new ConversationAppService(
//                conversationService.Object,
//                messageService,
//                new Mock<IAgentService>().Object,
//                new Mock<IDepartmentService>().Object,
//                null, null
//                );
//            appSerice.UnitOfWorkManager = MakeFakeUnitOfWorkManager();

//            // Act
//            var dto = appSerice.Insert(new ConversationCreateDto { Subject = fakeConversationEntity.Subject });

//            // Assert
//            conversationService.Verify(t => t.Insert(It.Is<Conversation>(r => r.Subject == fakeConversationEntity.Subject)));
//            Assert.NotNull(dto);
//            AssertDtoEqualToEntity(fakeConversationEntity, dto);
//        }

//        //[Fact]
//        //public async Task ShouldUpdate()
//        //{
//        //    // Arrange
//        //    var fakeConversationEntity = MakeConversationEntity(1);
//        //    var notificationService = new Mock<INotificationManager>().Object;
//        //    var messageService = MakeFakeMessageService(fakeConversationEntity.Messages);
//        //    var conversationService = new Mock<IConversationService>();
//        //    conversationService.Setup(t => t.Find(1)).Returns(fakeConversationEntity);
//        //    ConversationAppService appSerice = new ConversationAppService(
//        //        conversationService.Object,
//        //        messageService, null, null, null,
//        //        notificationService);
//        //    appSerice.UnitOfWorkManager = MakeFakeUnitOfWorkManager();

//        //    // Act
//        //    var dto = await appSerice.UpdateAsync(1, new ConversationUpdateDto());

//        //    // Assert
//        //    conversationService.Verify(t => t.Update(It.Is<Conversation>(r => r.Id == 1)));
//        //    Assert.NotNull(dto);
//        //    AssertDtoEqualToEntity(fakeConversationEntity, dto);
//        //}

//        //[Fact]
//        //public async Task ShouldThrowExceptionIfConversationNotExistsWhenUpdate()
//        //{
//        //    // Arrange
//        //    var conversationService = new Mock<IConversationService>();
//        //    conversationService.Setup(t => t.Find(1)).Returns<Conversation>(null);
//        //    ConversationAppService appSerice = new ConversationAppService(conversationService.Object, null, null, null, null, null);

//        //    // Act
//        //    Func<Task> action = () => { return appSerice.UpdateAsync(1, new ConversationUpdateDto()); };

//        //    // Assert
//        //    await Assert.ThrowsAsync<ExceptionWithCode>(action);
//        //}

//        //[Fact]
//        //public async Task ShouldNofityConversationUpdate()
//        //{
//        //    // Arrange
//        //    var fakeConversationEntity = MakeConversationEntity(1);
//        //    var notificationService = new Mock<INotificationManager>();
//        //    var messageService = MakeFakeMessageService(fakeConversationEntity.Messages);
//        //    var conversationService = new Mock<IConversationService>();
//        //    conversationService.Setup(t => t.Find(1)).Returns(fakeConversationEntity);
//        //    ConversationAppService appSerice = new ConversationAppService(conversationService.Object, messageService, null, null, null, notificationService.Object);
//        //    appSerice.UnitOfWorkManager = MakeFakeUnitOfWorkManager();

//        //    // Act
//        //    var dto = await appSerice.UpdateAsync(1, new ConversationUpdateDto());

//        //    // Assert
//        //    notificationService.Verify(t => t.NotifyUpdateConversation(It.IsAny<int>(), 1, It.IsAny<int?>()));
//        //}

//        [Fact]
//        public void ShouldDelete()
//        {
//            // Arrange
//            var fakeConversationEntity = MakeConversationEntity(1);
//            var conversationService = new Mock<IConversationService>();
//            conversationService.Setup(t => t.CheckIfExists(1)).Returns(fakeConversationEntity);
//            ConversationAppService appSerice = new ConversationAppService(conversationService.Object, null, null, null, null, null);

//            // Act
//            appSerice.Delete(1);

//            // Assert
//            conversationService.Verify(t => t.Delete(fakeConversationEntity));
//        }

//        [Fact]
//        public void ShouldGetLogs()
//        {
//            // Arrange
//            var logService = new Mock<IDomainService<ConversationLog>>();
//            logService.Setup(t => t.FindAll()).Returns(new List<ConversationLog>
//            {
//                new ConversationLog{ConversationId = 1}
//            }.AsQueryable());
//            ConversationAppService appSerice = new ConversationAppService(null, null, null, null, logService.Object, null);

//            // Act
//            IList<ConversationLogDto> conversationLogDtos = appSerice.GetLogs(1);

//            // Assert
//            Assert.True(conversationLogDtos.Any());
//        }

//        //[Fact]
//        //public async Task ShouldTakeConversationDto()
//        //{
//        //    // Arrange
//        //    var fakeConversationEntity = MakeConversationEntity(1);
//        //    var messageService = MakeFakeMessageService(fakeConversationEntity.Messages);
//        //    var conversationService = new Mock<IConversationService>();
//        //    var agentService = new Mock<IAgentService>();
//        //    var departmentService = new Mock<IDepartmentService>();
//        //    var notificationManagerMock = new Mock<INotificationManager>();
//        //    conversationService.Setup(t => t.Take(It.IsAny<Conversation>())).Returns(new Conversation { Id = 1, AgentId = 1, DepartmentId = 1 });
//        //    agentService.Setup(t => t.Find(1)).Returns(new Agent { Id = 1, Name = "a" });
//        //    departmentService.Setup(t => t.Find(1)).Returns(new Department { Id = 1, Name = "b" });
//        //    ConversationAppService appSerice = new ConversationAppService(conversationService.Object, messageService, agentService.Object, departmentService.Object, null, notificationManagerMock.Object);
//        //    appSerice.UnitOfWorkManager = DependencyResolverBuilder.MockUnitOfWorkManager().Object;

//        //    // Act
//        //    ConversationDto conversationLogDtos = await appSerice.TakeAsync(1);

//        //    // Assert
//        //    Assert.NotNull(conversationLogDtos);
//        //    notificationManagerMock.Verify(t => t.NotifyUpdateConversation(It.IsAny<int>(), 1, It.IsAny<int?>()));
//        //}

//        //[Fact]
//        //public async Task ShouldCloseConversationDto()
//        //{
//        //    // Arrange
//        //    var fakeConversationEntity = MakeConversationEntity(1);
//        //    var messageService = MakeFakeMessageService(fakeConversationEntity.Messages);
//        //    var conversationService = new Mock<IConversationService>();
//        //    var notificationServiceMock = new Mock<INotificationManager>();
//        //    conversationService.Setup(t => t.Close(It.IsAny<Conversation>())).Returns(new Conversation { Id = 1 });
//        //    ConversationAppService appSerice = new ConversationAppService(conversationService.Object, messageService, null, null, null, notificationServiceMock.Object);
//        //    appSerice.UnitOfWorkManager = DependencyResolverBuilder.MockUnitOfWorkManager().Object;

//        //    // Act
//        //    ConversationDto conversationLogDtos = await appSerice.CloseAsync(1);

//        //    // Assert
//        //    Assert.NotNull(conversationLogDtos);
//        //    notificationServiceMock.Verify(t => t.NotifyUpdateConversation(It.IsAny<int>(), 1, It.IsAny<int?>()));
//        //}

//        //[Fact]
//        //public async Task ShouldReopenConversationDto()
//        //{
//        //    // Arrange
//        //    var fakeConversationEntity = MakeConversationEntity(1);
//        //    var messageService = MakeFakeMessageService(fakeConversationEntity.Messages);
//        //    var conversationService = new Mock<IConversationService>();
//        //    var notificationServiceMock = new Mock<INotificationManager>();
//        //    conversationService.Setup(t => t.Reopen(It.IsAny<Conversation>())).Returns(new Conversation { Id = 1 });
//        //    ConversationAppService appSerice = new ConversationAppService(conversationService.Object, messageService, null, null, null, notificationServiceMock.Object);
//        //    appSerice.UnitOfWorkManager = DependencyResolverBuilder.MockUnitOfWorkManager().Object;

//        //    // Act
//        //    ConversationDto conversationLogDtos = await appSerice.ReopenAsync(1);

//        //    // Assert
//        //    Assert.NotNull(conversationLogDtos);
//        //    notificationServiceMock.Verify(t => t.NotifyUpdateConversation(It.IsAny<int>(), 1, It.IsAny<int?>()));
//        //}

//        //[Fact]
//        //public async Task ShouldMarkAsReadConversationDto()
//        //{
//        //    // Arrange
//        //    var fakeConversationEntity = MakeConversationEntity(1);
//        //    var messageService = MakeFakeMessageService(fakeConversationEntity.Messages);
//        //    var conversationService = new Mock<IConversationService>();
//        //    var notificationServiceMock = new Mock<INotificationManager>();
//        //    conversationService.Setup(t => t.MarkAsRead(It.IsAny<Conversation>())).Returns(new Conversation { Id = 1 });
//        //    ConversationAppService appSerice = new ConversationAppService(conversationService.Object, messageService, null, null, null, notificationServiceMock.Object);
//        //    appSerice.UnitOfWorkManager = DependencyResolverBuilder.MockUnitOfWorkManager().Object;

//        //    // Act
//        //    ConversationDto conversationLogDtos = await appSerice.MarkAsReadAsync(1);

//        //    // Assert
//        //    Assert.NotNull(conversationLogDtos);
//        //    notificationServiceMock.Verify(t => t.NotifyUpdateConversation(It.IsAny<int>(), 1, It.IsAny<int?>()));
//        //}

//        //[Fact]
//        //public async Task ShouldMarkAsUnReadConversationDto()
//        //{
//        //    // Arrange
//        //    var fakeConversationEntity = MakeConversationEntity(1);
//        //    var messageService = MakeFakeMessageService(fakeConversationEntity.Messages);
//        //    var conversationService = new Mock<IConversationService>();
//        //    var notificationServiceMock = new Mock<INotificationManager>();
//        //    conversationService.Setup(t => t.MarkAsUnRead(It.IsAny<Conversation>())).Returns(new Conversation { Id = 1 });
//        //    ConversationAppService appSerice = new ConversationAppService(conversationService.Object, messageService, null, null, null, notificationServiceMock.Object);
//        //    appSerice.UnitOfWorkManager = DependencyResolverBuilder.MockUnitOfWorkManager().Object;

//        //    // Act
//        //    ConversationDto conversationLogDtos = await appSerice.MarkAsUnReadAsync(1);

//        //    // Assert
//        //    Assert.NotNull(conversationLogDtos);
//        //    notificationServiceMock.Verify(t => t.NotifyUpdateConversation(It.IsAny<int>(), 1, It.IsAny<int?>()));
//        //}

//        private IMessageService MakeFakeMessageService(IList<Message> messages)
//        {
//            var messageServiceMock = new Mock<IMessageService>();
//            messageServiceMock.Setup(t => t.FindAll()).Returns(messages.AsQueryable());
//            messageServiceMock.Setup(t => t.FindAllByConversationId(It.IsAny<int>())).Returns(messages.AsQueryable());
//            messageServiceMock.Setup(t => t.FindAllByConversationIds(It.IsAny<int[]>())).Returns(messages.AsQueryable());
//            return messageServiceMock.Object;
//        }

//        private void AssertDtoEqualToEntity(Conversation entity, ConversationDto dto)
//        {
//            Assert.NotNull(dto);
//            Assert.Equal(entity.Id, dto.Id);
//            Assert.Equal(entity.Subject, dto.Subject);
//            Assert.Equal(entity.Note, dto.Note);
//            Assert.Equal(entity.Status, dto.Status);
//            Assert.Equal(entity.OriginalId, dto.OriginalId);
//            Assert.Equal(entity.AgentId, dto.AgentId);
//            Assert.Equal(entity.DepartmentId, dto.DepartmentId);
//            Assert.Equal(entity.IfRead, dto.IfRead);
//            Assert.Equal(entity.LastMessageSenderId, dto.LastMessageSenderId);
//            Assert.Equal(entity.LastMessageSender.Name, dto.LastMessageSenderName);
//            Assert.Equal(entity.LastMessageSender.Avatar, dto.LastMessageSenderAvatar);
//            Assert.Equal(entity.LastMessageSentTime, dto.LastMessageSentTime);
//            Assert.Equal(entity.Priority, dto.Priority);
//            Assert.Equal(entity.Messages.First().Content, dto.LastMessage);
//            Assert.Equal(entity.Messages.First().Sender.Id, dto.LastIntegrationAccountId);
//        }

//        public Conversation MakeConversationEntity(int id)
//        {
//            return new Conversation
//            {
//                Id = id,
//                Subject = "Test Conversation Subject",
//                Note = "Test Conversation Note",
//                Status = ConversationStatus.New,
//                OriginalId = new Guid().ToString(),
//                AgentId = 1,
//                DepartmentId = 1,
//                IfRead = true,
//                Messages = new List<Message>
//                {
//                    new Message{
//                        ConversationId =id,
//                        Content ="Test Message Content",
//                        Sender =new SocialUser{
//                            Id =1,
//                            Type =SocialUserType.IntegrationAccount
//                        }
//                    }
//                },
//                LastMessageSenderId = 1,
//                LastMessageSentTime = DateTime.UtcNow,
//                LastMessageSender = new SocialUser
//                {
//                    Id = 1,
//                    Name = "Test User",
//                    Avatar = "http://test"
//                },
//                Priority = ConversationPriority.High,
//                CreatedTime = DateTime.UtcNow
//            };
//        }
//    }
//}
