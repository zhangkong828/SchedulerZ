using SchedulerZ.LoadBalancer.Impl.FirstLoadBalancer;
using SchedulerZ.Route;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SchedulerZ.LoadBalancer
{
    public class LoadBalancerFactory : ILoadBalancerFactory
    {
        private readonly LoadBalancerConfig _loadBalancerConfig;
        private readonly IServiceRoute _serviceRoute;
        private readonly IEnumerable<ILoadBalancerCreator> _loadBalancerCreators;

        public LoadBalancerFactory(LoadBalancerConfig config, IServiceRoute serviceRoute, IEnumerable<ILoadBalancerCreator> loadBalancerCreators)
        {
            _loadBalancerConfig = config;
            _serviceRoute = serviceRoute;
            _loadBalancerCreators = loadBalancerCreators;
        }


        public ILoadBalancer Get()
        {
            var loadBalancerType = _loadBalancerConfig.Type ?? nameof(FirstLoadBalancer);
            var loadBalancerCreator = _loadBalancerCreators.SingleOrDefault(c => c.Type == loadBalancerType);

            if (loadBalancerCreator == null)
            {
                throw new Exception($"Could not find load balancer creator for Type: {loadBalancerType}, please check your config specified the correct load balancer and that you have registered a class with the same name.");
            }

            return loadBalancerCreator.Create(_serviceRoute);
        }
    }
}
