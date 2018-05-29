using System.Collections.Generic;
using Social.Domain.Entities;
using Tweetinvi.Models;
using Framework.Core;

namespace Social.Domain.DomainServices.Twitter
{
    public interface ITweetTreeWalker : ITransient
    {
        List<ITweet> BuildTweetTree(ITweet currentTweet);
        List<string> FindCustomerOriginalIdsInTweetTree(ITweet currentTweet, List<ITweet> tweets, IList<SocialAccount> socialAccounts);
    }
}