using Framework.Core;
using Moq;
using Social.Domain.DomainServices;
using Social.Domain.Entities;
using Social.Domain.Entities.General;
using Social.Infrastructure.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Social.UnitTest.DomainServices
{
    public class ConversationFieldServiceTest
    {
        [Fact]
        public void ShouldFillAgentOptions()
        {
            // Arrange
            var departmentServiceMock = new Mock<IDepartmentService>();
            var agentServiceMock = new Mock<IAgentService>();
            var socialUserServiceMock = new Mock<ISocialUserService>();
            var conversationFieldServiceMock = new Mock<IRepository<ConversationField>>();

            conversationFieldServiceMock.Setup(t => t.FindAll()).Returns(new List<ConversationField>
            {
                new ConversationField{ Id = 1,Name = "Agent Assignee", IfSystem = true, DataType = FieldDataType.Option}
            }.AsQueryable());
            agentServiceMock.Setup(t => t.FindAll()).Returns(new List<Agent>
            {
                new Agent { Id=1,Name="Test Agent 1"},
                new Agent { Id=2,Name="Test Agent 2"},
                new Agent { Id=3,Name="Test Agent 3"}
            });
            var conversationFieldService = new ConversationFieldService(departmentServiceMock.Object, agentServiceMock.Object, socialUserServiceMock.Object, null, null);
            conversationFieldService.Repository = conversationFieldServiceMock.Object;
            // Act
            var fields = conversationFieldService.FindAllAndFillOptions();
            // Assert
            Assert.Equal(3, fields.FirstOrDefault().Options.Count());
            Assert.Equal("Test Agent 3", fields.FirstOrDefault().Options.Where(t => t.Id == 3).FirstOrDefault().Name);
        }

        [Fact]
        public void ShouldFillDepartmentOptions()
        {
            // Arrange
            var departmentServiceMock = new Mock<IDepartmentService>();
            var agentServiceMock = new Mock<IAgentService>();
            var socialUserServiceMock = new Mock<ISocialUserService>();
            var conversationFieldServiceMock = new Mock<IRepository<ConversationField>>();

            conversationFieldServiceMock.Setup(t => t.FindAll()).Returns(new List<ConversationField>
            {
                new ConversationField{ Id = 1,Name = "Department Assignee", IfSystem = true, DataType = FieldDataType.Option}
            }.AsQueryable());
            departmentServiceMock.Setup(t => t.FindAll()).Returns(new List<Department>
            {
                new Department{Id=1,Name="Test Department 1"},
                new Department{Id=2,Name="Test Department 2"},
                new Department{Id=3,Name="Test Department 3"},
                new Department{Id=4,Name="Test Department 4"}
            });
            var conversationFieldService = new ConversationFieldService(departmentServiceMock.Object, agentServiceMock.Object, socialUserServiceMock.Object, null, null);
            conversationFieldService.Repository = conversationFieldServiceMock.Object;
            // Act
            var fields = conversationFieldService.FindAllAndFillOptions();
            // Assert
            Assert.Equal(4, fields.FirstOrDefault().Options.Count());
            Assert.Equal("Test Department 4", fields.FirstOrDefault().Options.Where(t => t.Id == 4).FirstOrDefault().Name);
        }

        [Fact]
        public void ShouldFillSocialAccountOptions()
        {
            // Arrange
            var departmentServiceMock = new Mock<IDepartmentService>();
            var agentServiceMock = new Mock<IAgentService>();
            var socialUserServiceMock = new Mock<ISocialUserService>();
            var conversationFieldServiceMock = new Mock<IRepository<ConversationField>>();

            conversationFieldServiceMock.Setup(t => t.FindAll()).Returns(new List<ConversationField>
            {
                new ConversationField{ Id = 1,Name = "Social Page/Account", IfSystem = true, DataType = FieldDataType.Option}
            }.AsQueryable());
            socialUserServiceMock.Setup(t => t.FindAll()).Returns(new List<SocialUser>
            {
                new SocialUser{Id=1,Name="Test SocialUser 1", Type = SocialUserType.IntegrationAccount},
                new SocialUser{Id=2,Name="Test SocialUser 2"},
                new SocialUser{Id=3,Name="Test SocialUser 3"},
                new SocialUser{Id=4,Name="Test SocialUser 4", Type = SocialUserType.IntegrationAccount}
            }.AsQueryable());
            var conversationFieldService = new ConversationFieldService(departmentServiceMock.Object, agentServiceMock.Object, socialUserServiceMock.Object, null, null);
            conversationFieldService.Repository = conversationFieldServiceMock.Object;
            // Act
            var fields = conversationFieldService.FindAllAndFillOptions();
            // Assert
            Assert.Equal(2, fields.FirstOrDefault().Options.Count());
            Assert.Equal("Test SocialUser 4", fields.FirstOrDefault().Options.Where(t => t.Id == 4).FirstOrDefault().Name);
        }

        [Fact]
        public void ShouldFillDateTimeOptions()
        {
            // Arrange
            var departmentServiceMock = new Mock<IDepartmentService>();
            var agentServiceMock = new Mock<IAgentService>();
            var socialUserServiceMock = new Mock<ISocialUserService>();
            var conversationFieldServiceMock = new Mock<IRepository<ConversationField>>();

            conversationFieldServiceMock.Setup(t => t.FindAll()).Returns(new List<ConversationField>
            {
                new ConversationField{ Id = 1,Name = "Date Created", IfSystem = true, DataType = FieldDataType.DateTime}
            }.AsQueryable());
            var conversationFieldService = new ConversationFieldService(departmentServiceMock.Object, agentServiceMock.Object, socialUserServiceMock.Object,null,null);
            conversationFieldService.Repository = conversationFieldServiceMock.Object;
            // Act
            var fields = conversationFieldService.FindAllAndFillOptions();
            // Assert
            Assert.Equal(5, fields.FirstOrDefault().Options.Count());
            Assert.Equal("@Today", fields.FirstOrDefault().Options.Where(t => t.Name == "Today").FirstOrDefault().Value);
        }
    }
}
