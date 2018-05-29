using Framework.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Quartz;
using Social.Domain.DomainServices;
using Framework.Core.UnitOfWork;
using Social.Domain.Entities;
using Social.Infrastructure.Enum;
using System.Data.Entity;
using Social.Application.AppServices;

namespace Social.Job.Jobs.Facebook
{
    public class PullVisitorPostsFromFeedJob : JobBase, ITransient
    {
        protected async override Task ExecuteJob(IJobExecutionContext context)
        {
            var socialAccounts = await GetFacebookSocialAccounts(context);
            foreach (var socialAccount in socialAccounts)
            {
                IFacebookAppService service = DependencyResolver.Resolve<IFacebookAppService>();
                await UnitOfWorkManager.RunWithoutTransaction(socialAccount.SiteId, async () =>
                {
                    await service.PullVisitorPostsFromFeed(socialAccount);
                });
            }
        }
    }
}
