using SchedulerZ.Caching;
using SchedulerZ.Route.Consul.ClientProvider;
using System;
using System.Collections.Generic;
using System.Text;

namespace SchedulerZ.Route.Consul.Caching
{
    public class ConsulCaching : ICaching
    {
        private readonly IConsulClientProvider _consulClientProvider;
        public ConsulCaching(IConsulClientProvider consulClientProvider)
        {
            _consulClientProvider = consulClientProvider;
        }

        public bool Exists(string key)
        {
            var client = _consulClientProvider.GetClient();
            if (client == null) return false;

            throw new NotImplementedException();
        }

        public string Get(string key)
        {
            throw new NotImplementedException();
        }

        public T Get<T>(string key)
        {
            throw new NotImplementedException();
        }

        public bool Remove(string key)
        {
            throw new NotImplementedException();
        }

        public bool Set(string key, object data, TimeSpan? expiry = null)
        {
            throw new NotImplementedException();
        }
    }
}
