using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SchedulerZ.Manager.API
{
    public static class DateTimeExtension
    {
        /// <summary>
        /// 转换时间为unix时间戳
        /// </summary>
        /// <param name="date"></param>
        /// <param name="accurateToMilliseconds">是否精确到毫秒</param>
        /// <returns></returns>
        public static long ConvertToUnixOfTime(this DateTime date, bool accurateToMilliseconds = true)
        {
            var startTime = TimeZoneInfo.ConvertTimeFromUtc(new DateTime(1970, 1, 1), TimeZoneInfo.Local);
            if (accurateToMilliseconds)
            {
                return (long)(date - startTime).TotalMilliseconds;

            }
            else
            {
                return (long)(date - startTime).TotalSeconds;
            }
        }
    }
}
