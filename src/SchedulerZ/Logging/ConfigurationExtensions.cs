using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace SchedulerZ.Logging
{
    public static class ConfigurationExtensions
    {
        public static IServiceCollection UseDefaultLogging(this IServiceCollection services)
        {
            services.AddSingleton<ILoggerProvider, ConsoleLoggerProvider>();

            return services;
        }
    }
}
