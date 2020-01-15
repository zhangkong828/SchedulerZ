using System;
using System.Collections.Generic;
using System.Text;

namespace SchedulerZ.Component
{
    public class ObjectContainer
    {

        public static IObjectContainer Current { get; private set; }


        public static void SetContainer(IObjectContainer container)
        {
            Current = container;
        }


        public static void Build()
        {
            Current.Build();
        }

        public static void RegisterType(Type implementationType, string serviceName = null, LifeTime lifetime = LifeTime.Singleton)
        {
            Current.RegisterType(implementationType, serviceName, lifetime);
        }

        public static void RegisterType(Type serviceType, Type implementationType, string serviceName = null, LifeTime lifetime = LifeTime.Singleton)
        {
            Current.RegisterType(serviceType, implementationType, serviceName, lifetime);
        }

        public static void Register<TService, TImplementer>(string serviceName = null, LifeTime lifetime = LifeTime.Singleton)
            where TService : class
            where TImplementer : class, TService
        {
            Current.Register<TService, TImplementer>(serviceName, lifetime);
        }

        public static void RegisterInstance<TService, TImplementer>(TImplementer instance, string serviceName = null, LifeTime lifetime = LifeTime.Singleton)
           where TService : class
           where TImplementer : class, TService
        {
            Current.RegisterInstance<TService, TImplementer>(instance, serviceName, lifetime);
        }

        public static void RegisterSingle<TService, TImplementer>(string serviceName = null)
            where TService : class
            where TImplementer : class, TService
        {
            Current.RegisterSingle<TService, TImplementer>(serviceName);
        }

        public static void RegisterSingleInstance<TImplementer>(TImplementer instance)
            where TImplementer : class
        {
            Current.RegisterSingleInstance<TImplementer>(instance);
        }

        public static void RegisterSingleInstance<TService, TImplementer>(TImplementer instance, string serviceName = null)
             where TService : class
             where TImplementer : class, TService
        {
            Current.RegisterSingleInstance<TService, TImplementer>(instance, serviceName);
        }


        public static TService Resolve<TService>() where TService : class
        {
            return Current.Resolve<TService>();
        }

        public static object Resolve(Type serviceType)
        {
            return Current.Resolve(serviceType);
        }

        public static bool TryResolve<TService>(out TService instance) where TService : class
        {
            return Current.TryResolve<TService>(out instance);
        }

        public static bool TryResolve(Type serviceType, out object instance)
        {
            return Current.TryResolve(serviceType, out instance);
        }

        public static TService ResolveNamed<TService>(string serviceName) where TService : class
        {
            return Current.ResolveNamed<TService>(serviceName);
        }

        public static object ResolveNamed(string serviceName, Type serviceType)
        {
            return Current.ResolveNamed(serviceName, serviceType);
        }

        public static bool TryResolveNamed(string serviceName, Type serviceType, out object instance)
        {
            return Current.TryResolveNamed(serviceName, serviceType, out instance);
        }
    }
}
