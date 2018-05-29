using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Social.Job
{
    public static class JobExtensions
    {
        public static void AddCustomData<TData>(this IJobDetail job, TData data)
        {
            job.JobDataMap.Add("custom_data", data);
        }

        public static TData GetCustomData<TData>(this IJobDetail job)
        {
            var customData = job.JobDataMap.Get("custom_data");
            if (customData == null)
            {
                return default(TData);
            }

            return (TData)customData;
        }
    }
}
