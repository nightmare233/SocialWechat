using Framework.Core;
using Social.Domain.DomainServices.Twitter;
using Social.Domain.Entities;
using Social.Infrastructure;
using Social.Infrastructure.Twitter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tweetinvi.Models;
using Tweetinvi.Parameters;

namespace Social.Domain.DomainServices
{
    public interface ITwitterPullJobService
    {
        Task PullDirectMessages(SocialAccount account);
        Task PullTweets(SocialAccount account);
    }

    public class TwitterPullJobService : ServiceBase, ITwitterPullJobService
    {
        private ITwitterService _twitterService;
        private ITwitterClient _twitterClient;

        public TwitterPullJobService(
            ITwitterService twitterService,
            ITwitterClient twitterClient
            )
        {
            _twitterService = twitterService;
            _twitterClient = twitterClient;
        }

        #region PullDirectMessages
        public async Task PullDirectMessages(SocialAccount account)
        {
            int maxNumberOfMessagesRetrieve = 50;
            DateTime since = DateTime.UtcNow.AddHours(-1);

            _twitterClient.SetUserCredentials(account.Token, account.TokenSecret);

            var recivedMessages = PullReceivedDirectMessages(account, maxNumberOfMessagesRetrieve, since);
            var sentMessages = PullSentDirectMessages(account, maxNumberOfMessagesRetrieve, since);

            var messages = recivedMessages.Concat(sentMessages).OrderBy(t => t.CreatedAt);
            foreach (var message in messages)
            {
                TwitterProcessResult result = null;
                await UnitOfWorkManager.RunWithNewTransaction(account.SiteId, async () =>
                {
                    result = await _twitterService.ProcessDirectMessage(account, message);
                });
                if (result != null)
                {
                    await result.Notify(account.SiteId);
                }
            }
        }

        private IList<IMessage> PullReceivedDirectMessages(SocialAccount account, int maxNumberOfMessagesRetrieve, DateTime since)
        {
            List<IMessage> messages = new List<IMessage>();
            var receivedDirectMessages = Tweetinvi.Message.GetLatestMessagesReceived(maxNumberOfMessagesRetrieve);
            while (receivedDirectMessages != null && receivedDirectMessages.Any())
            {
                if (receivedDirectMessages.First().CreatedAt.ToUniversalTime() <= since)
                {
                    break;
                }

                foreach (var message in receivedDirectMessages)
                {
                    if (message.CreatedAt.ToUniversalTime() <= since)
                    {
                        break;
                    }
                    messages.Add(message);
                };

                if (receivedDirectMessages.Any(t => t.CreatedAt.ToUniversalTime() <= since))
                {
                    break;
                }

                var maxId = receivedDirectMessages.Last().Id;
                var parameter = new MessagesReceivedParameters { MaxId = maxId, MaximumNumberOfMessagesToRetrieve = maxNumberOfMessagesRetrieve };
                receivedDirectMessages = Tweetinvi.Message.GetLatestMessagesReceived(parameter);

                if (receivedDirectMessages.Count() == 1 && receivedDirectMessages.First().Id == maxId)
                {
                    break;
                }
            }

            return messages;
        }

        private IList<IMessage> PullSentDirectMessages(SocialAccount account, int maxNumberOfMessagesRetrieve, DateTime since)
        {
            List<IMessage> messages = new List<IMessage>();
            var sentDirectMessages = Tweetinvi.Message.GetLatestMessagesSent(maxNumberOfMessagesRetrieve);
            while (sentDirectMessages != null && sentDirectMessages.Any())
            {
                if (sentDirectMessages.First().CreatedAt.ToUniversalTime() <= since)
                {
                    break;
                }

                foreach (var message in sentDirectMessages)
                {
                    if (message.CreatedAt.ToUniversalTime() <= since)
                    {
                        break;
                    }
                    messages.Add(message);
                };

                if (sentDirectMessages.Any(t => t.CreatedAt.ToUniversalTime() <= since))
                {
                    break;
                }

                var maxId = sentDirectMessages.Last().Id;
                var parameter = new MessagesSentParameters { MaxId = maxId, MaximumNumberOfMessagesToRetrieve = maxNumberOfMessagesRetrieve };
                sentDirectMessages = Tweetinvi.Message.GetLatestMessagesSent(parameter);

                if (sentDirectMessages.Count() == 1 && sentDirectMessages.First().Id == maxId)
                {
                    break;
                }
            }
            return messages;
        }
        #endregion

        #region PullTweets
        public async Task PullTweets(SocialAccount account)
        {
            int maxNumberOfTweetsRetrieve = 10;
            DateTime since = DateTime.UtcNow.AddHours(-1);

            _twitterClient.SetUserCredentials(account.Token, account.TokenSecret);

            var receivedTweets = PullMentionTimeLineTweets(account, maxNumberOfTweetsRetrieve, since);
            var sentTweets = new List<ITweet>();
            //var sentTweets = PullUserTimeLineTweets(account, maxNumberOfTweetsRetrieve, since);

            var tweets = receivedTweets.Concat(sentTweets).OrderBy(t => t.CreatedAt);
            foreach (var tweet in tweets)
            {
                TwitterProcessResult result = null;
                await UnitOfWorkManager.RunWithNewTransaction(account.SiteId, async () =>
                {
                    result = await _twitterService.ProcessTweet(account, tweet);
                });
                if (result != null)
                {
                    await result.Notify(account.SiteId);
                }
            }
        }

        private IList<ITweet> PullUserTimeLineTweets(SocialAccount account, int maxNumberOfTweetsRetrieve, DateTime since)
        {
            var timeLineTweets = new List<ITweet>();
            var tweets = _twitterClient.GetUserTimeline(long.Parse(account.SocialUser.OriginalId), maxNumberOfTweetsRetrieve);
            while (tweets != null && tweets.Any())
            {
                if (tweets.First().CreatedAt <= since)
                {
                    break;
                }

                foreach (var tweet in tweets)
                {
                    if (tweet.CreatedAt <= since)
                    {
                        break;
                    }
                    timeLineTweets.Add(tweet);
                };

                if (tweets.Any(t => t.CreatedAt <= since))
                {
                    break;
                }

                var maxId = tweets.Last().Id;
                tweets = _twitterClient.GetUserTimeline(long.Parse(account.SocialUser.OriginalId), maxNumberOfTweetsRetrieve, maxId);

                if (tweets.Count() == 1 && tweets.First().Id == maxId)
                {
                    break;
                }
            }
            return timeLineTweets;
        }
        private IList<ITweet> PullMentionTimeLineTweets(SocialAccount account, int maxNumberOfTweetsRetrieve, DateTime since)
        {
            var mentions = new List<ITweet>();
            var tweets = _twitterClient.GetMentionsTimeline(maxNumberOfTweetsRetrieve);
            while (tweets != null && tweets.Any())
            {
                if (tweets.First().CreatedAt.ToUniversalTime() <= since)
                {
                    break;
                }

                foreach (var tweet in tweets)
                {
                    if (tweet.CreatedAt.ToUniversalTime() <= since)
                    {
                        break;
                    }
                    mentions.Add(tweet);
                };

                if (tweets.Any(t => t.CreatedAt.ToUniversalTime() <= since))
                {
                    break;
                }

                var maxId = tweets.Last().Id;
                tweets = _twitterClient.GetMentionsTimeline(maxNumberOfTweetsRetrieve, maxId);

                if (tweets.Count() == 1 && tweets.First().Id == maxId)
                {
                    break;
                }
            }
            return mentions;
        }
        #endregion

    }
}
