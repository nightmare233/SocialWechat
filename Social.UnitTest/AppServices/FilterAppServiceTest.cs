using Framework.Core;
using Framework.Core.UnitOfWork;
using Moq;
using Social.Application.AppServices;
using Social.Application.Dto;
using Social.Domain;
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

namespace Social.UnitTest.AppServices
{
    public class FilterAppServiceTest : TestBase
    {
        [Fact]
        public void ShouldFindAllFilterListDto()
        {
            //Arrange
            var domainService = new Mock<IFilterService>();
            var agentService = new Mock<IAgentService>();
            var notificationManager = new Mock<INotificationManager>();
            var userContext = new Mock<IUserContext>();
            userContext.Setup(t => t.UserId).Returns(1);
            Filter filter = MakeFilterEntity(1);
            domainService.Setup(t => t.FindAll()).Returns(new List<Filter>
            {
                filter
            }.AsQueryable());
            domainService.Setup(t => t.GetConversationNum(filter)).Returns(1);
            FilterAppService filterAppService = new FilterAppService(domainService.Object, agentService.Object, notificationManager.Object,null);
            filterAppService.UserContext = userContext.Object;
            //Act
            List<FilterListDto> filterListDtos = filterAppService.FindAll();
            //Assert
            Assert.True(filterListDtos.Any());
            Assert.Equal(1, filterListDtos.FirstOrDefault().ConversationNum);
            AssertDtoEqualToEntity(MakeFilterEntity(1), filterListDtos.FirstOrDefault());
            agentService.Verify(t => t.FillCreatedByName(It.Is<List<FilterListDto>>(r => r.FirstOrDefault().CreatedBy == 1)));
        }

        [Fact]
        public void ShouldFindFilterById()
        {
            //Arrange
            var domainService = new Mock<IFilterService>();
            var agentService = new Mock<IAgentService>();
            domainService.Setup(t => t.Find(1)).Returns(MakeFilterEntity(1));
            agentService.Setup(t => t.GetDisplayName(1)).Returns("a");
            FilterAppService filterAppService = new FilterAppService(domainService.Object, agentService.Object, null, null);

            //Act
            FilterDetailsDto filterDetailsDto = filterAppService.Find(1);
            //Assert
            Assert.NotNull(filterDetailsDto);
            AssertDtoEqualToEntity(MakeFilterEntity(1), filterDetailsDto);
            Assert.Equal("a", filterDetailsDto.CreatedByName);
        }

        [Fact]
        public void ShouldFindFilterByIdWhenFilterIsNotFound()
        {
            //Arrange
            var domainService = new Mock<IFilterService>();
            domainService.Setup(t => t.Find(1)).Returns<Filter>(null);
            FilterAppService filterAppService = new FilterAppService(domainService.Object, null, null, null);

            //Act
            Action action = () => filterAppService.Find(1);
            //Assert
            Assert.Throws<ExceptionWithCode>(action);
        }

        [Fact]
        public async Task ShouldInsertByCreateDto()
        {
            //Arrange
            var domainService = new Mock<IFilterService>();
            var agentService = new Mock<IAgentService>();
            var notificationManager = new Mock<INotificationManager>();
            var unitOfWorkManager = new Mock<IUnitOfWorkManager>();

            unitOfWorkManager.Setup(t => t.Current.SaveChanges());
            domainService.Setup(t => t.Insert(It.Is<Filter>(r => r.Name == "name"))).Returns(MakeFilterEntity(1));
            FilterAppService filterAppService = new FilterAppService(domainService.Object, agentService.Object, notificationManager.Object, null);
            filterAppService.UnitOfWorkManager = unitOfWorkManager.Object;
            //Act
            FilterDetailsDto filterDetailsDto = await filterAppService.Insert(MakeFilterCreateDto());
            //Assert
            AssertDtoEqualToEntity(MakeFilterEntity(1), MakeFilterCreateDto());
            AssertDtoEqualToEntity(MakeFilterEntity(1), filterDetailsDto);
            Assert.NotNull(filterDetailsDto);
            domainService.Verify(t => t.CheckFieldIdExist(It.Is<List<FilterCondition>>(r => r.FirstOrDefault().FieldId == 1)));
            domainService.Verify(t => t.CheckFieldValue(It.Is<List<FilterCondition>>(r => r.FirstOrDefault().Value == "a")));
            agentService.Verify(t => t.FillCreatedByName(It.Is<List<FilterDetailsDto>>(r => r.FirstOrDefault().CreatedBy == 1)));
            notificationManager.Verify(t => t.NotifyNewFilter(It.Is<int>(r => r == 10000), It.Is<int>(r => r == 1)));
        }

        [Fact]
        public void ShouldFindSummary()
        {
            //Arrange
            var domainService = new Mock<IFilterService>();
            var agentService = new Mock<IAgentService>();
            Filter filter = MakeFilterEntity(1);
            domainService.Setup(t => t.Find(1)).Returns(filter);
            domainService.Setup(t => t.GetConversationNum(filter)).Returns(1);
            agentService.Setup(t => t.GetDisplayName(1)).Returns("a");
            FilterAppService filterAppService = new FilterAppService(domainService.Object, agentService.Object, null, null);
            //Act
            FilterListDto filterListDto = filterAppService.FindSummary(1);
            //Assert
            Assert.NotNull(filterListDto);
            AssertDtoEqualToEntity(filter, filterListDto);
            Assert.Equal("a", filterListDto.CreatedByName);
            Assert.Equal(1, filterListDto.ConversationNum);
        }

        [Fact]
        public void ShouldFindSummaryWhenFilterIsNotFound()
        {
            //Arrange
            var domainService = new Mock<IFilterService>();
            domainService.Setup(t => t.Find(1)).Returns<Filter>(null);
            FilterAppService filterAppService = new FilterAppService(domainService.Object, null, null, null);
            //Act
            Action action = () => filterAppService.FindSummary(1);
            //Assert
            Assert.Throws<ExceptionWithCode>(action);
        }

        [Fact]
        public void ShouldHasConversation()
        {
            //Arrange
            var domainService = new Mock<IFilterService>();
            Filter filter = MakeFilterEntity(1);
            domainService.Setup(t => t.Find(1)).Returns(filter);
            domainService.Setup(t => t.HasConversation(filter, 1)).Returns(true);
            FilterAppService filterAppService = new FilterAppService(domainService.Object, null, null, null);
            //Act
            bool hasConversation = filterAppService.HasConversation(1, 1);
            //Assert
            Assert.Equal(true, hasConversation);
        }

        [Fact]
        public void ShouldHasConversationWhenFilterIsNotFound()
        {
            //Arrange
            var domainService = new Mock<IFilterService>();
            domainService.Setup(t => t.Find(1)).Returns<Filter>(null);
            FilterAppService filterAppService = new FilterAppService(domainService.Object, null, null, null);
            //Act
            Action action = () => filterAppService.HasConversation(1, 1);
            //Assert
            Assert.Throws<ExceptionWithCode>(action);
        }

        [Fact]
        public void ShouldDelete()
        {
            //Arrange
            var domainService = new Mock<IFilterService>();
            var notificationManager = new Mock<INotificationManager>();
            Filter filter = MakeFilterEntity(1);
            domainService.Setup(t => t.Find(1)).Returns(filter);
            FilterAppService filterAppService = new FilterAppService(domainService.Object, null, notificationManager.Object, null);
            //Act
            filterAppService.Delete(1);
            //Assert
            domainService.Verify(t => t.Delete(It.Is<int>(r => r == 1)));
            notificationManager.Verify(t => t.NotifyDeleteFilter(It.Is<int>(r => r == 10000), It.Is<int>(r => r == 1)));
        }

        [Fact]
        public void ShouldDeleteWhenFilterIsNotFound()
        {
            //Arrange
            var domainService = new Mock<IFilterService>();
            domainService.Setup(t => t.Find(1)).Returns<Filter>(null);
            FilterAppService filterAppService = new FilterAppService(domainService.Object, null, null, null);
            //Act
            Action action = () => filterAppService.Delete(1);
            //Assert
            Assert.Throws<ExceptionWithCode>(action);
        }

        [Fact]
        public void ShouldUpdateWhenUpdateFilterIsNotFound()
        {
            //Arrange
            var domainService = new Mock<IFilterService>();
            domainService.Setup(t => t.Find(1)).Returns<Filter>(null);
            FilterAppService filterAppService = new FilterAppService(domainService.Object, null, null, null);
            //Act
            Action action = () => filterAppService.Update(1, MakeFilterUpdateDto());
            //Assert
            Assert.Throws<ExceptionWithCode>(action);
        }

        [Fact]
        public async Task ShouldUpdate()
        {
            //Arrange
            var domainService = new Mock<IFilterService>();
            var unitOfWorkManager = new Mock<IUnitOfWorkManager>();
            var notificationManager = new Mock<INotificationManager>();
            var agentService = new Mock<IAgentService>();
            unitOfWorkManager.Setup(t => t.Current.SaveChanges());
            Filter filter = MakeFilterEntity(1);
            domainService.Setup(t => t.Find(1)).Returns(filter);
            FilterAppService filterAppService = new FilterAppService(domainService.Object, agentService.Object, notificationManager.Object, null);
            filterAppService.UnitOfWorkManager = unitOfWorkManager.Object;
            //Act
            FilterDetailsDto filterDetailsDto = await filterAppService.Update(1, MakeFilterUpdateDto());
            //Assert
            domainService.Verify(t => t.DeleteConditons(It.Is<Filter>(r => r.Id == 1)));
            domainService.Verify(t => t.UpdateFilter(It.Is<Filter>(r => r.Id == 1), It.Is<FilterCondition[]>(r => r.FirstOrDefault().FieldId == 1)));
            notificationManager.Verify(t => t.NotifyUpdateFilter(It.Is<int>(r => r == 10000), It.Is<int>(r => r == 1)));
            agentService.Verify(t => t.FillCreatedByName(It.Is<List<FilterDetailsDto>>(r => r.FirstOrDefault().CreatedBy == 1)));
            AssertDtoEqualToEntity(filter, filterDetailsDto);
            AssertDtoEqualToEntity(filter, MakeFilterCreateDto());
        }

        [Fact]
        public void ShouldFindManageFilters()
        {
            //Arrange
            var domainService = new Mock<IFilterService>();
            var userContext = new Mock<IUserContext>();
            var agentService = new Mock<IAgentService>();
            domainService.Setup(t => t.FindAll()).Returns(new List<Filter>
            {
                MakeFilterEntity(1)
            }.AsQueryable());
            userContext.Setup(t => t.UserId).Returns(1);
            agentService.Setup(t => t.GetDisplayName(1)).Returns("a");
            FilterAppService filterAppService = new FilterAppService(domainService.Object, agentService.Object, null, null);
            filterAppService.UserContext = userContext.Object;
            //Act
            List<FilterManageDto> filterManageDtos = filterAppService.FindManageFilters();
            //Assert
            AssertDtoEqualToEntity(MakeFilterEntity(1), filterManageDtos.FirstOrDefault());
            Assert.Equal("a", filterManageDtos.FirstOrDefault().CreatedByName);
        }

        [Fact]
        public void ShouldSorting()
        {
            //Arrange
            var domainService = new Mock<IFilterService>();
            var userContext = new Mock<IUserContext>();
            var agentService = new Mock<IAgentService>();
            domainService.Setup(t => t.FindAll()).Returns(new List<Filter>
            {
                MakeFilterEntity(1)
            }.AsQueryable());
            agentService.Setup(t => t.GetDisplayName(1)).Returns("a");
            FilterAppService filterAppService = new FilterAppService(domainService.Object, agentService.Object, null, null);
            filterAppService.UserContext = userContext.Object;
            //Act
            List<FilterManageDto> filterManageDtos = filterAppService.Sorting(new List<FilterSortDto> { new FilterSortDto { Id = 1, Index = 2 } });
            //Assert
            Assert.Equal("a", filterManageDtos.FirstOrDefault().CreatedByName);
            Assert.Equal(2, filterManageDtos.FirstOrDefault().Index);
        }

        private void AssertDtoEqualToEntity(Filter entity, FilterManageDto dto)
        {
            Assert.NotNull(dto);
            Assert.Equal(entity.Id, dto.Id);
            Assert.Equal(entity.Name, dto.Name);
            Assert.Equal(entity.Index, dto.Index);
            Assert.Equal(entity.IfPublic, dto.IfPublic);
        }

        private void AssertDtoEqualToEntity(Filter entity, FilterListDto dto)
        {
            Assert.NotNull(dto);
            Assert.Equal(entity.Id, dto.Id);
            Assert.Equal(entity.Name, dto.Name);
            Assert.Equal(entity.Index, dto.Index);
            Assert.Equal(entity.IfPublic, dto.IfPublic);
            Assert.Equal(entity.CreatedBy, dto.CreatedBy);
        }

        private void AssertDtoEqualToEntity(Filter entity, FilterCreateDto dto)
        {
            Assert.NotNull(dto);
            Assert.Equal(entity.Name, dto.Name);
            Assert.Equal(entity.Index, dto.Index);
            Assert.Equal(entity.Type, dto.Type);
            Assert.Equal(entity.LogicalExpression, dto.LogicalExpression);
            Assert.Equal(entity.IfPublic, dto.IfPublic);
            Assert.Equal(entity.Conditions.FirstOrDefault().FieldId, dto.Conditions.FirstOrDefault().FieldId);
            Assert.Equal(entity.Conditions.FirstOrDefault().Value, dto.Conditions.FirstOrDefault().Value);

        }

        private void AssertDtoEqualToEntity(Filter entity, FilterDetailsDto dto)
        {
            Assert.NotNull(dto);
            Assert.Equal(entity.Name, dto.Name);
            Assert.Equal(entity.Index, dto.Index);
            Assert.Equal(entity.Type, dto.Type);
            Assert.Equal(entity.LogicalExpression, dto.LogicalExpression);
            Assert.Equal(entity.IfPublic, dto.IfPublic);
            Assert.Equal(entity.CreatedBy, dto.CreatedBy);
            Assert.Equal(entity.Conditions.FirstOrDefault().FieldId, dto.Conditions.FirstOrDefault().FieldId);
            Assert.Equal(entity.Conditions.FirstOrDefault().Value, dto.Conditions.FirstOrDefault().Value);

        }

        private Filter MakeFilterEntity(int id)
        {
            return new Filter
            {
                Id = id,
                Name = "name",
                Index = 1,
                Type = FilterType.All,
                LogicalExpression = "",
                IfPublic = true,
                CreatedBy = 1,
                SiteId = 10000,
                Conditions = new List<FilterCondition>
                {
                    new FilterCondition{ FieldId = 1 ,Value = "a"}
                }
            };
        }

        private FilterCreateDto MakeFilterCreateDto()
        {
            return new FilterCreateDto
            {
                Name = "name",
                Index = 1,
                IfPublic = true,
                Type = FilterType.All,
                LogicalExpression = "",
                Conditions = new List<FilterConditionCreateDto>
                {
                    new FilterConditionCreateDto{FieldId = 1, Value = "a"  }
                }
            };
        }

        private FilterUpdateDto MakeFilterUpdateDto()
        {
            return new FilterUpdateDto
            {
                Name = "name",
                Index = 1,
                IfPublic = true,
                Type = FilterType.All,
                LogicalExpression = "",
                Conditions = new List<FilterConditionCreateDto>
                {
                    new FilterConditionCreateDto{FieldId = 1, Value = "a"  }
                }
            };
        }
    }
}
