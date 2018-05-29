using Quartz.Spi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Quartz;
using Framework.Core;

namespace Social.Job
{
    public class JobFactory : IJobFactory
    {
        private readonly IDependencyResolver _dependencyResolver;

        public JobFactory(IDependencyResolver dependencyResolver)
        {
            _dependencyResolver = dependencyResolver;
        }

        public IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler)
        {
            return (IJob)_dependencyResolver.Resolve(bundle.JobDetail.JobType);
        }

        public void ReturnJob(IJob job)
        {
            _dependencyResolver.Release(job);
        }
    }
}
