using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Framework.Core.UnitTest.Extensions
{
    public class EnumExtensionsTest
    {
        [Fact]
        public void ShouldGetEnumName()
        {
            Assert.Equal("This is A", TestEnum.A.GetName());
            Assert.Equal("B", TestEnum.B.GetName());
        }

        public enum TestEnum
        {
            [Display(Name = "This is A")]
            A,
            B
        }
    }
}
