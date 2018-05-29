using System;
using System.Threading.Tasks;
using Quartz;

namespace Social.Job
{
    public interface IScheduleJobManager
    {
        Task ScheduleAsync<TJob, TData>(Action<JobBuilder> configureJob, Action<TriggerBuilder> configureTrigger, TData data) where TJob : IJob;
        Task ScheduleAsync<TJob>(Action<JobBuilder> configureJob, Action<TriggerBuilder> configureTrigger) where TJob : IJob;
        void Shutdown();
        void Start();
    }
}