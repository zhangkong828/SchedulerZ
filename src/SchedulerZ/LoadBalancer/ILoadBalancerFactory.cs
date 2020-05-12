using SchedulerZ.Route;
using System;
using System.Collections.Generic;
using System.Text;

namespace SchedulerZ.LoadBalancer
{
    interface ILoadBalancerFactory
    {
        ILoadBalancer Get();
    }
}
