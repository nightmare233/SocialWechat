using Framework.Core;
using Social.Domain.Entities;
using Social.Domain.Repositories;
using Social.Infrastructure;
using Social.Infrastructure.Enum;
using Social.Infrastructure.Facebook;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;

namespace Social.Domain.DomainServices
{
    public interface ISocialAccountService : IDomainService<SocialAccount>
    {
        IQueryable<SocialAccount> FindAllWithDeleted();
        SocialAccount FindAccount(int id, SocialUserSource source);
        Task<SocialAccount> GetAccountAsync(SocialUserSource source, string originalId);
        Task<IList<SocialAccount>> GetAccountsAsync(SocialUserSource source);
        Task InsertSocialAccountInGeneralDb(SocialUserSource source, string originalId);
        SocialAccount MarkAsEnable(int id, bool? ifEnable = true);
        Task AddTwitterAccountAsync(string authorizationId, string oauthVerifier);
        Task<SocialAccount> AddFacebookPageAsync(SocialAccount socialAccount, string originalId);
    }

    public class SocialAccountService : DomainService<SocialAccount>, ISocialAccountService
    {
        ISiteSocialAccountRepository _siteSocialAccountRepo;
        IRepository<SocialUser> _socialUserRepo;
        IFbClient _fbClient;
        ITwitterAuthService _twitterAuthService;

        public SocialAccountService(
            ISiteSocialAccountRepository siteSocialAccountRepo,
            IRepository<SocialUser> socialUserRepo,
            IFbClient fbClient,
            ITwitterAuthService twitterAuthService
            )
        {
            _siteSocialAccountRepo = siteSocialAccountRepo;
            _socialUserRepo = socialUserRepo;
            _fbClient = fbClient;
            _twitterAuthService = twitterAuthService;
        }

        public SocialAccount MarkAsEnable(int id, bool? ifEnable = true)
        {
            var account = this.Find(id);
            if (account == null)
            {
                throw SocialExceptions.SocialUserIdNotExists(id);
            }
            account.IfEnable = ifEnable.Value;
            this.Update(account);
            return account;
        }

        public override IQueryable<SocialAccount> FindAll()
        {
            return Repository.FindAll().Include(t => t.SocialUser).Where(t => t.IsDeleted == false);
        }

        public IQueryable<SocialAccount> FindAllWithDeleted()
        {
            return Repository.FindAll();
        }

        public override SocialAccount Find(int id)
        {
            return Repository.FindAll().Include(t => t.SocialUser).Where(t => t.IsDeleted == false).FirstOrDefault(t => t.Id == id);
        }

        public SocialAccount FindAccount(int id, SocialUserSource source)
        {
            return Repository.FindAll().Include(t => t.SocialUser)
                .Where(t => t.Id == id && t.IsDeleted == false
                && t.SocialUser.Type == SocialUserType.IntegrationAccount
                && t.SocialUser.Source == source)
                .FirstOrDefault();
        }

        public async Task<SocialAccount> GetAccountAsync(SocialUserSource source, string originalId)
        {
            return await Repository.FindAll().Include(t => t.SocialUser)
                .Where(t => t.SocialUser.OriginalId == originalId
                && t.SocialUser.Source == source
                && t.SocialUser.Type == SocialUserType.IntegrationAccount
                && t.IfEnable && t.IsDeleted == false)
                .FirstOrDefaultAsync();
        }

        public async Task<IList<SocialAccount>> GetAccountsAsync(SocialUserSource source)
        {
            return await Repository.FindAll().Include(t => t.SocialUser)
                .Where(t => t.SocialUser.Type == SocialUserType.IntegrationAccount
                && t.SocialUser.Source == source
                && t.IfEnable && t.IsDeleted == false)
                .ToListAsync();
        }

        public async override Task<SocialAccount> InsertAsync(SocialAccount entity)
        {
            ApplyDefaultValue(entity);
            base.Insert(entity);
            await CurrentUnitOfWork.SaveChangesAsync();
            await InsertSocialAccountInGeneralDb(entity.SocialUser.Source, entity.SocialUser.OriginalId);

            return entity;
        }

        public async Task InsertSocialAccountInGeneralDb(SocialUserSource source, string originalId)
        {
            int? siteIdOrNull = CurrentUnitOfWork.GetSiteId();
            if (siteIdOrNull == null)
            {
                throw new InvalidOperationException("site id must have value.");
            }

            int siteId = siteIdOrNull.Value;
            if (source == SocialUserSource.Facebook)
            {
                await UnitOfWorkManager.RunWithNewTransaction(null, async () =>
                {
                    if (!_siteSocialAccountRepo.FindAll().Any(t => t.SiteId == siteId && t.FacebookPageId == originalId))
                    {
                        await _siteSocialAccountRepo.InsertAsync(new SiteSocialAccount { SiteId = siteId, FacebookPageId = originalId });
                    }
                });
            }

            if (source == SocialUserSource.Twitter)
            {
                await UnitOfWorkManager.RunWithNewTransaction(null, async () =>
                {
                    if (!_siteSocialAccountRepo.FindAll().Any(t => t.SiteId == siteId && t.TwitterUserId == originalId))
                    {
                        await _siteSocialAccountRepo.InsertAsync(new SiteSocialAccount { SiteId = siteId, TwitterUserId = originalId });
                    }
                });
            }
        }

        public async override Task DeleteAsync(SocialAccount entity)
        {
            await base.DeleteAsync(entity);

            var user = _socialUserRepo.Find(entity.Id);
            _socialUserRepo.Delete(user);

            await DeleteSocialAccountInGeneralDb(entity);
        }

        private async Task DeleteSocialAccountInGeneralDb(SocialAccount entity)
        {
            int? siteIdOrNull = CurrentUnitOfWork.GetSiteId();
            if (siteIdOrNull == null)
            {
                throw new InvalidOperationException("site id must have value.");
            }

            int siteId = siteIdOrNull.Value;
            if (entity.SocialUser.Source == SocialUserSource.Facebook)
            {
                await UnitOfWorkManager.RunWithNewTransaction(null, async () =>
               {
                   var accoutns = _siteSocialAccountRepo.FindAll().Where(t => t.SiteId == siteId && t.FacebookPageId == entity.SocialUser.OriginalId).ToList();
                   foreach (var account in accoutns)
                   {
                       await _siteSocialAccountRepo.DeleteAsync(account);
                   }
               });
            }

            if (entity.SocialUser.Source == SocialUserSource.Twitter)
            {
                await UnitOfWorkManager.RunWithNewTransaction(null, async () =>
               {
                   var accoutns = _siteSocialAccountRepo.FindAll().Where(t => t.SiteId == siteId && t.TwitterUserId == entity.SocialUser.OriginalId).ToList();
                   foreach (var account in accoutns)
                   {
                       await _siteSocialAccountRepo.DeleteAsync(account);
                   }
               });
            }
        }

        private void ApplyDefaultValue(SocialAccount entity)
        {
            entity.IfEnable = true;
            if (entity.SocialUser.Source == SocialUserSource.Facebook)
            {
                entity.IfConvertMessageToConversation = true;
                entity.IfConvertVisitorPostToConversation = true;
                entity.IfConvertWallPostToConversation = true;
            }
            if (entity.SocialUser.Source == SocialUserSource.Twitter)
            {
                entity.IfConvertMessageToConversation = true;
                entity.IfConvertTweetToConversation = true;
            }
        }

        private bool IsDupliated(SocialUserSource source, string originalId)
        {
            return Repository.FindAll().Any(t => t.IsDeleted == false && t.SocialUser.OriginalId == originalId && t.SocialUser.Source == source);
        }

        public async Task<SocialAccount> AddFacebookPageAsync(SocialAccount socialAccount, string originalId)
        {
            if (IsDupliated(SocialUserSource.Facebook, originalId))
            {
                throw SocialExceptions.BadRequest($"'{socialAccount.SocialUser.Name}' has already been added.");
            }

            await _fbClient.SubscribeApp(originalId, socialAccount.Token);

            var socialUser = _socialUserRepo.FindAll()
                .Where(t => t.OriginalId == originalId && t.Source == SocialUserSource.Facebook)
                .OrderByDescending(t => t.Id)
                .FirstOrDefault();
            if (socialUser == null)
            {
                // create a new integraton account.
                await this.InsertAsync(socialAccount);
            }
            else
            {
                // if the user was a customer or a deleted integration account.
                socialUser.Type = SocialUserType.IntegrationAccount; // make sure convert a customer to integration account.
                socialUser.IsDeleted = false; // make sure to restore a deleted integration account.
                socialUser.Avatar = socialAccount.SocialUser.Avatar;
                socialUser.Email = socialAccount.SocialUser.Email;
                socialUser.OriginalLink = socialAccount.SocialUser.OriginalLink;
                socialAccount.Id = socialUser.Id;
                socialAccount.SocialUser = socialUser;
                if (socialUser.SocialAccount == null)
                {
                    socialUser.SocialAccount = socialAccount;
                }
                else
                {
                    socialUser.SocialAccount.Token = socialAccount.Token;
                    socialUser.SocialAccount.FacebookPageCategory = socialAccount.FacebookPageCategory;
                    socialUser.SocialAccount.FacebookSignInAs = socialAccount.FacebookSignInAs;
                }
                socialUser.SocialAccount.IsDeleted = false;
                socialUser.SocialAccount.IfEnable = true;
                socialUser.SocialAccount.IfConvertMessageToConversation = true;
                socialUser.SocialAccount.IfConvertVisitorPostToConversation = true;
                socialUser.SocialAccount.IfConvertWallPostToConversation = true;
                _socialUserRepo.Update(socialUser);

                await this.InsertSocialAccountInGeneralDb(SocialUserSource.Facebook, originalId);
            }

            CurrentUnitOfWork.SaveChanges();
            return this.Find(socialAccount.Id);
        }

        public async Task AddTwitterAccountAsync(string authorizationId, string oauthVerifier)
        {
            var user = await _twitterAuthService.ValidateAuthAsync(authorizationId, oauthVerifier);
            if (user != null)
            {
                if (IsDupliated(SocialUserSource.Twitter, user.IdStr))
                {
                    throw SocialExceptions.BadRequest($"'{user.Name}' has already been added.");
                }

                SocialAccount account = new SocialAccount
                {
                    Token = user.Credentials.AccessToken,
                    TokenSecret = user.Credentials.AccessTokenSecret,
                    IfConvertMessageToConversation = true,
                    IfConvertTweetToConversation = true,
                    IfEnable = true
                };

                var socialUser = _socialUserRepo.FindAll()
                    .Where(t => t.OriginalId == user.IdStr && t.Source == SocialUserSource.Twitter)
                    .OrderByDescending(t => t.Id)
                    .FirstOrDefault();
                if (socialUser == null)
                {
                    account.SocialUser = new SocialUser
                    {
                        Name = user.Name,
                        ScreenName = user.ScreenName,
                        Email = user.Email,
                        Source = SocialUserSource.Twitter,
                        Type = SocialUserType.IntegrationAccount,
                        Avatar = user.ProfileImageUrl,
                        OriginalId = user.IdStr,
                        OriginalLink = TwitterHelper.GetUserUrl(user.ScreenName)
                    };

                    await this.InsertAsync(account);
                }
                else
                {
                    // if the user was a customer or a deleted integration account.
                    socialUser.Type = SocialUserType.IntegrationAccount; // make sure convert a customer to integration account.
                    socialUser.IsDeleted = false; // make sure to restore a deleted integration account.
                    socialUser.Avatar = user.ProfileImageUrl;
                    socialUser.Name = user.Name;
                    socialUser.ScreenName = user.ScreenName;
                    socialUser.Email = user.Email;
                    account.Id = socialUser.Id;
                    account.SocialUser = socialUser;
                    if (socialUser.SocialAccount == null)
                    {
                        socialUser.SocialAccount = account;
                    }
                    else
                    {
                        socialUser.SocialAccount.Token = account.Token;
                        socialUser.SocialAccount.TokenSecret = account.TokenSecret;
                    }
                    socialUser.SocialAccount.IsDeleted = false;
                    socialUser.SocialAccount.IfEnable = true;
                    socialUser.SocialAccount.IfConvertMessageToConversation = true;
                    socialUser.SocialAccount.IfConvertTweetToConversation = true;
                    _socialUserRepo.Update(socialUser);

                    await InsertSocialAccountInGeneralDb(SocialUserSource.Twitter, user.IdStr);
                }
            }
        }
    }
}
