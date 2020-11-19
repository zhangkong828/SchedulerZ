using CSRedis;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace SchedulerZ.Caching.Redis
{
    public static class ConfigurationExtensions
    {
        public static IServiceCollection UseRedisCache(this IServiceCollection services, Action<RedisCacheConfig> configDelegate = null)
        {
            var config = Config.Get<RedisCacheConfig>("RedisCache") ?? new RedisCacheConfig();
            configDelegate?.Invoke(config);

            Check.NotNullOrEmpty(config.Connection, "地址");

            services.AddSingleton(config);

            var RedisClient = new CSRedisClient(config.Connection);
            services.AddSingleton(_ => RedisClient);

            services.AddSingleton<ICaching, RedisCache>();
            services.AddSingleton<ICachingProvider, RedisCacheProvider>();

            return services;
        }
    }
}
