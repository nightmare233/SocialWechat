using Framework.Core;
using Social.Domain.Entities;
using Social.Domain.Repositories;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Social.Domain.DomainServices
{
    public interface ISiteSocialAccountService
    {
        Task<List<SiteSocialAccount>> GetFacebookSiteAccountsAsync();
        Task<List<int>> GetFacebookSiteIdsAsync();
        Task<List<SiteSocialAccount>> GetTwitterSiteAccountsAsync();
        Task<List<int>> GetTwitterSiteIdsAsync();
    }

    public class SiteSocialAccountService : ServiceBase, ISiteSocialAccountService
    {
        private ISiteSocialAccountRepository _siteSocialAccountRepo;

        public SiteSocialAccountService(ISiteSocialAccountRepository siteSocialAccountRepo)
        {
            _siteSocialAccountRepo = siteSocialAccountRepo;
        }


        public async Task<List<SiteSocialAccount>> GetFacebookSiteAccountsAsync()
        {
            List<SiteSocialAccount> accounts = new List<SiteSocialAccount>();

            await UnitOfWorkManager.RunWithoutTransaction(null, () =>
            {
                return Task.Run(() =>
                     accounts = _siteSocialAccountRepo.FindAll().Where(t => t.FacebookPageId != null).ToList()
                    );
            });

            return accounts;
        }

        public async Task<List<int>> GetFacebookSiteIdsAsync()
        {
            List<int> siteIds = new List<int>();

            await UnitOfWorkManager.RunWithoutTransaction(null, () =>
            {
                return Task.Run(() =>
                     siteIds = _siteSocialAccountRepo.FindAll().Where(t => t.FacebookPageId != null).Select(t => t.SiteId).Distinct().ToList()
                    );
            });

            return siteIds;
        }

        public async Task<List<SiteSocialAccount>> GetTwitterSiteAccountsAsync()
        {
            List<SiteSocialAccount> accounts = new List<SiteSocialAccount>();

            await UnitOfWorkManager.RunWithoutTransaction(null, () =>
            {
                return Task.Run(() =>
                 accounts = _siteSocialAccountRepo.FindAll().Where(t => t.TwitterUserId != null).ToList()
                );
            });

            return accounts;
        }

        public async Task<List<int>> GetTwitterSiteIdsAsync()
        {
            List<int> siteIds = new List<int>();

            await UnitOfWorkManager.RunWithoutTransaction(null, () =>
            {
                return Task.Run(() =>
                     siteIds = _siteSocialAccountRepo.FindAll().Where(t => t.TwitterUserId != null).Select(t => t.SiteId).Distinct().ToList()
                    );
            });

            return siteIds;
        }
    }
}
