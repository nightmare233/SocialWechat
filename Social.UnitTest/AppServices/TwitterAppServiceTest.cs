using Moq;
using Social.Application.AppServices;
using Social.Domain.DomainServices;
using Social.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tweetinvi.Models;
using Xunit;

namespace Social.UnitTest.AppServices
{
    public class TwitterAppServiceTest
    {
        [Fact]
        public void ShouldProcessTweet()
        {
            //Arrange
            var twitterService = new Mock<ITwitterService>();
            var twitterPullJobService = new Mock<ITwitterPullJobService>();
            var tweet = new Mock<ITweet>();
             TwitterAppService TwitterAppService = new TwitterAppService(twitterService.Object,twitterPullJobService.Object);
            //Act
            TwitterAppService.ProcessTweet(new SocialAccount { SiteId = 10000}, tweet.Object);
            //Assert
        }
    }
}
