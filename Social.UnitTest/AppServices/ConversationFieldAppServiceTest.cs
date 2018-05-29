using Moq;
using Social.Application.AppServices;
using Social.Application.Dto;
using Social.Domain.DomainServices;
using Social.Domain.Entities;
using Social.Infrastructure.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Social.UnitTest.AppServices
{
    public class ConversationFieldAppServiceTest:TestBase
    {
        [Fact]
        public void ShouldGetAllFieldDtos()
        {
            //Arrange
            var conversationFields = new List<ConversationField>
            {
              MakeConversationEntity(1),
              MakeConversationEntity(2),
            };
            var domainService = new Mock<IConversationFieldService>();
            domainService.Setup(t => t.FindAllAndFillOptions()).Returns(conversationFields);
            ConversationFieldAppService conversationFieldAppService = new ConversationFieldAppService(domainService.Object);
            //Act
            List<ConversationFieldDto> conversationDtos = conversationFieldAppService.FindAll();
            //Assert
            Assert.True(conversationDtos.Any());
            AssertDtoEqualToEntity(conversationFields.First(t => t.Id == 1), conversationDtos[0]);
            AssertDtoEqualToEntity(conversationFields.First(t => t.Id == 2), conversationDtos[1]);
        }

        [Fact]
        public void ShouldGetFieldDtoById()
        {
            //Arrange
            var domainService = new Mock<IConversationFieldService>();
            domainService.Setup(t => t.Find(1)).Returns(MakeConversationEntity(1));
            ConversationFieldAppService conversationFieldAppService = new ConversationFieldAppService(domainService.Object);
            //Act
            ConversationFieldDto conversationDto = conversationFieldAppService.Find(1);
            //Assert
            Assert.NotNull(conversationDto);
            AssertDtoEqualToEntity(MakeConversationEntity(1), conversationDto);
        }

        private void AssertDtoEqualToEntity(ConversationField entity, ConversationFieldDto dto)
        {
            Assert.NotNull(dto);
            Assert.Equal(entity.Id, dto.Id);
            Assert.Equal(entity.DataType, dto.DataType);
            Assert.Equal(entity.Name, dto.Name);
            Assert.Equal(entity.Options.First().FieldId, dto.Options.First().FieldId);
            Assert.Equal(entity.Options.First().Name, dto.Options.First().Name);
            Assert.Equal(entity.Options.First().Value, dto.Options.First().Value);
            Assert.Equal(entity.Options.First().Index, dto.Options.First().Index);
        }

        private ConversationField MakeConversationEntity(int id)
        {
            return new ConversationField
            {
                     Id = id,
                     DataType = FieldDataType.String,
                     Name = "Field",
                     Options = new List<ConversationFieldOption>
                     {
                         new ConversationFieldOption
                         {
                             FieldId = 1,
                             Name = "a",
                             Value = "1",
                             Index = 1
                         }
                     }
            };
        }
    }
}
