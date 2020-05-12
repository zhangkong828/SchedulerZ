using SchedulerZ.Component;
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
        public static Configuration UseLoadBalancer(this Configuration configuration, Action<LoadBalancerConfig> loadBalancerConfig = null)
        {
            var config = new LoadBalancerConfig();
            loadBalancerConfig?.Invoke(config);

            ObjectContainer.RegisterSingleInstance(config);

            ObjectContainer.RegisterSingle<ILoadBalancerCreator, FirstLoadBalancerCreator>(); //第一个
            ObjectContainer.RegisterSingle<ILoadBalancerCreator, LastLoadBalancerCreator>();//最后一个
            ObjectContainer.RegisterSingle<ILoadBalancerCreator, RandomLoadBalancerCreator>();//随机
            ObjectContainer.RegisterSingle<ILoadBalancerCreator, RoundRobinLoadBalancerCreator>();//轮训
            ObjectContainer.RegisterSingle<ILoadBalancerCreator, LeastConnectionLoadBalancerCreator>();//最小连接

            ObjectContainer.RegisterSingle<ILoadBalancerFactory, LoadBalancerFactory>();
            return configuration;
        }
    }
}
