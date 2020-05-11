using SchedulerZ.Route;
using System;
using System.Collections.Generic;
using System.Text;

namespace SchedulerZ.LoadBalancer
{
    public interface ILoadBalancerCreator
    {
        ILoadBalancer Create(IServiceRoute serviceRoute);
        string Type { get; }
    }
}
