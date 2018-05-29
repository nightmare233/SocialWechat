using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Framework.Core.UnitTest.Extensions
{
    public class UriExtensionsTest
    {
        [Fact]
        public void ShouldGetMimeType()
        {
            Uri uri = new Uri("https://scontent.xx.fbcdn.net/v/t39.1997-6/851593_488524174594361_1054180181_n.png?oh=35d85ade93718d7b7bd419731dcce129&oe=5A0125F7");

            string mimeType = uri.GetMimeType();

            Assert.Equal("image/png", mimeType);
        }

        [Fact]
        public void ShouldGetDefaultMimeType()
        {
            Uri uri = new Uri("https://scontent.xx.fbcdn.net");

            string mimeType = uri.GetMimeType();

            Assert.Equal("application/octet-stream", mimeType);
        }
    }
}
