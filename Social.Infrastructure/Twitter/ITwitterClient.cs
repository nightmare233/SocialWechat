using Framework.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tweetinvi.Models;

namespace Social.Infrastructure.Twitter
{
    public interface ITwitterClient : ITransient
    {
        IAuthenticatedUser GetAuthenticatedUser(string oauthVerifier, string authorizationKey, string authorizationSecret);
        IAuthenticationContext InitAuthentication(string redirectUri);
        void SetUserCredentials(string token, string secret);
        ITweet GetTweet(long tweetId);
        ITweet GetTweet(string userToken, string userSecret, long tweetId);
        IEnumerable<ITweet> GetTweets(long[] tweetIds);
        IUser GetUser(long userId);
        IUser GetUser(string userToken, string userSecret, long userId);
        IEnumerable<IMessage> GetLatestMessagesReceived(int maxNumberOfMessagesRetrieve, long? maxId);
        IEnumerable<IMessage> GetLatestMessagesSent(int maxNumberOfMessagesRetrieve, long? maxId);
        IEnumerable<ITweet> GetUserTimeline(long userId, int maxNumberOfTweetsRetrieve, long? maxId = null);
        IEnumerable<ITweet> GetMentionsTimeline(int maxNumberOfTweetsRetrieve, long? maxId = null);
        ITweet PublishTweet(string message, ITweet inReplyTo);
        ITweet PublishTweet(string userToken, string userSecret, string message, ITweet inReplyTo);
        IMessage PublishMessage(string message, IUser user);
        IMessage PublishMessage(string userToken, string userSecret, string message, IUser user);
    }
}
