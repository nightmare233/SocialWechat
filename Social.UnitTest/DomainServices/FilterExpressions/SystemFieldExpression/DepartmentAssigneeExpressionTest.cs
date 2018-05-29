using Framework.Core;
using Moq;
using Social.Domain.DomainServices;
using Social.Domain.Entities;
using Social.Infrastructure.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Social.UnitTest.DomainService.FilterExpressions.SystemFieldExpression
{
    public class DepartmentAssigneeExpressionTest
    {
        [Fact]
        public void ShouldFilterByIsCondition()
        {
            FilterCondition condition = new FilterCondition
            {
                Field = new ConversationField { Name = "Department Assignee", DataType = FieldDataType.Option },
                MatchType = ConditionMatchType.Is,
                Value = "@My Department"
            };

            var options = new ExpressionBuildOptions
            {
                UserId = 1,
                MyDepartments = new int[] { 1}
            };
            var expression = new DepartmentAssigneeExpression().SetOptions(options).Build(condition);

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
            FilterCondition condition = new FilterCondition
            {
                Field = new ConversationField { Name = "Department Assignee", DataType = FieldDataType.Option },
                MatchType = ConditionMatchType.IsNot,
                Value = "@My Department"
            };
            var options = new ExpressionBuildOptions
            {
                UserId = 1,
                MyDepartments = new int[] { 1 }
            };
            var expression = new DepartmentAssigneeExpression().SetOptions(options).Build(condition);

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
