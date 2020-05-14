using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace SchedulerZ.Scheduler.QuartzNet
{
    public static class ConfigurationExtensions
    {
        public static IServiceCollection UseQuartzNetScheduler(this IServiceCollection services, Action<QuartzNetConfig> quartzNetConfig = null)
        {
            var config = new QuartzNetConfig();
            quartzNetConfig?.Invoke(config);

            services.AddSingleton(config);

            services.AddSingleton<ISchedulerManager, SchedulerManager>();

            return services;
        }
    }
}
