using SchedulerZ.Component;
using SchedulerZ.Core.Scheduler;
using SchedulerZ.Logging;
using System;

namespace SchedulerZ.Test
{
    class Program
    {
        static Configuration _configuration;
        static ILogger _logger;

        static SchedulerManager _schedulerManager;

        static void Main(string[] args)
        {
            _configuration = Configuration.Create().UseAutofac().BuildContainer();

            _logger = Configuration.LoggerProvider.CreateLogger(typeof(Program).Name);


            _schedulerManager=SchedulerManager.Scheduler;
            _schedulerManager.StartScheduler();


            var jobView = new JobView() { 
                Name="HelloWorldJob",
                Remark="这是一个测试Job",
                CronExpression= "0/5 * * * * ? ",
                AssemblyName= "SchedulerZ.HelloWorldJob",
                ClassName= "SchedulerZ.HelloWorldJob.HelloWorldJob",
            };

            _schedulerManager.StartJob(jobView);

            Console.WriteLine("over!");
            Console.ReadKey();
        }
    }
}
