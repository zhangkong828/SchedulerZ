using SchedulerZ.Route;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulerZ.LoadBalancer.Impl.FirstLoadBalancer
{
    public class FirstLoadBalancer : ILoadBalancer
    {
        private readonly IServiceRoute _serviceRoute;
        public FirstLoadBalancer(IServiceRoute serviceRoute)
        {
            _serviceRoute = serviceRoute;
        }

        public async Task<ServiceRouteDescriptor> Lease(string serviceName)
        {
            var services = await _serviceRoute.DiscoverServices(serviceName);
            return await Task.FromResult(services.FirstOrDefault());
        }

        public void Release(ServiceRouteDescriptor serviceRouteDescriptor)
        {
            
        }
    }
}
