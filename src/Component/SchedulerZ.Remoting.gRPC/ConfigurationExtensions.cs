﻿using Microsoft.Extensions.DependencyInjection;
using SchedulerZ.gRPC;
using System;
using System.Collections.Generic;
using System.Text;

namespace SchedulerZ.Remoting.gRPC
{
    public static class ConfigurationExtensions
    {
        public static IServiceCollection UseGrpcRemoting(this IServiceCollection services, Action<GrpcServiceConfig> configDelegate = null)
        {
            var config = Config.Get<GrpcServiceConfig>("GrpcService") ?? new GrpcServiceConfig();
            configDelegate?.Invoke(config);

            Check.NotNullOrEmpty(config.Host, "主机");
            Check.Positive(config.Port, "端口");

            services.AddSingleton(config);

            services.AddSingleton<SchedulerService.SchedulerServiceBase, SchedulerServiceImpl>();
            services.AddSingleton<FileService.FileServiceBase, FileServiceImpl>();
            services.AddHostedService<GrpcHostedService>();
            services.AddHostedService<FileHostedService>();

            return services;
        }
    }
}
