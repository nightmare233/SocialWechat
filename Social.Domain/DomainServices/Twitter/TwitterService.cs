using Framework.Core;
using Social.Domain.DomainServices.Twitter;
using Social.Domain.Entities;
using Social.Infrastructure;
using Social.Infrastructure.Twitter;
using System.Threading.Tasks;
using Tweetinvi.Models;

namespace Social.Domain.DomainServices
{
    public interface ITwitterService
    {
        Task<TwitterProcessResult> ProcessDirectMessage(SocialAccount account, IMessage directMsg);
        Task<TwitterProcessResult> ProcessTweet(SocialAccount account, ITweet currentTweet);
        Message GetTweetMessage(SocialAccount socialAccount, long tweetId);
        ITweet GetTweet(SocialAccount socialAccount, long tweetId);
        ITweet ReplyTweet(SocialAccount socialAccount, ITweet inReplyTo, string message);
        IUser GetUser(SocialAccount socialAccount, long userId);
        IMessage PublishMessage(SocialAccount socialAccount, IUser user, string message);
    }

    public class TwitterService : ServiceBase, ITwitterService
    {
        private IConversationService _conversationService;
        private IMessageService _messageService;
        private ITwitterTweetService _tweetServcie;
        private ITwitterDirectMessageService _directMessageService;
        private ITwitterClient _twitterClient;

        public TwitterService(
            IConversationService conversationService,
            IMessageService messageService,
            ISocialUserService socialUserService,
            ITwitterTweetService tweetServcie,
            ITwitterDirectMessageService directMessageService,
            ITwitterClient twitterClient
            )
        {
            _conversationService = conversationService;
            _messageService = messageService;
            _tweetServcie = tweetServcie;
            _directMessageService = directMessageService;
            _twitterClient = twitterClient;
        }

        public Message GetTweetMessage(SocialAccount account, long tweetId)
        {
            _twitterClient.SetUserCredentials(account.Token, account.TokenSecret);
            var tweet = _twitterClient.GetTweet(tweetId);
            if (tweet == null)
            {
                return null;
            }

            var message = TwitterConverter.ConvertToMessage(tweet);
            message.Sender = new SocialUser
            {
                Name = tweet.CreatedBy.Name,
                Avatar = tweet.CreatedBy.ProfileImageUrl,
                ScreenName = tweet.CreatedBy.ScreenName,
                OriginalId = tweet.CreatedBy.IdStr,
                OriginalLink = TwitterHelper.GetUserUrl(tweet.CreatedBy.ScreenName)

            };
            return message;
        }

        public ITweet GetTweet(SocialAccount socialAccount, long tweetId)
        {
            return _twitterClient.GetTweet(socialAccount.Token, socialAccount.TokenSecret, tweetId);
        }

        public IUser GetUser(SocialAccount socialAccount, long userId)
        {
            return _twitterClient.GetUser(socialAccount.Token, socialAccount.TokenSecret, userId);
        }

        public ITweet ReplyTweet(SocialAccount socialAccount, ITweet inReplyTo, string message)
        {
            return _twitterClient.PublishTweet(socialAccount.Token, socialAccount.TokenSecret, message, inReplyTo);
        }

        public IMessage PublishMessage(SocialAccount socialAccount, IUser user, string message)
        {
            return _twitterClient.PublishMessage(socialAccount.Token, socialAccount.TokenSecret, message, user);
        }

        public async Task<TwitterProcessResult> ProcessDirectMessage(SocialAccount account, IMessage directMsg)
        {
            return await _directMessageService.ProcessDirectMessage(account, directMsg);
        }

        public async Task<TwitterProcessResult> ProcessTweet(SocialAccount currentAccount, ITweet currentTweet)
        {
            return await _tweetServcie.ProcessTweet(currentAccount, currentTweet);
        }
    }
}
