using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tweetinvi;
using Tweetinvi.Credentials.Models;
using Tweetinvi.Models;
using Tweetinvi.Parameters;

namespace Social.Infrastructure.Twitter
{
    public class TwitterClient : ITwitterClient
    {
        public IAuthenticationContext InitAuthentication(string redirectUri)
        {
            var appCreds = new ConsumerCredentials(AppSettings.TwitterConsumerKey, AppSettings.TwitterConsumerSecret);
            return AuthFlow.InitAuthentication(appCreds, redirectUri);
        }

        public IAuthenticatedUser GetAuthenticatedUser(string oauthVerifier, string authorizationKey, string authorizationSecret)
        {
            var appCreds = new ConsumerCredentials(AppSettings.TwitterConsumerKey, AppSettings.TwitterConsumerSecret);
            var token = new AuthenticationToken()
            {
                AuthorizationKey = authorizationKey,
                AuthorizationSecret = authorizationSecret,
                ConsumerCredentials = appCreds
            };

            var userCredentils = AuthFlow.CreateCredentialsFromVerifierCode(oauthVerifier, token);
            var user = User.GetAuthenticatedUser(userCredentils);
            return user;
        }

        public void SetUserCredentials(string token, string secret)
        {
            Auth.SetUserCredentials(AppSettings.TwitterConsumerKey, AppSettings.TwitterConsumerSecret, token, secret);
        }

        public ITweet GetTweet(long tweetId)
        {
            return Tweet.GetTweet(tweetId);
        }

        public ITweet GetTweet(string userToken, string userSecret, long tweetId)
        {
            SetUserCredentials(userToken, userSecret);
            return Tweet.GetTweet(tweetId);
        }

        public IEnumerable<ITweet> GetTweets(long[] tweetIds)
        {
            return Tweet.GetTweets(tweetIds);
        }

        public IUser GetUser(long userId)
        {
            return User.GetUserFromId(userId);
        }

        public IUser GetUser(string userToken, string userSecret, long userId)
        {
            SetUserCredentials(userToken, userSecret);
            return User.GetUserFromId(userId);
        }

        public IEnumerable<IMessage> GetLatestMessagesReceived(int maxNumberOfMessagesRetrieve, long? maxId)
        {
            var parameter = new MessagesReceivedParameters
            {
                MaxId = maxId,
                MaximumNumberOfMessagesToRetrieve = maxNumberOfMessagesRetrieve
            };
            return Message.GetLatestMessagesReceived(parameter);
        }

        public IEnumerable<IMessage> GetLatestMessagesSent(int maxNumberOfMessagesRetrieve, long? maxId)
        {
            var parameter = new MessagesSentParameters
            {
                MaxId = maxId,
                MaximumNumberOfMessagesToRetrieve = maxNumberOfMessagesRetrieve
            };
            return Message.GetLatestMessagesSent(parameter);
        }

        public IEnumerable<ITweet> GetUserTimeline(long userId, int maxNumberOfTweetsRetrieve, long? maxId = null)
        {
            if (maxId.HasValue)
            {
                var parameter = new UserTimelineParameters { MaxId = maxId.Value, MaximumNumberOfTweetsToRetrieve = maxNumberOfTweetsRetrieve };
                return Timeline.GetUserTimeline(userId, parameter);
            }
            else
            {
                return Timeline.GetUserTimeline(userId, maxNumberOfTweetsRetrieve);
            }
        }

        public IEnumerable<ITweet> GetMentionsTimeline(int maxNumberOfTweetsRetrieve, long? maxId = null)
        {
            if (maxId.HasValue)
            {
                var parameter = new MentionsTimelineParameters { MaxId = maxId.Value, MaximumNumberOfTweetsToRetrieve = maxNumberOfTweetsRetrieve };
                return Timeline.GetMentionsTimeline(parameter);
            }
            else
            {
                return Timeline.GetMentionsTimeline(maxNumberOfTweetsRetrieve);
            }
        }

        public ITweet PublishTweet(string message, ITweet inReplyTo)
        {
            return Tweet.PublishTweet(message, new PublishTweetOptionalParameters
            {
                InReplyToTweet = inReplyTo
            });
        }

        public ITweet PublishTweet(string userToken, string userSecret, string message, ITweet inReplyTo)
        {
            SetUserCredentials(userToken, userSecret);
            return PublishTweet(message, inReplyTo);
        }

        public IMessage PublishMessage(string message, IUser user)
        {
            return Message.PublishMessage(message, user);
        }

        public IMessage PublishMessage(string userToken, string userSecret, string message, IUser user)
        {
            SetUserCredentials(userToken, userSecret);
            return Message.PublishMessage(message, user);
        }
    }
}
