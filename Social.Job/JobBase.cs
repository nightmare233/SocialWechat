using System;
using System.Runtime.Remoting.Contexts;
using System.Threading;
using System.Threading.Tasks;
using Common.Logging;
using Quartz;
using Framework.Core;
using Framework.Core.UnitOfWork;
using Social.Domain.Entities;
using Social.Domain.DomainServices;
using Social.Infrastructure.Enum;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Social.Job
{
    public abstract class JobBase : IJob
    {
        protected static ILog Logger = SchedulerLogger.GetLogger();

        private IUnitOfWorkManager _unitOfWorkManager;
        public IUnitOfWorkManager UnitOfWorkManager
        {
            get
            {
                if (_unitOfWorkManager == null)
                {
                    throw new InvalidOperationException("Must set UnitOfWorkManager before use it.");
                }

                return _unitOfWorkManager;
            }
            set { _unitOfWorkManager = value; }
        }

        public ISocialAccountService SocialAccountService { get; set; }

        public IDependencyResolver DependencyResolver { get; set; }

        protected IUnitOfWork CurrentUnitOfWork
        {
            get { return UnitOfWorkManager.Current; }
        }

        public async void Execute(IJobExecutionContext context)
        {

            JobKey jobKey = context.JobDetail.Key;
            Logger.Info($"-->{Thread.CurrentThread.Name}-{jobKey} start executing.");

            try
            {
                await ExecuteJob(context);
            }
            catch (Exception ex)
            {
                Logger.Error($"-->{Thread.CurrentThread.Name}-{jobKey} executed failed.", ex);
            }

            Logger.Info($"-->{Thread.CurrentThread.Name}-{jobKey} executed complete.");
        }

        protected abstract Task ExecuteJob(IJobExecutionContext context);

        protected async Task<SocialAccount> GetTwitterSocialAccount(IJobExecutionContext context)
        {
            var siteSocicalAccount = context.JobDetail.GetCustomData<SiteSocialAccount>();
            if (siteSocicalAccount == null)
            {
                return null;
            }

            int siteId = siteSocicalAccount.SiteId;
            string twitterUserId = siteSocicalAccount.TwitterUserId;

            SocialAccount socialAccount = null;
            await UnitOfWorkManager.RunWithoutTransaction(siteId, async () =>
            {
                socialAccount = await SocialAccountService.GetAccountAsync(SocialUserSource.Twitter, twitterUserId);
            });

            return socialAccount;
        }

        protected async Task<IList<SocialAccount>> GetTwitterSocialAccounts(IJobExecutionContext context)
        {
            IList<SocialAccount> result = new List<SocialAccount>();
            var siteId = context.JobDetail.GetCustomData<int>();
            if (siteId == 0)
            {
                return result;
            }

            await UnitOfWorkManager.RunWithoutTransaction(siteId, async () =>
            {
                result = await SocialAccountService.GetAccountsAsync(SocialUserSource.Twitter);
            });

            return result;
        }

        protected async Task<SocialAccount> GetFacebookSocialAccount(IJobExecutionContext context)
        {
            var siteSocicalAccount = context.JobDetail.GetCustomData<SiteSocialAccount>();
            if (siteSocicalAccount == null)
            {
                return null;
            }

            int siteId = siteSocicalAccount.SiteId;
            string facebookPageId = siteSocicalAccount.FacebookPageId;

            SocialAccount socialAccount = null;
            await UnitOfWorkManager.RunWithoutTransaction(siteId, async () =>
            {
                socialAccount = await SocialAccountService.GetAccountAsync(SocialUserSource.Facebook, facebookPageId);
            });

            return socialAccount;
        }

        protected async Task<IList<SocialAccount>> GetFacebookSocialAccounts(IJobExecutionContext context)
        {
            IList<SocialAccount> result = new List<SocialAccount>();
            var siteId = context.JobDetail.GetCustomData<int>();
            if (siteId == 0)
            {
                return result;
            }

            await UnitOfWorkManager.RunWithoutTransaction(siteId, async () =>
            {
                result = await SocialAccountService.GetAccountsAsync(SocialUserSource.Facebook);
                //result = result.Where(t => t.Id == 430031).ToList();
            });

            return result;
        }
    }
}
