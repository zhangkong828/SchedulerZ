using SchedulerZ.Route;
using System;
using System.Collections.Generic;
using System.Text;

namespace SchedulerZ.LoadBalancer.Impl.FirstLoadBalancer
{
    public class FirstLoadBalancerCreator : ILoadBalancerCreator
    {
        public string Type => nameof(FirstLoadBalancer);

        public ILoadBalancer Create(IServiceRoute serviceRoute)
        {
            return new FirstLoadBalancer(serviceRoute);
        }
    }
}
