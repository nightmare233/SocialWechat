using Common.Logging;
using Framework.Core;
using Quartz;
using Quartz.Impl;
using System;
using System.Threading.Tasks;

namespace Social.Job
{
    public class ScheduleJobManager : IScheduleJobManager, ITransient
    {
        private IScheduler _scheduler;
        private readonly ILog _log = SchedulerLogger.GetLogger();

        public ScheduleJobManager(IDependencyResolver dependencyResolver)
        {
            _scheduler = StdSchedulerFactory.GetDefaultScheduler();
            _scheduler.JobFactory = new JobFactory(dependencyResolver);
        }

        public Task ScheduleAsync<TJob>(Action<JobBuilder> configureJob, Action<TriggerBuilder> configureTrigger) where TJob : IJob
        {
            var jobToBuild = JobBuilder.Create<TJob>();
            configureJob(jobToBuild);
            var job = jobToBuild.Build();

            var triggerToBuild = TriggerBuilder.Create();
            configureTrigger(triggerToBuild);
            var trigger = triggerToBuild.Build();

            _scheduler.ScheduleJob(job, trigger);

            return Task.FromResult(0);
        }

        public Task ScheduleAsync<TJob, TData>(Action<JobBuilder> configureJob, Action<TriggerBuilder> configureTrigger, TData data) where TJob : IJob
        {
            var jobToBuild = JobBuilder.Create<TJob>();
            configureJob(jobToBuild);
            var job = jobToBuild.Build();

            var triggerToBuild = TriggerBuilder.Create();
            configureTrigger(triggerToBuild);
            var trigger = triggerToBuild.Build();

            job.AddCustomData(data);
            _scheduler.ScheduleJob(job, trigger);

            return Task.FromResult(0);
        }

        public void Start()
        {
            _scheduler.Start();
            _log.Info("------- Started Scheduler -----------------");
        }

        public void Shutdown()
        {
            _log.Info("------- Shutting Down ---------------------");

            try
            {
                _scheduler.Shutdown(true);
            }
            catch (Exception ex)
            {
                _log.Error(ex);
            }

            _log.Info("------- Shutdown Complete -----------------");
        }
    }
}
