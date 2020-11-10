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
            var config = Config.Get<ConnectionConfig>("MySqlStore") ?? new ConnectionConfig();
            configDelegate?.Invoke(config);

            Check.NotNullOrEmpty(config.ConnectionString, "连接字符串");

            Config.DbConnector = new DbConnector()
            {
                Provider = DbProvider.MySQL,
                ConnectionString = config.ConnectionString
            };
            
            services.AddDbContext<SchedulerZContext>();

            services.AddScoped<IJobStore, JobStoreService>();
            services.AddScoped<IAccountStore, AccountStoreService>();

            using (var ctx = new SchedulerZContext())
            {
                DbInitializer.Initialize(ctx);
            }

            return services;
        }

    }
}
