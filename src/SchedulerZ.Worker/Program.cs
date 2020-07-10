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
                            services.UseDefaultLogging()
                                    .UseConsulServiceRoute(config =>
                                    {
                                        config.Host = "192.168.31.101";
                                        config.Port = 8500;

                                    }, registerService =>
                                    {
                                        registerService.Name = "test";
                                        registerService.Address = "192.168.31.200";
                                        registerService.Port = 10001;
                                        registerService.HealthCheckType = "TCP";
                                        registerService.HealthCheck = "192.168.31.200:10001";
                                    })
                                    .UseGrpcRemoting(config =>
                                    {
                                        config.Host = "0.0.0.0";
                                        config.Port = 10001;
                                    })
                                    .UseQuartzNetScheduler();
                        })
                        .Build();

            host.Run();
        }


    }
}
