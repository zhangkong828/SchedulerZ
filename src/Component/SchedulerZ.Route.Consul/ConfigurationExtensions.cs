using SchedulerZ.Component;
using SchedulerZ.Route.Consul.ClientProvider;
using SchedulerZ.Route.Consul.ClientProvider.Impl;
using System;
using System.Collections.Generic;
using System.Text;

namespace SchedulerZ.Route.Consul
{
    public static class ConfigurationExtensions
    {
        public static Configuration UseConsulServiceRoute(this Configuration configuration, Action<ConsulServiceRouteConfig> consulServiceRouteConfig = null)
        {
            var config = new ConsulServiceRouteConfig();
            consulServiceRouteConfig?.Invoke(config);

            Check.NotNullOrEmpty(config.Scheme, "协议");
            Check.NotNullOrEmpty(config.Host, "主机");
            Check.Positive(config.Port, "端口");

            ObjectContainer.RegisterSingleInstance(config);

            ObjectContainer.RegisterSingle<IConsulClientProvider, DefaultConsulClientProvider>();
            ObjectContainer.RegisterSingle<IServiceRoute, ConsulServiceRoute>();
            return configuration;
        }
    }
}
