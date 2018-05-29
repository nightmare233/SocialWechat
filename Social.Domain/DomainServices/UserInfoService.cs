using Framework.Core;
using Social.Domain.Entities;
using Social.Infrastructure.Facebook;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tweetinvi.Models;

namespace Social.Domain.DomainServices
{
    public interface IUserInfoService
    {
        //FbUser GetFacebookInfo(string OriginalId, Conversation conversation, int userId);
        //IUser GetTwitterInfo(string OriginalId, Conversation conversation, int userId);
    }
    public class UserInfoService : DomainService<SocialAccount>, IUserInfoService
    {
        private IRepository<SocialUser> _socialUserRepo;
        private IRepository<Message> _messageRepo;
        private IRepository<Conversation> _conversationRepo;
        private IRepository<SocialAccount> _socialAccountRepo;
        private IFbClient _fbClient;

        public UserInfoService(
            IRepository<SocialUser> socialUserRepo,
            IRepository<Message> messageRepo,
            IRepository<Conversation> conversationRepo,
            IRepository<SocialAccount> socialAccountRepo,
            IFbClient fbClient
            )
        {
            _socialUserRepo = socialUserRepo;
            _messageRepo = messageRepo;
            _conversationRepo = conversationRepo;
            _socialAccountRepo = socialAccountRepo;
            _fbClient = fbClient;
        }

        //public FbUser GetFacebookInfo(string OriginalId, Conversation conversation, int userId)
        //{
        //    var socialAccount = _socialAccountRepo.Find(userId);
        //    string token = null;
        //    if (socialAccount != null)
        //    {
        //        token = socialAccount.Token;
        //    }
        //    else
        //    {
        //        int? accountId = null;
        //        while (accountId == null)
        //        {
        //            accountId = conversation.Messages.First().SenderId == userId ? conversation.Messages.First().ReceiverId : conversation.Messages.First().SenderId;
        //        }
        //        token = _socialAccountRepo.Find(accountId.Value).Token;
        //    }

        //    FbUser fbUser = _fbClient.GetFacebookUserInfo(token, OriginalId);
        //    return fbUser;
        //}

        //public IUser GetTwitterInfo(string OriginalId, Conversation conversation, int userId)
        //{
        //    SocialAccount socialAccount = _socialAccountRepo.Find(userId);
        //    var twitterService = DependencyResolver.Resolve<ITwitterService>();
        //    if (socialAccount == null)
        //    {
        //        int? accountId = null;
        //        while (accountId == null)
        //        {
        //            accountId = conversation.Messages.First().SenderId == userId ? conversation.Messages.First().ReceiverId : conversation.Messages.First().SenderId;
        //        }
        //        socialAccount = _socialAccountRepo.Find(accountId.Value);
        //    }
        //    IUser twitterUser = twitterService.GetUser(socialAccount, long.Parse(OriginalId));
        //    return twitterUser;
        //}
    }
}
