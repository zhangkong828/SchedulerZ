using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SchedulerZ.Caching;
using SchedulerZ.LoadBalancer;
using System;
using System.Collections.Generic;
using System.Text;

namespace SchedulerZ.Configurations
{
    public static class ConfigurationExtensions
    {
        public static IConfigurationBuilder AddConfigFile(this IConfigurationBuilder builder, string path, bool optional, bool reloadOnChange)
        {
            builder.AddJsonFile(path, optional: optional, reloadOnChange: reloadOnChange);

            var configurationBuilder = new ConfigurationBuilder()
             .SetBasePath(AppContext.BaseDirectory)
             .AddJsonFile(path, optional: optional, reloadOnChange: reloadOnChange);

            Config.Configuration = configurationBuilder.Build();
            Config.Options = Config.Get<ConfigOptions>("SchedulerZ") ?? new ConfigOptions();
            Config.LoggerOptions = Config.Get<LoggerOptions>("SchedulerZ:Logger") ?? new LoggerOptions();
            return builder;
        }

        public static IServiceCollection UseSchedulerZ(this IServiceCollection services)
        {
            services.UseDefaultCaching();
            services.UseLoadBalancer();

            return services;
        }
    }
}
