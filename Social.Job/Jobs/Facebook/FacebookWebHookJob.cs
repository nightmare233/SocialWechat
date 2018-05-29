using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Quartz;
using Framework.Core;
using Social.Domain.Entities;
using Framework.Core.UnitOfWork;
using Social.Infrastructure.Facebook;
using Social.Application.AppServices;
using Social.Infrastructure.Enum;
using System.Data.Entity;
using Social.Domain.DomainServices;
using System.Transactions;

namespace Social.Job.Jobs
{
    public class FacebookWebHookJob : JobBase, ITransient
    {
        private IFacebookAppService _fbWebHookAppService;
        private IRepository<FacebookWebHookRawData> _hookRawDataRepo;
        private ISocialAccountService _socialAccountService;

        public FacebookWebHookJob(
            IFacebookAppService fbWebHookAppService,
            IRepository<FacebookWebHookRawData> hookRawDataRepo,
            ISocialAccountService socialAccountService
            )
        {
            _fbWebHookAppService = fbWebHookAppService;
            _hookRawDataRepo = hookRawDataRepo;
            _socialAccountService = socialAccountService;
        }


        protected async override Task ExecuteJob(IJobExecutionContext context)
        {
            var siteSocicalAccount = context.JobDetail.GetCustomData<SiteSocialAccount>();
            if (siteSocicalAccount == null)
            {
                return;
            }

            int siteId = siteSocicalAccount.SiteId;
            string facebookPageId = siteSocicalAccount.FacebookPageId;
            List<FacebookWebHookRawData> rawDataList = new List<FacebookWebHookRawData>();

            await UnitOfWorkManager.Run(TransactionScopeOption.Required, siteId, async () =>
             {
                 SocialAccount account = await _socialAccountService.GetAccountAsync(SocialUserSource.Facebook, facebookPageId);
                 if (account != null)
                 {
                     rawDataList = _hookRawDataRepo.FindAll().Where(t => t.IsDeleted == false).OrderBy(t => t.CreatedTime).Take(50).ToList();

                     foreach (FacebookWebHookRawData rawData in rawDataList)
                     {
                         var data = Newtonsoft.Json.JsonConvert.DeserializeObject<FbHookData>(rawData.Data);
                         await _fbWebHookAppService.ProcessWebHookData(/*account, */data);
                         _hookRawDataRepo.Delete(rawData);
                     }
                 }
             });
        }
    }
}
