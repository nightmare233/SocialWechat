using Moq;
using Social.Domain.DomainServices;
using Social.Domain.Entities;
using Social.Infrastructure.Enum;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Social.UnitTest.DomainService.FilterExpressions.SystemFieldExpression
{
    public class DepartmentAssigneeStatusExpressionTest
    {
        [Fact]
        public void ShouldFilterByIsCondition()
        {
            var departmentServiceMock = new Mock<IDepartmentService>();
            departmentServiceMock.Setup(t => t.GetMatchedStatusDepartments(1)).Returns(new[] { 1, 3 });


            FilterCondition condition = new FilterCondition
            {
                Field = new ConversationField { Name = "Department Assignee Status", DataType = FieldDataType.Option },
                MatchType = ConditionMatchType.Is,
                Value = "1"
            };
            var expression = new DepartmentAssigneeStatusExpression(departmentServiceMock.Object).Build(condition);

            var conditions = new List<Conversation>
            {
                new Conversation{Id=1,DepartmentId=1},
                new Conversation{Id=2,DepartmentId=null}
            }.AsQueryable();

            var result = conditions.Where(expression).ToList();

            Assert.Equal(1, result.Count);
            Assert.Equal(1, result.First().Id);
        }

        [Fact]
        public void ShouldFilterByIsNotCondition()
        {
            var departmentServiceMock = new Mock<IDepartmentService>();
            departmentServiceMock.Setup(t => t.GetMatchedStatusDepartments(1)).Returns(new[] { 1, 3 });


            FilterCondition condition = new FilterCondition
            {
                Field = new ConversationField { Name = "Department Assignee Status", DataType = FieldDataType.Option },
                MatchType = ConditionMatchType.IsNot,
                Value = "1"
            };
            var expression = new DepartmentAssigneeStatusExpression(departmentServiceMock.Object).Build(condition);

            var conditions = new List<Conversation>
            {
                new Conversation{Id=1,DepartmentId=1},
                new Conversation{Id=2,DepartmentId=null}
            }.AsQueryable();

            var result = conditions.Where(expression).ToList();

            Assert.Equal(1, result.Count);
            Assert.Equal(2, result.First().Id);
        }
    }
}
