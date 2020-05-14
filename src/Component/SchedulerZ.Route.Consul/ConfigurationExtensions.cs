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
        public static IServiceCollection UseConsulServiceRoute(this IServiceCollection services, Action<ConsulServiceRouteConfig> consulServiceRouteConfig = null)
        {
            var config = new ConsulServiceRouteConfig();
            consulServiceRouteConfig?.Invoke(config);

            Check.NotNullOrEmpty(config.Scheme, "协议");
            Check.NotNullOrEmpty(config.Host, "主机");
            Check.Positive(config.Port, "端口");

            services.AddSingleton(config);
            services.AddSingleton<IConsulClientProvider, DefaultConsulClientProvider>();
            services.AddSingleton<IServiceRoute, ConsulServiceRoute>();

            return services;
        }

    }
}
