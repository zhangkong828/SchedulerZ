using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace SchedulerZ
{
    public class Config
    {
        public static ConfigOptions Options { get; internal set; } = new ConfigOptions();

        public static IConfigurationRoot Configuration { get; internal set; }

        public static string GetValue(string key)
        {
            return GetValue<string>(key);
        }

        public static T GetValue<T>(string key)
        {
            return Configuration.GetValue<T>(key);
        }

        public static bool IsExists(string key)
        {
            return Configuration.GetSection(key).Exists();
        }

    }
}
