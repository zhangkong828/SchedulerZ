using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
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
                                    .UseGrpcRemotingClient()
                                    .BuildServiceProvider();

            _logger = serviceProvider.GetService<ILoggerProvider>().CreateLogger("Main");

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
                FilePath = "111"
            };

            remoting.StartJob(job, service);


            Console.ReadKey();
        }
    }
}
