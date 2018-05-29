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
        public class RunningJob
        {
            public string JobKey { get; set; }
            public string JobGroup { get; set; }
            public int SiteId { get; set; }
            public string OriginalAccountId { get; set; }
            public DateTime LastScheduleTime { get; set; }

            public bool IsTimeout
            {
                get
                {
                    return (DateTime.UtcNow - LastScheduleTime).TotalSeconds > 300;
                }
            }
        }
    }
}
