using SchedulerZ.Route;
using System;
using System.Collections.Generic;
using System.Text;

namespace SchedulerZ.LoadBalancer.Impl.RandomLoadBalancer
{
    public class RandomLoadBalancerCreator : ILoadBalancerCreator
    {
        public string Type => nameof(RandomLoadBalancer);

        public ILoadBalancer Create(IServiceRoute serviceRoute)
        {
            return new RandomLoadBalancer(serviceRoute);
        }
    }
}
