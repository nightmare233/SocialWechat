using Framework.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Quartz;
using Social.Application.AppServices;

namespace Social.Job.Jobs.Facebook
{
    public class PullMessagesJob : JobBase, ITransient
    {
        protected async override Task ExecuteJob(IJobExecutionContext context)
        {
            var socialAccounts = await GetFacebookSocialAccounts(context);
            foreach (var socialAccount in socialAccounts)
            {
                IFacebookAppService sevice = DependencyResolver.Resolve<IFacebookAppService>();
                await UnitOfWorkManager.RunWithoutTransaction(socialAccount.SiteId, async () =>
               {
                   await sevice.PullMessagesJob(socialAccount);
               });
            }
        }
    }
}
