using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Framework.Core.UnitTest.Extensions
{
    public class TypeExtensions
    {
        [Fact]
        public void ShouldReturnTrueIfHasAttribute()
        {
            Assert.True(typeof(TestClass).HasAttribute<TableAttribute>());
        }

        [Fact]
        public void ShouldGetAttribute()
        {
            var attribute = typeof(TestClass).GetAttribute<TableAttribute>();

            Assert.Equal("Test", attribute.Name);
        }

        [Fact]
        public void ShouldGetAttributes()
        {
            var attributes = typeof(TestClass).GetAttributes<TableAttribute>();

            Assert.Equal("Test", attributes[0].Name);
        }


        [Table("Test")]
        public class TestClass
        {

        }
    }
}
