using SchedulerZ.Caching;
using System;
using System.Collections.Generic;
using System.Text;

namespace SchedulerZ.Route.Consul.Caching
{
    public class ConsulCachingProvider : ICachingProvider
    {
        private readonly ConsulCaching _consulCaching;
        public ConsulCachingProvider(ConsulCaching consulCaching)
        {
            _consulCaching = consulCaching;
        }


        public ICaching CreateCaching()
        {
            return _consulCaching;
        }
    }
}
