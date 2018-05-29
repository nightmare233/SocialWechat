using Social.Domain.Entities;
using Social.Infrastructure.Twitter;
using System.Collections.Generic;
using System.Linq;
using Tweetinvi.Models;

namespace Social.Domain.DomainServices.Twitter
{
    public class TweetTreeWalker : ITweetTreeWalker
    {
        private ITwitterClient _twitterClient;
        public TweetTreeWalker(ITwitterClient twitterClient)
        {
            _twitterClient = twitterClient;
        }

        public List<ITweet> BuildTweetTree(ITweet currentTweet)
        {
            var tweets = new List<ITweet>();
            BuildTweetTree(currentTweet, tweets);
            return tweets;
        }

        private void BuildTweetTree(ITweet currentTweet, List<ITweet> tweets)
        {
            tweets.Add(currentTweet);

            // for performance reason, we just get 10 parent tweet in the tree.
            if (tweets.Count >= 10)
            {
                return;
            }

            if (currentTweet.InReplyToStatusId != null)
            {
                ITweet inReplyToTweet = _twitterClient.GetTweet(currentTweet.InReplyToStatusId.Value);
                if (inReplyToTweet == null)
                {
                    return;
                }
                BuildTweetTree(inReplyToTweet, tweets);
            }
        }

        public List<string> FindCustomerOriginalIdsInTweetTree(ITweet currentTweet, List<ITweet> tweets, IList<SocialAccount> socialAccounts)
        {
            return FindCustomerOriginalIdsFromAncestorsInTweetTree(currentTweet, tweets, socialAccounts)
                   .Concat(FindCustomerOriginalIdsFromDecendantsInTweetTree(currentTweet, tweets, socialAccounts)).Distinct().ToList();
        }

        private List<string> FindCustomerOriginalIdsFromAncestorsInTweetTree(ITweet currentTweet, List<ITweet> tweets, IList<SocialAccount> socialAccounts)
        {
            List<string> customerOriginalIds = new List<string>();
            if (currentTweet == null)
            {
                return customerOriginalIds;
            }

            // customer @ integration account
            if (!IsIntegrationAccount(currentTweet.CreatedBy.IdStr, socialAccounts)
                && currentTweet.UserMentions.Any(t => IsIntegrationAccount(t.IdStr, socialAccounts)))
            {
                customerOriginalIds.Add(currentTweet.CreatedBy.IdStr);
            }

            if (currentTweet.InReplyToStatusId != null)
            {
                ITweet ancestor = tweets.FirstOrDefault(t => t.Id == currentTweet.InReplyToStatusId);
                customerOriginalIds.AddRange(FindCustomerOriginalIdsFromAncestorsInTweetTree(ancestor, tweets, socialAccounts));
            }

            return customerOriginalIds;
        }

        private List<string> FindCustomerOriginalIdsFromDecendantsInTweetTree(ITweet currentTweet, List<ITweet> tweets, IList<SocialAccount> socialAccounts)
        {
            List<string> customerOriginalIds = new List<string>();

            // customer @ integration account
            if (!IsIntegrationAccount(currentTweet.CreatedBy.IdStr, socialAccounts)
                && currentTweet.UserMentions.Any(t => IsIntegrationAccount(t.IdStr, socialAccounts)))
            {
                customerOriginalIds.Add(currentTweet.CreatedBy.IdStr);
            }

            var decendants = tweets.Where(t => t.InReplyToStatusId == currentTweet.Id);
            if (!decendants.Any())
            {
                return customerOriginalIds;
            }

            foreach (var decendant in decendants)
            {
                customerOriginalIds.AddRange(FindCustomerOriginalIdsFromDecendantsInTweetTree(decendant, tweets, socialAccounts));
            }

            return customerOriginalIds;
        }

        private bool IsIntegrationAccount(string originalId, IList<SocialAccount> socialAccounts)
        {
            return socialAccounts.Any(t => t.SocialUser.OriginalId == originalId);
        }
    }
}
