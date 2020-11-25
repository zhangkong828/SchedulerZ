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
using SchedulerZ.Configurations;
using SchedulerZ.Store.MySQL;

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
                            configurationBuilder.AddConfigFile("schedulerZ.json", optional: true, reloadOnChange: true);
                        })
                        .ConfigureServices((context, services) =>
                        {
                            services.UseSchedulerZ()
                                    .UseMySQL()
                                    .UseConsulServiceRoute()
                                    .UseGrpcRemoting()
                                    .UseQuartzNetScheduler();
                        })
                        .Build();

            host.Run();
        }


    }
}
