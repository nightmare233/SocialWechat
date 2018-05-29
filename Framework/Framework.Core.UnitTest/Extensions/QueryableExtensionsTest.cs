using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Framework.Core.UnitTest.Extensions
{
    public class QueryableExtensionsTest
    {
        [Fact]
        public void ShouldPagingData()
        {
            var data = MakeTestData();
            int totalCount;
            var result = data.Paging(new Pager { Page = 2, PageSize = 10, IsAsc = true }, out totalCount);

            Assert.Equal(10, result.Count());
            Assert.Equal(500, totalCount);
            Assert.True(result.All(t => t.Id > 10 && t.Id <= 20));
        }


        [Fact]
        public void ShouldPagingIdData()
        {
            var data = MakeTestData();
            var result = data.Paging(new IdPager { SinceId = 10, MaxId = 20 });

            Assert.Equal(10, result.Count());
            Assert.True(result.All(t => t.Id > 10 && t.Id <= 20));
        }

        [Fact]
        public void ShouldApplyMaxNumberWhenPaingIdData()
        {
            var data = MakeTestData();
            var result = data.Paging(new IdPager { SinceId = 10, MaxId = 20, MaxNumberOfDataRetrieve = 5 });

            Assert.Equal(5, result.Count());
            Assert.True(result.All(t => t.Id > 15 && t.Id <= 20));
        }

        [Fact]
        public void ShouldApplyDefaultMaxNumberWhenPaingIdData()
        {
            var data = MakeTestData();
            var result = data.Paging(new IdPager());

            Assert.Equal(200, result.Count());
            Assert.True(result.All(t => t.Id > 300 && t.Id <= 500));
        }

        private IQueryable<TestEntity> MakeTestData()
        {
            List<TestEntity> data = new List<TestEntity>();
            for (int i = 1; i <= 500; i++)
            {
                var entity = new TestEntity();
                entity.SetId(i);
                data.Add(entity);
            }

            return data.AsQueryable();
        }
    }

    public class TestEntity : Entity
    {
        public void SetId(int id)
        {
            Id = id;
        }
    }
}
