using Framework.Core;
using Moq;
using Social.Domain;
using Social.Domain.DomainServices;
using Social.Domain.Entities;
using Social.Infrastructure.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Social.UnitTest.DomainServices
{
    public class FilterServiceTest
    {
        //[Fact]
        //public void ShouldCheckFieldValueWorks()
        //{
        //    // Arrange
        //    var filterConditionRepo = new Mock<IRepository<FilterCondition>>();
        //    var filterRepo = new Mock<IRepository<Filter>>();
        //    var conversationService = new Mock<IRepository<Conversation>>();
        //    var userRepo = new Mock<IRepository<SocialUser>>();
        //    var conversation = new Mock<IConversationService>();
        //    var conversationFieldRepo = new Mock<IRepository<ConversationField>>();

        //    var conversationFieldOptionService = new Mock<IConversationFieldService>();
        //    conversationFieldOptionService.Setup(t => t.FindAllAndFillOptions()).Returns(new List<ConversationField>
        //    {
        //        new ConversationField{ Id = 14,DataType = FieldDataType.DateTime,Options = new List<ConversationFieldOption>
        //        {
        //            new ConversationFieldOption{FieldId = 14, Value = "@Today" },
        //            new ConversationFieldOption{FieldId = 14, Value = "@Yesterday" }
        //        }},
        //        new ConversationField{ Id = 5,DataType = FieldDataType.Option,Options = new List<ConversationFieldOption>
        //        {
        //            new ConversationFieldOption{FieldId = 5, Value = "Low" },
        //            new ConversationFieldOption{FieldId = 5, Value = "High" }
        //        }}
        //    });
        //       var filterService = new FilterService(filterConditionRepo.Object, filterRepo.Object, conversationService.Object, userRepo.Object, conversation.Object,
        //       conversationFieldRepo.Object, conversationFieldOptionService.Object);
        //    List<FilterCondition> filterConditons = new List<FilterCondition>
        //    {
        //        new FilterCondition{ Id = 1, FieldId = 14,MatchType = ConditionMatchType.Between,Value = "@Today|1"},
        //        new FilterCondition{ Id = 2, FieldId = 5,MatchType = ConditionMatchType.Is,Value = "1"}

        //    };
        //    // Act
        //    Action action = () => { filterService.CheckFieldValue(filterConditons); };
        //    // Assert
        //    Assert.Throws<ExceptionWithCode>(action);
        //}
    }
}
