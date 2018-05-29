using Framework.Core;
using Quartz;
using Social.Application.AppServices;
using Social.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Social.Job.Jobs
{
    public class TwitterPullTweetsJob : JobBase, ITransient
    {
        protected async override Task ExecuteJob(IJobExecutionContext context)
        {
            var socialAccounts = await GetTwitterSocialAccounts(context);
            foreach (var socialAccount in socialAccounts)
            {
                ITwitterAppService twitterAppService = DependencyResolver.Resolve<ITwitterAppService>();
                await twitterAppService.PullTweets(socialAccount);
            }
        }
    }
}
