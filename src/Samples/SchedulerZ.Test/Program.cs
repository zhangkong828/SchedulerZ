using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SchedulerZ.LoadBalancer;
using SchedulerZ.Logging;
using SchedulerZ.Models;
using SchedulerZ.Route;
using SchedulerZ.Route.Consul;
using SchedulerZ.Scheduler;
using SchedulerZ.Scheduler.QuartzNet;
using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace SchedulerZ.Test
{
    class Program
    {
        static ILogger _logger;
        
        static void Main(string[] args)
        {
            var serviceProvider = new ServiceCollection()
                                    .UseDefaultLogging()
                                    .UseLoadBalancer(config =>
                                    {
                                        config.Type = "RoundRobinLoadBalancer";
                                    })
                                    .UseConsulServiceRoute(config =>
                                    {
                                        config.Host = "192.168.1.203";
                                    })
                                    .BuildServiceProvider();

            _logger = serviceProvider.GetService<ILoggerProvider>().CreateLogger("Main");


            var loadBalancer = serviceProvider.GetService<ILoadBalancerFactory>().Get();

            var service = loadBalancer.Lease("test").GetAwaiter().GetResult();

            Console.WriteLine($"{service.Name}|{service.Address}:{service.Port}");

            var job = new JobEntity()
            {
                Id = "test",
                Name = "HelloWorldJob",
                Remark = "这是一个测试Job",
                CronExpression = "0/5 * * * * ? ",
                AssemblyName = "SchedulerZ.HelloWorldJob",
                ClassName = "SchedulerZ.HelloWorldJob.HelloWorld",
            };



            Console.ReadKey();
        }
    }
}
