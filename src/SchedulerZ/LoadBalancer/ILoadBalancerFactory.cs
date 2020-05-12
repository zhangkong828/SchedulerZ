using SchedulerZ.Route;
using System;
using System.Collections.Generic;
using System.Text;

namespace SchedulerZ.LoadBalancer
{
    public interface ILoadBalancerFactory
    {
        ILoadBalancer Get();
    }
}
