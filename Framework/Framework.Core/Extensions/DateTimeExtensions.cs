using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework.Core
{
    public static class DateTimeExtensions
    {
        public static DateTime FromUnixTimeSeconds(long unixTimeStamp)
        {
            DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            epoch = epoch.AddSeconds(unixTimeStamp);
            return epoch;
        }

        public static long ToUnixTimeSeconds(this DateTime datetime)
        {
            DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            return Convert.ToInt64((datetime.ToUniversalTime() - epoch).TotalSeconds);
        }

        public static long? ToUnixTimeSeconds(this DateTime? datetime)
        {
            if (datetime == null)
            {
                return null;
            }

            return datetime.Value.ToUnixTimeSeconds();
        }


        public static DateTime Normalize(this DateTime dateTime)
        {
            if (dateTime.Kind == DateTimeKind.Unspecified)
            {
                return DateTime.SpecifyKind(dateTime, DateTimeKind.Utc);
            }

            if (dateTime.Kind == DateTimeKind.Local)
            {
                return dateTime.ToUniversalTime();
            }

            return dateTime;
        }

        public static DateTime? Normalize(this DateTime? dateTime)
        {
            if (dateTime.HasValue)
            {
                if (dateTime.Value.Kind == DateTimeKind.Unspecified)
                {
                    return DateTime.SpecifyKind(dateTime.Value, DateTimeKind.Utc);
                }

                if (dateTime.Value.Kind == DateTimeKind.Local)
                {
                    return dateTime.Value.ToUniversalTime();
                }
            }

            return dateTime;
        }
    }
}
