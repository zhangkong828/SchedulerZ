using SchedulerZ.Route;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulerZ.LoadBalancer.Impl.LeastConnectionLoadBalancer
{
    public class LeastConnectionLoadBalancer : ILoadBalancer
    {
        private readonly IServiceRoute _serviceRoute;

        private readonly ConcurrentDictionary<ServiceRouteDescriptor, long> _dic;

        private static readonly object _syncLock = new object();

        public LeastConnectionLoadBalancer(IServiceRoute serviceRoute)
        {
            _dic = new ConcurrentDictionary<ServiceRouteDescriptor, long>();
            _serviceRoute = serviceRoute;
        }

        public async Task<ServiceRouteDescriptor> Lease(string serviceName)
        {
            var services = await _serviceRoute.DiscoverServices(serviceName);

            lock (_syncLock)
            {
                UpdateServices(services);
                var leastConnection = GetLeastConnection(serviceName);
                return leastConnection;
            }
        }

        public void Release(ServiceRouteDescriptor serviceRouteDescriptor)
        {
            lock (_syncLock)
            {
                if (_dic.ContainsKey(serviceRouteDescriptor))
                {
                    var count = _dic[serviceRouteDescriptor];
                    _dic.TryUpdate(serviceRouteDescriptor, count - 1, count);
                }
            }
        }


        private void UpdateServices(IEnumerable<ServiceRouteDescriptor> services)
        {
            if (_dic.Count > 0)
            {
                var toRemove = new List<ServiceRouteDescriptor>();

                foreach (var item in _dic)
                {
                    var match = services.FirstOrDefault(s => s.Equals(item.Key));

                    if (match == null)
                    {
                        toRemove.Add(item.Key);
                    }
                }

                foreach (var item in toRemove)
                {
                    _dic.TryRemove(item, out long count);
                }

                foreach (var service in services)
                {
                    var exists = _dic.FirstOrDefault(d => d.Key.Equals(service)).Key;

                    if (exists == null)
                    {
                        _dic.TryAdd(service, 0);
                    }
                }
            }
            else
            {
                foreach (var service in services)
                {
                    _dic.TryAdd(service, 0);
                }
            }

        }



        private ServiceRouteDescriptor GetLeastConnection(string serviceName)
        {
            var connection = _dic.Where(x => x.Key.Name == serviceName).OrderBy(x => x.Value).FirstOrDefault();

            _dic.TryUpdate(connection.Key, connection.Value + 1, connection.Value);
            return connection.Key;
        }
    }
}
