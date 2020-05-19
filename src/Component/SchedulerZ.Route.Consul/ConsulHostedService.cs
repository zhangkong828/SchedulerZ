using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SchedulerZ.Route.Consul
{
    public class ConsulHostedService : IHostedService
    {
        private readonly IServiceRoute _serviceRoute;
        private readonly ServiceRouteDescriptor _service;
        public ConsulHostedService(IServiceRoute serviceRoute, ServiceRouteDescriptor service)
        {
            _serviceRoute = serviceRoute;
            _service = service;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await _serviceRoute.RegisterService(_service);
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await _serviceRoute.DeregisterService(_service);
        }
    }
}
