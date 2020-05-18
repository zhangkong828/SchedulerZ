using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using SchedulerZ.LoadBalancer;
using SchedulerZ.Logging;
using SchedulerZ.Route;
using SchedulerZ.Route.Consul;
using SchedulerZ.Scheduler.QuartzNet;
using System;
using System.IO;
using System.Reflection;
using SchedulerZ.Remoting.gRPC;

namespace SchedulerZ.Worker
{
    class Program
    {
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
                            services.UseConsulServiceRoute(config =>
                            {
                                config.Host = "192.168.31.101";
                            });
                            services.UseGrpcRemoting(config =>
                            {
                                config.Host = "0.0.0.0";
                                config.Port = 10001;
                            });
                            services.UseQuartzNetScheduler();
                        })
                        .Build();

            //注册
            var serviceRoute = host.Services.GetService<IServiceRoute>();
            var service = new ServiceRouteDescriptor()
            {
                Id = Guid.NewGuid().ToString("n"),
                Name = "test",
                Address = "192.168.31.200",
                Port = 10001
            };
            serviceRoute.RegisterService(service).GetAwaiter().GetResult();
            host.Run();
        }


    }
}
