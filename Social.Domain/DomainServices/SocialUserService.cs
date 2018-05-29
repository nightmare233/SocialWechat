using Framework.Core;
using Social.Domain.Entities;
using Social.Infrastructure;
using Social.Infrastructure.Enum;
using Social.Infrastructure.Facebook;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tweetinvi;
using Tweetinvi.Models;

namespace Social.Domain.DomainServices
{
    public interface ISocialUserService : IDomainService<SocialUser>
    {
        IQueryable<SocialUser> FindAllWithDeleted();
        Task<SocialUser> GetOrCreateTwitterUser(IUser twitterUser);
        Task<SocialUser> GetOrCreateTwitterUser(string orignalUserId);
        Task<SocialUser> GetOrCreateFacebookUser(string token, string fbUserId);
        Task<List<SocialUser>> GetOrCreateSocialUsers(string token, List<FbUser> fbSenders);
    }

    public class SocialUserService : DomainService<SocialUser>, ISocialUserService
    {
        private IFbClient _fbClient;

        public SocialUserService(IFbClient fbClient)
        {
            _fbClient = fbClient;
        }

        public IQueryable<SocialUser> FindAllWithDeleted()
        {
            return base.FindAll();
        }

        public override IQueryable<SocialUser> FindAll()
        {
            return base.FindAll().Where(t => t.IsDeleted == false);
        }

        public override SocialUser Find(int id)
        {
            return base.FindAll().Where(t => t.IsDeleted == false).FirstOrDefault(t => t.Id == id);
        }

        private SocialUser FindByOriginalId(string originalId, SocialUserSource source)
        {
            return FindAllWithDeleted()
                .Where(t => t.OriginalId == originalId && t.Source == source)
                .OrderByDescending(t => t.Id)
                .FirstOrDefault();
        }

        public async Task<SocialUser> GetOrCreateTwitterUser(IUser twitterUser)
        {
            var user = FindByOriginalId(twitterUser.IdStr, SocialUserSource.Twitter);
            if (user == null)
            {
                user = new SocialUser
                {
                    OriginalId = twitterUser.IdStr,
                    Name = twitterUser.Name,
                    ScreenName = twitterUser.ScreenName,
                    Avatar = twitterUser.ProfileImageUrl,
                    Source = SocialUserSource.Twitter,
                    Type = SocialUserType.Customer,
                    OriginalLink = TwitterHelper.GetUserUrl(twitterUser.ScreenName)
                };

                await UnitOfWorkManager.RunWithNewTransaction(CurrentUnitOfWork.GetSiteId(), async () =>
                {
                    await Repository.InsertAsync(user);
                    CurrentUnitOfWork.SaveChanges();
                });
            }

            if (user.IsDeleted)
            {
                user.IsDeleted = false;
                user.Type = SocialUserType.Customer;
                Update(user);
            }

            return user;
        }

        public async Task<SocialUser> GetOrCreateTwitterUser(string orignalUserId)
        {
            var user = FindByOriginalId(orignalUserId, SocialUserSource.Twitter);
            if (user == null)
            {
                IUser twitterUser = Tweetinvi.User.GetUserFromId(long.Parse(orignalUserId));
                user = FindByOriginalId(orignalUserId, SocialUserSource.Twitter);
                if (user == null)
                {
                    user = new SocialUser
                    {
                        OriginalId = twitterUser.IdStr,
                        Name = twitterUser.Name,
                        ScreenName = twitterUser.ScreenName,
                        Avatar = twitterUser.ProfileImageUrl,
                        Source = SocialUserSource.Twitter,
                        Type = SocialUserType.Customer,
                        OriginalLink = TwitterHelper.GetUserUrl(twitterUser.ScreenName)
                    };

                    await UnitOfWorkManager.RunWithNewTransaction(CurrentUnitOfWork.GetSiteId(), async () =>
                    {
                        await Repository.InsertAsync(user);
                    });
                }
            }

            if (user.IsDeleted)
            {
                user.IsDeleted = false;
                user.Type = SocialUserType.Customer;
                Update(user);
            }

            return user;
        }

        public async Task<SocialUser> GetOrCreateFacebookUser(string token, string fbUserId)
        {
            var user = FindByOriginalId(fbUserId, SocialUserSource.Facebook);
            if (user == null)
            {
                FbUser fbUser = await _fbClient.GetUserInfo(token, fbUserId);
                user = FindByOriginalId(fbUserId, SocialUserSource.Facebook);
                if (user == null)
                {
                    user = new SocialUser
                    {
                        OriginalId = fbUser.id,
                        Name = fbUser.name,
                        Email = fbUser.email,
                        OriginalLink = fbUser.link,
                        Avatar = fbUser.pic,
                        Source = SocialUserSource.Facebook
                    };

                    await UnitOfWorkManager.RunWithNewTransaction(CurrentUnitOfWork.GetSiteId(), async () =>
                    {
                        await InsertAsync(user);
                    });
                }
            }

            if (user.IsDeleted)
            {
                user.IsDeleted = false;
                user.Type = SocialUserType.Customer;
                Update(user);
            }

            return user;
        }

        public async Task<List<SocialUser>> GetOrCreateSocialUsers(string token, List<FbUser> fbSenders)
        {
            var uniqueFbSenders = new List<FbUser>();
            foreach (var fbSender in fbSenders)
            {
                if (!uniqueFbSenders.Any(t => t.id == fbSender.id))
                {
                    uniqueFbSenders.Add(fbSender);
                }
            }

            List<SocialUser> senders = new List<SocialUser>();
            var fbSenderIds = uniqueFbSenders.Select(t => t.id).ToList();
            var existingUsers = FindAllWithDeleted().Where(t => t.Source == SocialUserSource.Facebook && fbSenderIds.Contains(t.OriginalId)).ToList();
            senders.AddRange(existingUsers);
            uniqueFbSenders.RemoveAll(t => existingUsers.Any(e => e.OriginalId == t.id));
            foreach (var fbSender in uniqueFbSenders)
            {
                var sender = new SocialUser()
                {
                    OriginalId = fbSender.id,
                    Name = fbSender.name,
                    Email = fbSender.email,
                    Source = SocialUserSource.Facebook,
                };

                await UnitOfWorkManager.RunWithNewTransaction(CurrentUnitOfWork.GetSiteId(), async () =>
                {
                    await InsertAsync(sender);
                    senders.Add(sender);
                });
            }

            foreach (var sender in senders)
            {
                if (sender.IsDeleted)
                {
                    sender.IsDeleted = false;
                    sender.Type = SocialUserType.Customer;
                    Update(sender);
                }
            }

            return senders;
        }
    }
}
