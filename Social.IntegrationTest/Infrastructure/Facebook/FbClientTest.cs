using Social.Infrastructure.Facebook;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Social.IntegrationTest.Infrastructure.Facebook
{
    public class FbClientTest : TestBase
    {
        private FbClient fbClient = new FbClient();


        [Fact]
        public async Task ShouldGetTaggedVisitorPost()
        {
            var result = await fbClient.GetTaggedVisitorPosts(TestFacebookAccount.SocialUser.OriginalId, TestFacebookAccount.Token);

            Assert.True(result.data.Count > 0);
            Assert.NotNull(result.paging);
        }

        [Fact]
        public async Task ShouldGetPostComment()
        {
            var comment = await fbClient.GetPostComment("153568051877788_153962908504969", TestFacebookAccount.Token);
            Assert.NotNull(comment);
        }

        [Fact]
        public async Task ShoulGetMe()
        {
            var me = await fbClient.GetMe("EAACEdEose0cBAKjXdit7tZC26ZAUHlMTbj3RZCDtQLgJaTfJXxS7TWZCeDihcpWBp4sKZAtvhSzfca9x7pxxIeSRZBLZBZBxC32eUDr0nQSqnbTIIJnkPR20R2BOMf3ZAN8F4A7S0FTT5F3xAuGVNixC9k7ZCYQHNOvmAqn71SkFU8vrphWTuhH7lAZBhRl2jkLdQsZD");
            Assert.NotNull(me);
        }

        [Fact]
        public async Task DeleteFacebookAccount()
        {
            await fbClient.UnSubscribeApp("18", "EAAR8yzs1uVQBACjMl0TyHgxHnsnKklXhwlymMAtZBMVzbtqJUc7yZCspGpzMVkTEE6bMExj1qJyNHEHGdaxc6oNnZB1FZBp2qvCf4p8F538tiOBUi2LZBZAhZBdUSTWj3b4aiAw5vpG7AvrPQhtqUErAyvZCizZAsyOsMA7wu8x7jN2PNz37WjrSK");
        }
    }
}
