using Microsoft.Extensions.Hosting;
using SchedulerZ.Utility;
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
        private readonly RegisterServiceConfig _registerServiceConfig;
        private readonly ServiceRouteDescriptor _registerSrvice;
        public ConsulHostedService(IServiceRoute serviceRoute, RegisterServiceConfig registerServiceConfig)
        {
            _serviceRoute = serviceRoute;
            _registerServiceConfig = registerServiceConfig;

            _registerSrvice = new ServiceRouteDescriptor()
            {
                Id = ObjectId.Default().NextString(),
                Name = _registerServiceConfig.Name,
                Address = _registerServiceConfig.Address,
                Port = _registerServiceConfig.Port,
                HealthCheckType = _registerServiceConfig.HealthCheckType,
                HealthCheck = _registerServiceConfig.HealthCheck
            };
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await _serviceRoute.RegisterService(_registerSrvice);
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await _serviceRoute.DeRegisterService(_registerSrvice);
        }
    }
}
