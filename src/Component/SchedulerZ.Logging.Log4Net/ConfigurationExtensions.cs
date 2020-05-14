using Microsoft.Extensions.DependencyInjection;
using SchedulerZ.Logging;
using SchedulerZ.Logging.Log4Net;
using System;
using System.Collections.Generic;
using System.Text;

namespace SchedulerZ.Logging.Log4Net
{
    public static class ConfigurationExtensions
    {
        public static IServiceCollection UseLog4Net(this IServiceCollection services)
        {
            services.AddSingleton<ILoggerProvider, Log4NetLoggerProvider>(provider => { return new Log4NetLoggerProvider("log4net.config", "SchedulerZ.Logging.Log4Net"); });

            return services;
        }

        public static IServiceCollection UseLog4Net(this IServiceCollection services, string configFile, string loggerRepository = "SchedulerZ.Logging.Log4Net")
        {
            services.AddSingleton<ILoggerProvider, Log4NetLoggerProvider>(provider => { return new Log4NetLoggerProvider(configFile, loggerRepository); });

            return services;
        }

    }
}
