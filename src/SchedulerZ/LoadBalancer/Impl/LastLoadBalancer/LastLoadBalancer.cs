using SchedulerZ.Route;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulerZ.LoadBalancer.Impl.LastLoadBalancer
{
    public class LastLoadBalancer : ILoadBalancer
    {
        private readonly IServiceRoute _serviceRoute;
        public LastLoadBalancer(IServiceRoute serviceRoute)
        {
            _serviceRoute = serviceRoute;
        }

        public async Task<ServiceRouteDescriptor> Lease(string serviceName)
        {
            var services = await _serviceRoute.DiscoverServices(serviceName);
            return await Task.FromResult(services.LastOrDefault());
        }

        public void Release(ServiceRouteDescriptor serviceRouteDescriptor)
        {

        }
    }
}
