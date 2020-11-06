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
            var builder = new ConfigurationBuilder()
             .SetBasePath(AppContext.BaseDirectory)
             .AddJsonFile(path, optional: optional, reloadOnChange: reloadOnChange);

            Configuration = builder.Build();
        }
    }
}
