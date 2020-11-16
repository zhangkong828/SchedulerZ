﻿using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace SchedulerZ
{
    public class Config
    {
        public static ConfigOptions Options { get; internal set; } = new ConfigOptions();
        public static DbConnector DbConnector { get; set; }


        public static IConfigurationRoot Configuration { get; internal set; }

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
            return Configuration.GetValue<T>(key, defaultValue);
        }

        public static T Get<T>(string key) where T : class
        {
            return Configuration.GetSection(key).Get<T>();
        }

        public static bool IsExists(string key)
        {
            return Configuration.GetSection(key).Exists();
        }

    }
}