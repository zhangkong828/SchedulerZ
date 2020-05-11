using SchedulerZ.Route;
using System;
using System.Collections.Generic;
using System.Text;

namespace SchedulerZ.LoadBalancer.Impl.LastLoadBalancer
{
    public class LastLoadBalancerCreator : ILoadBalancerCreator
    {
        public string Type => nameof(LastLoadBalancer);

        public ILoadBalancer Create(IServiceRoute serviceRoute)
        {
            return new LastLoadBalancer(serviceRoute);
        }
    }
}
