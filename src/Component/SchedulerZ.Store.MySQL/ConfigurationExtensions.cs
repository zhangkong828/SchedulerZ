using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SchedulerZ.Store.MySQL.Impl;
using System;
using System.Collections.Generic;
using System.Text;

namespace SchedulerZ.Store.MySQL
{
    public static class ConfigurationExtensions
    {
        public static IServiceCollection UseMySQL(this IServiceCollection services, Action<ConnectionConfig> configDelegate = null)
        {
            var config = new ConnectionConfig();
            configDelegate?.Invoke(config);

            Check.NotNullOrEmpty(config.ConnectionString, "连接字符串");

            services.AddSingleton(config);

            services.AddDbContext<SchedulerZContext>(options => options.UseMySQL(config.ConnectionString));

            services.AddSingleton<IJobStore, JobStoreService>();
            services.AddSingleton<IAccountStore, AccountStoreService>();

            return services;
        }

    }
}
