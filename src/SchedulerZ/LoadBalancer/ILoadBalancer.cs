using SchedulerZ.Route;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SchedulerZ.LoadBalancer
{
    public interface ILoadBalancer
    {
        Task<ServiceRouteDescriptor> Lease(string serviceName);

        void Release(ServiceRouteDescriptor serviceRouteDescriptor);
    }
}
