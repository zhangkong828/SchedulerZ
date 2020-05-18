using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace SchedulerZ.Remoting.gRPC.Client
{
    public static class ConfigurationExtensions
    {
        public static IServiceCollection UseGrpcRemotingClient(this IServiceCollection services, Action<GrpcClientConfig> grpcClientConfig = null)
        {
            var config = new GrpcClientConfig();
            grpcClientConfig?.Invoke(config);


            services.AddSingleton(config);

            services.AddSingleton<IEndpointStrategy, EndpointStrategy>();
            services.AddSingleton(typeof(IGrpcClientFactory<>), typeof(GrpcClientFactory<>));

            services.AddSingleton<ISchedulerRemoting, SchedulerRemoting>();

            return services;
        }
    }
}
