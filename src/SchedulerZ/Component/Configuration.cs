using SchedulerZ.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace SchedulerZ.Component
{
    public class Configuration
    {
        public static ILoggerProvider LoggerProvider { get; private set; } = new ConsoleLoggerProvider();
        public static Configuration Instance { get; private set; }
        private Configuration() { }

        public static Configuration Create()
        {
            Instance = new Configuration();
            return Instance;
        }
        public Configuration Build()
        {
            ObjectContainer.Build();
            if (ObjectContainer.TryResolve<ILoggerProvider>(out ILoggerProvider loggerProvider))
            {
                LoggerProvider = loggerProvider;
            }
            return this;
        }

        public Configuration RegisterComponent<TService, TImplementer>(string serviceName = null, LifeTime lifetime = LifeTime.Singleton)
            where TService : class
            where TImplementer : class, TService
        {
            ObjectContainer.Register<TService, TImplementer>(serviceName, lifetime);
            return this;
        }

        public Configuration RegisterComponent<TService, TImplementer>(TImplementer instance, string serviceName = null, LifeTime lifetime = LifeTime.Singleton)
            where TService : class
            where TImplementer : class, TService
        {
            ObjectContainer.RegisterInstance<TService, TImplementer>(instance, serviceName, lifetime);
            return this;
        }

        public Configuration RegisterCommonComponents()
        {
            RegisterComponent<ILoggerProvider, ConsoleLoggerProvider>();
            return this;
        }

    }
}
