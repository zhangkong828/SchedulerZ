using SchedulerZ.Logging;
using SchedulerZ.Logging.Log4Net;
using System;
using System.Collections.Generic;
using System.Text;

namespace SchedulerZ.Component
{
    public static class ConfigurationExtensions
    {
        public static Configuration UseLog4Net(this Configuration configuration)
        {
            return UseLog4Net(configuration, "log4net.config");
        }


        public static Configuration UseLog4Net(this Configuration configuration, string configFile, string loggerRepository = "SchedulerZ.Logging.Log4Net")
        {
            ObjectContainer.RegisterInstance<ILoggerProvider, Log4NetLoggerProvider>(new Log4NetLoggerProvider(configFile, loggerRepository));
            return configuration;
        }
    }
}
