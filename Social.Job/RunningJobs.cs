using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using Social.Domain.Entities;
using Quartz;

namespace Social.Job
{
    public partial class RunningJobs
    {
        private ConcurrentDictionary<string, RunningJob> _runningJobCache = new ConcurrentDictionary<string, RunningJob>();

        public bool IsRunning<TJob>(int siteId, string originalAccountId) where TJob : JobBase
        {
            string key = GetJobKey<TJob>(siteId, originalAccountId);
            return _runningJobCache.ContainsKey(key);
        }

        public bool IsRunning<TJob>(int siteId) where TJob : JobBase
        {
            string key = GetJobKey<TJob>(siteId);
            return _runningJobCache.ContainsKey(key);
        }

        public void Schedule<TJob>(IScheduleJobManager scheduleJobManager, SiteSocialAccount account, Action<TriggerBuilder> configureTrigger) where TJob : JobBase
        {
            string originalAccountId = !string.IsNullOrWhiteSpace(account.FacebookPageId) ? account.FacebookPageId : account.TwitterUserId;
            if (string.IsNullOrWhiteSpace(originalAccountId))
            {
                return;
            }

            string groupName = GetJobGroup<TJob>();
            string jobKey = GetJobKey<TJob>(account.SiteId, originalAccountId);
            bool isRunning = IsRunning<TJob>(account.SiteId, originalAccountId);
            if (!isRunning)
            {
                scheduleJobManager.ScheduleAsync<TJob, SiteSocialAccount>(
                job =>
                {
                    job.WithIdentity(jobKey, groupName);
                },
                configureTrigger,
                account
                );
            }

            var runningJob = new RunningJob
            {
                JobKey = jobKey,
                JobGroup = groupName,
                SiteId = account.SiteId,
                OriginalAccountId = originalAccountId,
                LastScheduleTime = DateTime.UtcNow
            };

            _runningJobCache.AddOrUpdate(jobKey, runningJob, (k, job) => { return runningJob; });
        }

        public void Schedule<TJob>(IScheduleJobManager scheduleJobManager, int siteId, Action<TriggerBuilder> configureTrigger) where TJob : JobBase
        {
            string groupName = GetJobGroup<TJob>();
            string jobKey = GetJobKey<TJob>(siteId);
            bool isRunning = IsRunning<TJob>(siteId);
            if (!isRunning)
            {
                scheduleJobManager.ScheduleAsync<TJob, int>(
                job =>
                {
                    job.WithIdentity(jobKey, groupName);
                },
                configureTrigger,
                siteId
                );
            }

            var runningJob = new RunningJob
            {
                JobKey = jobKey,
                JobGroup = groupName,
                SiteId = siteId,
                LastScheduleTime = DateTime.UtcNow
            };

            _runningJobCache.AddOrUpdate(jobKey, runningJob, (k, job) => { return runningJob; });
        }

        public void StopTimeoutJobs(IScheduler scheduler)
        {
            var timemOutJobs = _runningJobCache.Where(t => t.Value.IsTimeout).ToList();
            foreach (var timemOutJob in timemOutJobs)
            {
                JobKey jobKey = new JobKey(timemOutJob.Key, timemOutJob.Value.JobGroup);
                IJobDetail jobDetail = scheduler.GetJobDetail(jobKey);
                if (jobDetail != null)
                {
                    bool isDeleted = scheduler.DeleteJob(jobKey);
                }

                RunningJob stopjob;
                _runningJobCache.TryRemove(timemOutJob.Key, out stopjob);
            }
        }

        public static string GetJobKey<TJob>(int siteId, string originalAccountId) where TJob : JobBase
        {
            return $"{typeof(TJob).Name} - SiteId({siteId}) - OriginalId({originalAccountId})";
        }

        public static string GetJobKey<TJob>(int siteId) where TJob : JobBase
        {
            return $"{typeof(TJob).Name} - SiteId({siteId})";
        }

        public static string GetJobGroup<TJob>() where TJob : JobBase
        {
            return typeof(TJob).Name;
        }

        public RunningJob Get<TJob>(int siteId, string originalAccountId) where TJob : JobBase
        {
            string jobKey = GetJobKey<TJob>(siteId, originalAccountId);
            RunningJob job;
            _runningJobCache.TryGetValue(jobKey, out job);
            return job;
        }

        public void Remove<TJob>(int siteId, string originalAccountId) where TJob : JobBase
        {
            string jobKey = GetJobKey<TJob>(siteId, originalAccountId);
            RunningJob removedJob;
            _runningJobCache.TryRemove(jobKey, out removedJob);
        }
    }
}
