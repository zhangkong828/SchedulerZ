using Consul;
using SchedulerZ.Route.Consul.ClientProvider;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SchedulerZ.Route.Consul
{
    public class ConsulServiceRoute : IServiceRoute
    {
        private readonly ConsulServiceRouteConfig _config;
        private readonly IConsulClientProvider _consulClientProvider;
        public ConsulServiceRoute(ConsulServiceRouteConfig config, IConsulClientProvider consulClientProvider)
        {
            _config = config;
            _consulClientProvider = consulClientProvider;
        }


        public async Task<IEnumerable<ServiceRouteDescriptor>> DiscoverServices(string name)
        {
            var services = new List<ServiceRouteDescriptor>();
            var client = _consulClientProvider.GetClient();
            if (client == null) return services;
            var queryResult = await client.Health.Service(name);
            if (queryResult.StatusCode == HttpStatusCode.OK)
            {
                foreach (var item in queryResult.Response)
                {
                    services.Add(new ServiceRouteDescriptor()
                    {
                        Id = item.Service.ID,
                        Name = item.Service.Service,
                        Tags = item.Service.Tags,
                        Address = item.Service.Address,
                        Port = item.Service.Port
                    });
                }
            }
            return services;
        }

        public async Task<bool> RegisterService(ServiceRouteDescriptor service)
        {
            var client = _consulClientProvider.GetClient();
            if (client == null) return false;

            var checkId = GenCheckId(service);
            var checkName = GenCheckName(service);
            var agentCheck = new AgentCheckRegistration
            {
                ID = checkId,
                Name = checkName,
                Interval = TimeSpan.FromMilliseconds(_config.ServiceCheckInterval),
                Status = HealthStatus.Passing,
                DeregisterCriticalServiceAfter = TimeSpan.FromMilliseconds(_config.ServiceCriticalInterval),
            };

            switch (service.HealthCheckType.ToLower())
            {
                case "http":
                    agentCheck.HTTP = service.HealthCheck;
                    break;
                case "tcp":
                    agentCheck.TCP = service.HealthCheck;
                    break;
            }

            var agentService = new AgentServiceRegistration
            {
                ID = service.Id,
                Name = service.Name,
                Address = service.Address,
                Port = service.Port,
                Tags = service.Tags,
                Check = agentCheck
            };
            var response = await client.Agent.ServiceRegister(agentService);
            return response.StatusCode == HttpStatusCode.OK;
        }


        public async Task<bool> DeRegisterService(ServiceRouteDescriptor service)
        {
            var client = _consulClientProvider.GetClient();
            if (client == null) return false;
            var response = await client.Agent.ServiceDeregister(service.Id);
            return response.StatusCode == HttpStatusCode.OK;
        }


        private string GenCheckId(ServiceRouteDescriptor service)
        {
            return $"{service.Id}";
        }

        private string GenCheckName(ServiceRouteDescriptor service)
        {
            return $"{service.Name}";
        }

        public async Task<IEnumerable<ServiceRouteDescriptor>> QueryServices()
        {
            var services = new List<ServiceRouteDescriptor>();
            var client = _consulClientProvider.GetClient();
            if (client == null) return services;
            var queryResult = await client.Agent.Services();
            if (queryResult.StatusCode == HttpStatusCode.OK)
            {
                foreach (var item in queryResult.Response)
                {
                    services.Add(new ServiceRouteDescriptor()
                    {
                        Id = item.Value.ID,
                        Name = item.Value.Service,
                        Tags = item.Value.Tags,
                        Address = item.Value.Address,
                        Port = item.Value.Port
                    });
                }
            }
            return services;
        }

        public async Task<IEnumerable<NodeDescriptor>> QueryNodes()
        {
            var nodes = new List<NodeDescriptor>();
            var client = _consulClientProvider.GetClient();
            if (client == null) return nodes;
            var queryResult = await client.Catalog.Nodes();
            if (queryResult.StatusCode == HttpStatusCode.OK)
            {
                foreach (var item in queryResult.Response)
                {
                    nodes.Add(new NodeDescriptor()
                    {
                        Name = item.Name,
                        Address = item.Address
                    });
                }
            }
            return nodes;
        }
    }
}
