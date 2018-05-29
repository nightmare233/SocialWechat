using Common.Logging;
using System;

namespace Social.Job
{
    public class SchedulerLogger
    {
        private static Lazy<ILog> Logger = new Lazy<ILog>(LoggerFactory);

        public static ILog GetLogger()
        {
            return Logger.Value;
        }

        private static ILog LoggerFactory()
        {
            return LogManager.GetLogger("Social.Job");
        }
    }
}
