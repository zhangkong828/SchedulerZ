using SchedulerZ.Route;
using System;
using System.Collections.Generic;
using System.Text;

namespace SchedulerZ.LoadBalancer.Impl.LeastConnectionLoadBalancer
{
    public class LeastConnectionLoadBalancerCreator : ILoadBalancerCreator
    {
        public string Type => nameof(LeastConnectionLoadBalancer);

        public ILoadBalancer Create(IServiceRoute serviceRoute)
        {
            return new LeastConnectionLoadBalancer(serviceRoute);
        }
    }
}
