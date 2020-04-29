using SchedulerZ.Component;
using SchedulerZ.Core.Domain;
using SchedulerZ.Core.Scheduler;
using SchedulerZ.Logging;
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
            _configuration = Configuration.Create().UseAutofac().BuildContainer();

            _logger = Configuration.LoggerProvider.CreateLogger(typeof(Program).Name);


            _schedulerManager=SchedulerManager.Scheduler;
            _schedulerManager.StartScheduler();


            var jobView = new JobView() { 
                Id="test",
                Name="HelloWorldJob",
                Remark="这是一个测试Job",
                CronExpression= "0/5 * * * * ? ",
                AssemblyName= "SchedulerZ.HelloWorldJob",
                ClassName= "SchedulerZ.HelloWorldJob.HelloWorld",
            };

            _schedulerManager.StartJob(jobView);



            //var domain = DomainManager.Create(jobView.AssemblyName);
            //var jobAssemblyLocation = JobFactory.GetJobAssemblyPath(jobView.AssemblyName);
            //var assembly = domain.LoadFile(jobAssemblyLocation);
            //Type type = assembly.GetType(jobView.ClassName, true, true);
            //var instance = Activator.CreateInstance(type);
            //var j = instance as JobBase;
            //Console.WriteLine(j);
            //DomainManager.Remove(jobView.AssemblyName);
            
            Console.WriteLine("over!");
            Console.ReadKey();
        }
    }
}
