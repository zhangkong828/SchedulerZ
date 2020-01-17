using Autofac;
using System;
using System.Collections.Generic;
using System.Text;

namespace SchedulerZ.Component.Autofac
{
    public class AutofacObjectContainer : IObjectContainer
    {
        public AutofacObjectContainer() : this(new ContainerBuilder())
        {
        }

        public AutofacObjectContainer(ContainerBuilder containerBuilder)
        {
            ContainerBuilder = containerBuilder;
        }


        public ContainerBuilder ContainerBuilder { get; }

        public IContainer Container { get; private set; }

        public void Build()
        {
            Container = ContainerBuilder.Build();
        }


        public void RegisterType(Type implementationType, string serviceName = null, LifeTime lifetime = LifeTime.Singleton)
        {
            if (implementationType.IsGenericType)
            {
                var registrationBuilder = ContainerBuilder.RegisterGeneric(implementationType);
                if (serviceName != null)
                {
                    registrationBuilder.Named(serviceName, implementationType);
                }
                if (lifetime == LifeTime.Singleton)
                {
                    registrationBuilder.SingleInstance();
                }
            }
            else
            {
                var registrationBuilder = ContainerBuilder.RegisterType(implementationType);
                if (serviceName != null)
                {
                    registrationBuilder.Named(serviceName, implementationType);
                }
                if (lifetime == LifeTime.Singleton)
                {
                    registrationBuilder.SingleInstance();
                }
            }
        }

        public void RegisterType(Type serviceType, Type implementationType, string serviceName = null, LifeTime lifetime = LifeTime.Singleton)
        {
            if (implementationType.IsGenericType)
            {
                var registrationBuilder = ContainerBuilder.RegisterGeneric(implementationType).As(serviceType);
                if (serviceName != null)
                {
                    registrationBuilder.Named(serviceName, implementationType);
                }
                if (lifetime == LifeTime.Singleton)
                {
                    registrationBuilder.SingleInstance();
                }
            }
            else
            {
                var registrationBuilder = ContainerBuilder.RegisterType(implementationType).As(serviceType);
                if (serviceName != null)
                {
                    registrationBuilder.Named(serviceName, serviceType);
                }
                if (lifetime == LifeTime.Singleton)
                {
                    registrationBuilder.SingleInstance();
                }
            }
        }

        public void Register<TService, TImplementer>(string serviceName = null, LifeTime lifetime = LifeTime.Singleton)
            where TService : class
            where TImplementer : class, TService
        {
            var registrationBuilder = ContainerBuilder.RegisterType<TImplementer>().As<TService>();
            if (serviceName != null)
            {
                registrationBuilder.Named<TService>(serviceName);
            }
            if (lifetime == LifeTime.Singleton)
            {
                registrationBuilder.SingleInstance();
            }
        }


        public void RegisterSingle<TService, TImplementer>(string serviceName = null)
          where TService : class
          where TImplementer : class, TService
        {
            var registrationBuilder = ContainerBuilder.RegisterType<TImplementer>().As<TService>().SingleInstance();
            if (serviceName != null)
            {
                registrationBuilder.Named<TService>(serviceName);
            }
        }

        public void RegisterInstance<TService, TImplementer>(TImplementer instance, string serviceName = null, LifeTime lifetime = LifeTime.Singleton)
           where TService : class
           where TImplementer : class, TService
        {
            var registrationBuilder = ContainerBuilder.RegisterInstance(instance).As<TService>();
            if (serviceName != null)
            {
                registrationBuilder.Named<TService>(serviceName);
            }
            if (lifetime == LifeTime.Singleton)
            {
                registrationBuilder.SingleInstance();
            }
        }

        public void RegisterSingleInstance<TImplementer>(TImplementer instance)
          where TImplementer : class
        {
            ContainerBuilder.RegisterInstance(instance).SingleInstance();
        }

        public void RegisterSingleInstance<TService, TImplementer>(TImplementer instance, string serviceName = null)
           where TService : class
           where TImplementer : class, TService
        {
            var registrationBuilder = ContainerBuilder.RegisterInstance(instance).As<TService>().SingleInstance();
            if (serviceName != null)
            {
                registrationBuilder.Named<TService>(serviceName);
            }
        }


        public TService Resolve<TService>() where TService : class
        {
            return Container.Resolve<TService>();
        }

        public object Resolve(Type serviceType)
        {
            return Container.Resolve(serviceType);
        }

        public bool TryResolve<TService>(out TService instance) where TService : class
        {
            return Container.TryResolve(out instance);
        }

        public bool TryResolve(Type serviceType, out object instance)
        {
            return Container.TryResolve(serviceType, out instance);
        }

        public TService ResolveNamed<TService>(string serviceName) where TService : class
        {
            return Container.ResolveNamed<TService>(serviceName);
        }

        public object ResolveNamed(string serviceName, Type serviceType)
        {
            return Container.ResolveNamed(serviceName, serviceType);
        }

        public bool TryResolveNamed(string serviceName, Type serviceType, out object instance)
        {
            return Container.TryResolveNamed(serviceName, serviceType, out instance);
        }


    }
}
