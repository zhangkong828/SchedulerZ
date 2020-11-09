using Microsoft.Extensions.DependencyInjection;
using SchedulerZ.Route.Consul.ClientProvider;
using SchedulerZ.Route.Consul.ClientProvider.Impl;
using System;
using System.Collections.Generic;
using System.Text;

namespace SchedulerZ.Route.Consul
{
    public static class ConfigurationExtensions
    {
        public static IServiceCollection UseConsulServiceRoute(this IServiceCollection services, Action<ConsulServiceRouteConfig> configDelegate = null, Action<ServiceRouteDescriptor> registerServiceDelegate = null)
        {
            var config = Config.GetValue<ConsulServiceRouteConfig>("ConsulServiceRoute");
            configDelegate?.Invoke(config);

            Check.NotNullOrEmpty(config.Scheme, "协议");
            Check.NotNullOrEmpty(config.Host, "主机");
            Check.Positive(config.Port, "端口");

            services.AddSingleton(config);
            services.AddSingleton<IConsulClientProvider, DefaultConsulClientProvider>();
            services.AddSingleton<IServiceRoute, ConsulServiceRoute>();

            if (registerServiceDelegate != null)
            {
                var serviceRouteDescriptor = new ServiceRouteDescriptor();
                registerServiceDelegate.Invoke(serviceRouteDescriptor);

                Check.NotNullOrEmpty(serviceRouteDescriptor.Id, "服务Id");
                Check.NotNullOrEmpty(serviceRouteDescriptor.Name, "服务名称");
                Check.NotNullOrEmpty(serviceRouteDescriptor.Address, "地址");
                Check.Positive(serviceRouteDescriptor.Port, "端口");
                Check.NotNullOrEmpty(serviceRouteDescriptor.HealthCheckType, "健康检查类型");
                Check.NotNullOrEmpty(serviceRouteDescriptor.HealthCheck, "健康检查地址");

                services.AddSingleton(serviceRouteDescriptor);
                services.AddHostedService<ConsulHostedService>();
            }

            return services;
        }

    }
}
