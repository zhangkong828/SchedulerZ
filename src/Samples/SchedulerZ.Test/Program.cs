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
using System.Reflection;

namespace SchedulerZ.Test
{
    class Program
    {
        static ILogger _logger;

        static ISchedulerManager _schedulerManager;

        static void Main(string[] args)
        {
            var host = new HostBuilder()
                       .UseConsoleLifetime()
                       .ConfigureAppConfiguration((context, configurationBuilder) =>
                       {
                           configurationBuilder.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
                       })
                       .ConfigureServices((context, services) =>
                       {
                           services.UseDefaultLogging();
                           services.UseLoadBalancer(config =>
                           {
                               config.Type = "RoundRobinLoadBalancer";
                           });
                           services.UseConsulServiceRoute(config =>
                           {
                               config.Host = "192.168.1.203";
                           });
                           services.UseQuartzNetScheduler();
                       })
                       .Build();

            var serviceProvider = host.Services;

            //var serviceProvider = new ServiceCollection()
            //                        .UseDefaultLogging()
            //                        .UseLoadBalancer(config =>
            //                        {
            //                            config.Type = "RoundRobinLoadBalancer";
            //                        })
            //                        .UseConsulServiceRoute(config =>
            //                        {
            //                            config.Host = "192.168.1.203";
            //                        })
            //                        .BuildServiceProvider();

            _logger = serviceProvider.GetService<ILoggerProvider>().CreateLogger("Main");


            var serviceRoute = serviceProvider.GetService<IServiceRoute>();

            var service = new ServiceRouteDescriptor()
            {
                Id = Guid.NewGuid().ToString("n"),
                Name = "test",
                Address = "192.168.1.202",
                Port = 10004
            };

            serviceRoute.RegisterService(service).GetAwaiter().GetResult();

            var result = serviceRoute.DiscoverServices("test").GetAwaiter().GetResult();
            foreach (var item in result)
            {
                Console.WriteLine($"{item.Id}|{item.Name} {item.Address}:{item.Port}");
            }


            _schedulerManager = serviceProvider.GetService<ISchedulerManager>();

            var jobView = new JobEntity()
            {
                Id = "test",
                Name = "HelloWorldJob",
                Remark = "这是一个测试Job",
                CronExpression = "0/5 * * * * ? ",
                AssemblyName = "SchedulerZ.HelloWorldJob",
                ClassName = "SchedulerZ.HelloWorldJob.HelloWorld",
            };

            _schedulerManager.StartJob(jobView);



            host.Run();
        }
    }
}
