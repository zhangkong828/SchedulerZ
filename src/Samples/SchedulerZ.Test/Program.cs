using SchedulerZ.Component;
using SchedulerZ.Core.Domain;
using SchedulerZ.Core.Scheduler;
using SchedulerZ.Logging;
using SchedulerZ.Route;
using SchedulerZ.Route.Consul;
using System;
using System.IO;
using System.Reflection;

namespace SchedulerZ.Test
{
    class Program
    {
        static Configuration _configuration;
        static ILogger _logger;

        static SchedulerManager _schedulerManager;

        static void Main(string[] args)
        {
            _configuration = Configuration.Create()
                .UseAutofac()
                .UseConsulServiceRoute(config =>
                {
                    config.Host = "192.168.1.203";
                })
                .Build();

            _logger = Configuration.LoggerProvider.CreateLogger(typeof(Program).Name);


            var serviceRoute = ObjectContainer.Resolve<IServiceRoute>();

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

            //_schedulerManager = SchedulerManager.Scheduler;
            //_schedulerManager.StartScheduler();


            //var jobView = new JobView()
            //{
            //    Id = "test",
            //    Name = "HelloWorldJob",
            //    Remark = "这是一个测试Job",
            //    CronExpression = "0/5 * * * * ? ",
            //    AssemblyName = "SchedulerZ.HelloWorldJob",
            //    ClassName = "SchedulerZ.HelloWorldJob.HelloWorld",
            //};

            //_schedulerManager.StartJob(jobView);



            Console.WriteLine("over!");
            Console.ReadKey();
        }
    }
}
