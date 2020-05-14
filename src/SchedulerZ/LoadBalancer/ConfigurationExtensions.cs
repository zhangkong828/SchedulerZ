using Microsoft.Extensions.DependencyInjection;
using SchedulerZ.LoadBalancer.Impl.FirstLoadBalancer;
using SchedulerZ.LoadBalancer.Impl.LastLoadBalancer;
using SchedulerZ.LoadBalancer.Impl.LeastConnectionLoadBalancer;
using SchedulerZ.LoadBalancer.Impl.RandomLoadBalancer;
using SchedulerZ.LoadBalancer.Impl.RoundRobinLoadBalancer;
using System;
using System.Collections.Generic;
using System.Text;

namespace SchedulerZ.LoadBalancer
{
    public static class ConfigurationExtensions
    {
        public static IServiceCollection UseLoadBalancer(this IServiceCollection services, Action<LoadBalancerConfig> loadBalancerConfig = null)
        {
            var config = new LoadBalancerConfig();
            loadBalancerConfig?.Invoke(config);

            services.AddSingleton(config);

            services.AddSingleton<ILoadBalancerCreator, FirstLoadBalancerCreator>(); //第一个
            services.AddSingleton<ILoadBalancerCreator, LastLoadBalancerCreator>();//最后一个
            services.AddSingleton<ILoadBalancerCreator, RandomLoadBalancerCreator>();//随机
            services.AddSingleton<ILoadBalancerCreator, RoundRobinLoadBalancerCreator>();//轮训
            services.AddSingleton<ILoadBalancerCreator, LeastConnectionLoadBalancerCreator>();//最小连接

            services.AddSingleton<ILoadBalancerFactory, LoadBalancerFactory>();

            return services;
        }

    }
}
