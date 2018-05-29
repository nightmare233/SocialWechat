using Framework.Core;
using Social.Application.AppServices;
using Social.Domain.DomainServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Quartz;
using Social.Domain.Entities;

namespace Social.Job.Jobs
{
    public class TwitterPullDirectMessagesJob : JobBase, ITransient
    {
        protected async override Task ExecuteJob(IJobExecutionContext context)
        {
            var socialAccounts = await GetTwitterSocialAccounts(context);
            foreach (var socialAccount in socialAccounts)
            {
                ITwitterAppService twitterAppService = DependencyResolver.Resolve<ITwitterAppService>();
                await twitterAppService.PullDirectMessages(socialAccount);
            }
        }
    }
}
