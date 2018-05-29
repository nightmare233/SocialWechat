using Social.Domain.DomainServices.Twitter;
using Social.Infrastructure;
using Social.Infrastructure.Twitter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tweetinvi;
using Tweetinvi.Models;
using Xunit;

namespace Social.IntegrationTest.DomainServices.Twitter
{
    public class TweetTreeWalkerTest : TestBase
    {
        [Fact]
        public void ShouldOnlyWalk10LevelWhenBuildTweetTree()
        {
            TweetTreeWalker walker = new TweetTreeWalker(new TwitterClient());
            Auth.SetCredentials(new TwitterCredentials(AppSettings.TwitterConsumerKey, AppSettings.TwitterConsumerSecret,
              "880620227460866048-E1XTzWOFE1xLfxYDor5I5oGMiVUVs86", "ii3mahfjH0qjU4wupnj3Du6JbWjwPuM2p8jHoUrcgpLYp"));
            //var tweet = Tweet.GetTweet(894838703922728960); 10 level
            var tweet = Tweet.GetTweet(893393512925872129);
            var tweets = walker.BuildTweetTree(tweet);

            Assert.True(tweets.Count <= 10);
        }
    }
}
