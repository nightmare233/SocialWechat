using Framework.Core;
using Moq;
using Social.Application.AppServices;
using Social.Application.Dto;
using Social.Domain;
using Social.Domain.DomainServices;
using Social.Domain.Entities;
using Social.Infrastructure.Enum;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Social.UnitTest.DomainService.FilterExpressions.SystemFieldExpression
{
    public class TimeToLastMessageExpressionTest
    {
        //[Fact]
        //public void ShouldFilterByIsCondition()
        //{
        //    FilterCondition condition = new FilterCondition
        //    {
        //        Field = new ConversationField { Name = "Time Since Last Message", DataType = FieldDataType.Number },
        //        MatchType = ConditionMatchType.Is,
        //        Value = "0"
        //    };

        //    //var conversationProspect = new Mock<DbSet<Conversation>>();
        //    // var mockContext = new Mock<SiteDataContext>();

        //    //mockContext.Setup(m => m.Conversations).Returns(conversationProspect.Object);
        //    //mockContext.Setup(m => m.Set<Conversation>()).Returns(conversationProspect.Object);

        //    //var service = new DomainService<Conversation>();
        //    //var conversationService = new Mock<IDomainService<Conversation>>();


        //    //conversationService.Object.Insert(new Conversation { Id = 1, LastMessageSentTime = DateTime.UtcNow });

        //    //conversationProspect.Verify(m => m.Add(It.IsAny<Conversation>()), Times.Once());
        //    //mockContext.Verify(m => m.SaveChanges(), Times.Once());
        //    var contextMock = DbContextMockFactory.Create<TestDbContext>();
        //    var conversation = new Conversation { Id = 1, LastMessageSentTime = DateTime.UtcNow };
        //    var setMock = contextMock.MockSetFor<Conversation>(conversation);
        //    // var context = new SiteDataContext("");

        //    //  context.Conversations.Add(new Conversation());

        //    var expression = new TimeToLastMessageExpression().Build(condition);

        //    var conversations = setMock.Object.Conversations.AsQueryable();

        //    var result = conversations.Where(expression).ToList();

        //    Assert.Equal(1, result.Count);
        //    Assert.Equal(1, result.First().Id);
        //}

        //public class TestDbContext : SiteDataContext
        //{
        //    public TestDbContext() : base("test")
        //    {

        //    }
        //}


        //[Fact]
        //public void ShouldFilterByIsNotCondition()
        //{
        //    FilterCondition condition = new FilterCondition
        //    {
        //        Field = new ConversationField { Name = "Time Since Last Message", DataType = FieldDataType.Number },
        //        MatchType = ConditionMatchType.IsNot,
        //        Value = "0"
        //    };

        //    var expression = new TimeToLastMessageExpression().Build(condition);

        //    var conditions = new List<Conversation>
        //    {
        //        new Conversation{Id=1,LastMessageSentTime =DateTime.UtcNow },
        //        new Conversation{Id=2,LastMessageSentTime=DateTime.UtcNow.AddDays(-2)}
        //    }.AsQueryable();

        //    var result = conditions.Where(expression).ToList();

        //    Assert.Equal(1, result.Count);
        //    Assert.Equal(2, result.First().Id);
        //}

        //[Fact]
        //public void ShouldFilterByIsLessThanCondition()
        //{
        //    FilterCondition condition = new FilterCondition
        //    {
        //        Field = new ConversationField { Name = "Time Since Last Message", DataType = FieldDataType.Number },
        //        MatchType = ConditionMatchType.IsLessThan,
        //        Value = "2"
        //    };

        //    var expression = new TimeToLastMessageExpression().Build(condition);

        //    var conditions = new List<Conversation>
        //    {
        //        new Conversation{Id=1,LastMessageSentTime =DateTime.UtcNow },
        //        new Conversation{Id=2,LastMessageSentTime=DateTime.UtcNow.AddDays(-2)}
        //    }.AsQueryable();

        //    var result = conditions.Where(expression).ToList();

        //    Assert.Equal(1, result.Count);
        //    Assert.Equal(1, result.First().Id);
        //}

        //[Fact]
        //public void ShouldFilterByIsMoreThanCondition()
        //{
        //    FilterCondition condition = new FilterCondition
        //    {
        //        Field = new ConversationField { Name = "Time Since Last Message", DataType = FieldDataType.Number },
        //        MatchType = ConditionMatchType.IsMoreThan,
        //        Value = "1"
        //    };

        //    var expression = new TimeToLastMessageExpression().Build(condition);

        //    var conditions = new List<Conversation>
        //    {
        //        new Conversation{Id=1,LastMessageSentTime =DateTime.UtcNow },
        //        new Conversation{Id=2,LastMessageSentTime=DateTime.UtcNow.AddDays(-2)}
        //    }.AsQueryable();

        //    var result = conditions.Where(expression).ToList();

        //    Assert.Equal(1, result.Count);
        //    Assert.Equal(2, result.First().Id);
        //}
    }
}
