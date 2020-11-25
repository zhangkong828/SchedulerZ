using Microsoft.Extensions.DependencyInjection;
using SchedulerZ.Caching.Default;
using System;
using System.Collections.Generic;
using System.Text;

namespace SchedulerZ.Caching
{
    public static class ConfigurationExtensions
    {
        public static IServiceCollection UseDefaultCaching(this IServiceCollection services)
        {
            services.AddSingleton<ICachingProvider, MemoryCacheProvider>();

            return services;
        }
    }
}
