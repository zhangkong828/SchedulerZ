using SchedulerZ.Route;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulerZ.LoadBalancer.Impl.RandomLoadBalancer
{
    public class RandomLoadBalancer : ILoadBalancer
    {
        private readonly IServiceRoute _serviceRoute;
        private readonly Random _random;
        public RandomLoadBalancer(IServiceRoute serviceRoute)
        {
            _random = new Random();
            _serviceRoute = serviceRoute;
        }

        public async Task<ServiceRouteDescriptor> Lease(string serviceName)
        {
            var services = await _serviceRoute.DiscoverServices(serviceName);
            var index = _random.Next(services.Count());
            return services.ElementAtOrDefault(index);
        }

        public void Release(ServiceRouteDescriptor serviceRouteDescriptor)
        {

        }
    }
}
