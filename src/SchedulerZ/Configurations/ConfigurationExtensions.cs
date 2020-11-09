using Microsoft.Extensions.Configuration;
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
            if (Config.IsExists("SchedulerZ"))
                Config.Options = Config.GetValue<ConfigOptions>("SchedulerZ");

            return builder;
        }
    }
}
