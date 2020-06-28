using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SchedulerZ.Manager.API
{
    public class CacheKey
    {
        public static string Token(string userId)
        {
            return $"SchedulerZ:Token:{userId}";
        }
    }
}
