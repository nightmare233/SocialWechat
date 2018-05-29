using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Quartz;
using Framework.Core;
using Social.Application.AppServices;

namespace Social.Job.Jobs.Facebook
{
    public class GetRawDataJob : JobBase, ITransient
    {
        protected async override Task ExecuteJob(IJobExecutionContext context)
        {
            var socialAccounts = await GetFacebookSocialAccounts(context);
            foreach (var socialAccount in socialAccounts)
            {
                IMessageAttachmentAppService sevice = DependencyResolver.Resolve<IMessageAttachmentAppService>();
                await UnitOfWorkManager.RunWithoutTransaction(socialAccount.SiteId, async () =>
                {
                    await sevice.GetRawDataJob();
                });
            }
        }
    }
}
