using SchedulerZ.Route;
using System;
using System.Collections.Generic;
using System.Text;

namespace SchedulerZ.LoadBalancer.Impl.RoundRobinLoadBalancer
{
    public class RoundRobinLoadBalancerCreator : ILoadBalancerCreator
    {
        public string Type => nameof(RoundRobinLoadBalancer);

        public ILoadBalancer Create(IServiceRoute serviceRoute)
        {
            return new RoundRobinLoadBalancer(serviceRoute);
        }
    }
}
