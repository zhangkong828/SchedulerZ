using Microsoft.Extensions.Configuration;
using SchedulerZ.Configurations;
using System;
using System.Collections.Generic;
using System.Text;

namespace SchedulerZ
{
    public class Config
    {
        public static LoggerOptions LoggerOptions { get; set; } = new LoggerOptions();
        public static ConfigOptions Options { get; set; } = new ConfigOptions();
        public static DbConnector DbConnector { get; set; }

        public static string NodeHost { get; set; }
        public static int NodePort { get; set; }


        public static IConfigurationRoot Configuration { get; set; }

        public static string GetValue(string key, string defaultValue = null)
        {
            return GetValue<string>(key, defaultValue);
        }

        public static bool GetValue(string key, bool defaultValue = false)
        {
            return GetValue<bool>(key, defaultValue);
        }

        public static int GetValue(string key, int defaultValue = 0)
        {
            return GetValue<int>(key, defaultValue);
        }

        public static long GetValue(string key, long defaultValue = 0)
        {
            return GetValue<long>(key, defaultValue);
        }

        public static double GetValue(string key, double defaultValue = 0)
        {
            return GetValue<double>(key, defaultValue);
        }

        public static T GetValue<T>(string key, T defaultValue)
        {
            if (Configuration == null) return default;
            return Configuration.GetValue<T>(key, defaultValue);
        }

        public static T Get<T>(string key) where T : class
        {
            if (Configuration == null) return default;
            return Configuration.GetSection(key).Get<T>();
        }

        public static bool IsExists(string key)
        {
            if (Configuration == null) return false;
            return Configuration.GetSection(key).Exists();
        }

    }
}
