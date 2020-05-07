using Consul;
using System;
using System.Collections.Generic;
using System.Text;

namespace SchedulerZ.Route.Consul.ClientProvider
{
    public interface IConsulClientProvider
    {
        ConsulClient GetClient();

        bool IsHealth(string host, int port);

    }
}
