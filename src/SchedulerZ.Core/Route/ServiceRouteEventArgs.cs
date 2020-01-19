using System;
using System.Collections.Generic;
using System.Text;

namespace SchedulerZ.Core.Route
{
    public class ServiceRouteEventArgs
    {
        public ServiceRouteEventArgs(ServiceRouteDescriptor route)
        {
            Route = route;
        }

        public ServiceRouteDescriptor Route { get; private set; }
    }

    public class ServiceRouteChangedEventArgs : ServiceRouteEventArgs
    {
        public ServiceRouteChangedEventArgs(ServiceRouteDescriptor route, ServiceRouteDescriptor oldRoute) : base(route)
        {
            OldRoute = oldRoute;
        }

        public ServiceRouteDescriptor OldRoute { get; set; }
    }
}
