using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SchedulerZ.Configurations;
using SchedulerZ.LoadBalancer;
using SchedulerZ.Logging;
using SchedulerZ.Models;
using SchedulerZ.Remoting;
using SchedulerZ.Remoting.gRPC.Client;
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
            var configurationBuilder = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("schedulerZ.json", optional: true, reloadOnChange: true);

            Config.Configuration = configurationBuilder.Build();
            Config.Options = Config.Get<ConfigOptions>("SchedulerZ") ?? new ConfigOptions();
            Config.LoggerOptions = Config.Get<LoggerOptions>("SchedulerZ:Logger") ?? new LoggerOptions();

            var serviceProvider = new ServiceCollection()
                                    .UseLoadBalancer(config =>
                                    {
                                        config.Type = "RoundRobinLoadBalancer";
                                    })
                                    .UseConsulServiceRoute(config =>
                                    {
                                        config.Host = "192.168.1.203";
                                    })
                                    .UseGrpcRemotingClient()
                                    .BuildServiceProvider();

            _logger = TraceLogger.Instance;


            //通过 负载 拿到可用service
            var loadBalancer = serviceProvider.GetService<ILoadBalancerFactory>().Get();
            var service = loadBalancer.Lease("worker").GetAwaiter().GetResult();
            Console.WriteLine($"{service.Name}|{service.Address}:{service.Port}");

            //远程调用
            var remoting = serviceProvider.GetService<ISchedulerRemoting>();
            var job = new JobEntity()
            {
                Name = "HelloWorldJob",
                Remark = "这是一个测试Job",
                CronExpression = "0/5 * * * * ? ",
                AssemblyName = "SchedulerZ.HelloWorldJob",
                ClassName = "SchedulerZ.HelloWorldJob.HelloWorld",
                CustomParamsJson = "[{\"key\":\"name\",\"value\":\"zk\"}]",
                FilePath = "SchedulerZ.HelloWorldJob.zip"
            };

            remoting.StartJob(job, service);

            //上传
            //remoting.UploadFile(@"D:\github\SchedulerZ\src\Jobs\SchedulerZ.HelloWorldJob\bin\Debug\netcoreapp3.1\SchedulerZ.HelloWorldJob.zip", service);

            //下载
            //remoting.DownloadFile("SchedulerZ.HelloWorldJob.zip", @"d:\", service);

            Console.ReadKey();
        }
    }
}
