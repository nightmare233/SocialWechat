using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Social.Infrastructure
{
    public static class Logger
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(Logger));

        public static void Error(Exception ex)
        {
            logger.Error(ex);
        }

        public static void Error(string message, Exception ex)
        {
            logger.Error(message, ex);
        }
    }
}
