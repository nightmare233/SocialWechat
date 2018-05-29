using Framework.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Quartz;
using Social.Job.Jobs.Facebook;
using Social.Domain;
using Social.Domain.Entities;
using Social.Infrastructure;
using Social.Domain.Core;
using Social.Domain.DomainServices;

namespace Social.Job.Jobs
{
    public class SchedulerJob : JobBase, ITransient
    {
        private static RunningJobs RunningJobs = new RunningJobs();

        private IScheduleJobManager _scheduleJobManager;
        private ISiteSocialAccountService _siteSocialAccountService;

        public SchedulerJob(
            IScheduleJobManager schedulerJobManager,
            ISiteSocialAccountService siteSocialAccountService
            )
        {
            _scheduleJobManager = schedulerJobManager;
            _siteSocialAccountService = siteSocialAccountService;
        }

        public static void RemoveRunningJob<TJob>(int siteId, string originalAccountId) where TJob : JobBase
        {
            RunningJobs.Remove<TJob>(siteId, originalAccountId);
        }

        protected async override Task ExecuteJob(IJobExecutionContext context)
        {
            // schedule job for every site
            List<int> facebookSiteIds = await _siteSocialAccountService.GetFacebookSiteIdsAsync();
            if (facebookSiteIds != null && facebookSiteIds.Any())
            {
                foreach (var siteId in facebookSiteIds)
                {
                    RunningJobs.Schedule<PullTaggedVisitorPostsJob>(_scheduleJobManager, siteId, CronTrigger(AppSettings.FacebookPullTaggedVisitorPostsJobCronExpression));
                    //RunningJobs.Schedule<PullVisitorPostsFromFeedJob>(_scheduleJobManager, siteId, CronTrigger(AppSettings.FacebookPullVisitorPostsFromFeedJobCronExpression));
                    //RunningJobs.Schedule<PullMessagesJob>(_scheduleJobManager, siteId, CronTrigger(AppSettings.FacebookPullMessagesJobCronExpression));
                    RunningJobs.Schedule<GetRawDataJob>(_scheduleJobManager, siteId, CronTrigger(AppSettings.FacebookGetRawDataJobCronExpression));
                }
            }

            // schedule job for every site
            //List<int> twitterSiteIds = await _siteSocialAccountService.GetTwitterSiteIdsAsync();
            //if (twitterSiteIds != null && twitterSiteIds.Any())
            //{
            //    foreach (var siteId in twitterSiteIds)
            //    {
            //        RunningJobs.Schedule<TwitterPullTweetsJob>(_scheduleJobManager, siteId, CronTrigger(AppSettings.TwitterPullTweetsJobCronExpression));
            //        RunningJobs.Schedule<TwitterPullDirectMessagesJob>(_scheduleJobManager, siteId, CronTrigger(AppSettings.TwitterPullDirectMessagesJobCronExpression));
            //    }
            //}

            // schedule job for every twitter integration account
            List<SiteSocialAccount> twitterAccounts = await _siteSocialAccountService.GetTwitterSiteAccountsAsync();
            if (twitterAccounts != null && twitterAccounts.Any())
            {
                foreach (var twitterAccount in twitterAccounts)
                {
                    RunningJobs.Schedule<TwitterUserStreamJob>(_scheduleJobManager, twitterAccount, StartNowTrigger());
                }
            }

            RunningJobs.StopTimeoutJobs(context.Scheduler);
        }

        private Action<TriggerBuilder> CronTrigger(string cronExpression)
        {
            return trigger =>
            {
                trigger.WithCronSchedule(cronExpression, x => x.WithMisfireHandlingInstructionIgnoreMisfires()).Build();
            };
        }

        private Action<TriggerBuilder> StartNowTrigger()
        {
            return trigger =>
            {
                trigger.StartNow().Build();
            };
        }
    }
}
