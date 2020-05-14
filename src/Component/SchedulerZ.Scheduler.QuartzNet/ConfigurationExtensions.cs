using Microsoft.Extensions.DependencyInjection;
using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
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

            services.AddSingleton(provider =>
            {
                NameValueCollection properties = new NameValueCollection();
                properties["quartz.scheduler.instanceName"] = "SchedulerZ.SchedulerManager";
                properties["quartz.threadPool.type"] = "Quartz.Simpl.SimpleThreadPool, Quartz";
                properties["quartz.threadPool.threadCount"] = "50";
                properties["quartz.threadPool.threadPriority"] = "Normal";

                ISchedulerFactory factory = new StdSchedulerFactory(properties);
                var scheduler = factory.GetScheduler().GetAwaiter().GetResult();
                return scheduler;
            });
            services.AddHostedService<QuartzNetHostedService>();

            services.AddSingleton<ISchedulerManager, SchedulerManager>();

            return services;
        }
    }
}
