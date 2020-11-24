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
        public static IServiceCollection UseConsulServiceRoute(this IServiceCollection services, Action<ConsulServiceRouteConfig> configDelegate = null, Action<RegisterServiceConfig> registerServiceDelegate = null)
        {
            var config = Config.Get<ConsulServiceRouteConfig>("ConsulServiceRoute") ?? new ConsulServiceRouteConfig();
            configDelegate?.Invoke(config);

            Check.NotNullOrEmpty(config.Scheme, "协议");
            Check.NotNullOrEmpty(config.Host, "主机");
            Check.Positive(config.Port, "端口");

            services.AddSingleton(config);
            services.AddSingleton<IConsulClientProvider, DefaultConsulClientProvider>();
            services.AddSingleton<IServiceRoute, ConsulServiceRoute>();


            //是否有需要注册的服务
            if (Config.IsExists("ConsulServiceRoute:RegisterService") || registerServiceDelegate != null)
            {
                var registerServiceConfig = Config.Get<RegisterServiceConfig>("ConsulServiceRoute:RegisterService");
                registerServiceDelegate?.Invoke(registerServiceConfig);

                Check.NotNullOrEmpty(registerServiceConfig.Name, "服务名称");
                Check.NotNullOrEmpty(registerServiceConfig.Address, "地址");
                Check.Positive(registerServiceConfig.Port, "端口");
                Check.NotNullOrEmpty(registerServiceConfig.HealthCheckType, "健康检查类型");
                Check.NotNullOrEmpty(registerServiceConfig.HealthCheck, "健康检查地址");

                services.AddSingleton(registerServiceConfig);
                services.AddHostedService<ConsulHostedService>();

                Config.NodeHost = registerServiceConfig.Address;
                Config.NodePort = registerServiceConfig.Port;
            }

            return services;
        }

    }
}
