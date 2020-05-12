using SchedulerZ.Route;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using System.Threading;

namespace SchedulerZ.LoadBalancer.Impl.RoundRobinLoadBalancer
{
    public class RoundRobinLoadBalancer : ILoadBalancer
    {
        private readonly IServiceRoute _serviceRoute;

        private readonly object _lock = new object();

        private int _last;
        public RoundRobinLoadBalancer(IServiceRoute serviceRoute)
        {
            _serviceRoute = serviceRoute;
        }

        public async Task<ServiceRouteDescriptor> Lease(string serviceName)
        {
            var services = await _serviceRoute.DiscoverServices(serviceName);
            lock (_lock)
            {
                if (_last >= services.Count())
                {
                    _last = 0;
                }

                var next = services.ElementAtOrDefault(_last);
                //_last++;
                Interlocked.Increment(ref _last);
                return next;
            }
        }

        public void Release(ServiceRouteDescriptor serviceRouteDescriptor)
        {

        }
    }
}
