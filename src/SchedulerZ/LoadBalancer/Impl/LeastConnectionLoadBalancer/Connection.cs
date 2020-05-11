using SchedulerZ.Route;
using System;
using System.Collections.Generic;
using System.Text;

namespace SchedulerZ.LoadBalancer.Impl.LeastConnectionLoadBalancer
{
    public class Connection
    {
        public Connection(ServiceRouteDescriptor service, long connections)
        {
            Service = service;
            Connections = connections;
        }

        public ServiceRouteDescriptor Service { get; set; }
        public long Connections { get; set; }
    }
}
