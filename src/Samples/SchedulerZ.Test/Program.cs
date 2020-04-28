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
                Name="HelloWorldJob",
                Remark="这是一个测试Job",
                CronExpression= "0/5 * * * * ? ",
                AssemblyName= "SchedulerZ.HelloWorldJob",
                ClassName= "SchedulerZ.HelloWorldJob.HelloWorld",
            };

            //_schedulerManager.StartJob(jobView);

            var t = typeof(HelloWorld);
            var i= Activator.CreateInstance(t);
            var j1 = i as JobBase;

            var domain = DomainManager.Create(jobView.AssemblyName);
            using (DomainManager.Lock(jobView.AssemblyName))
            {
                string jobLocation = JobFactory.GetJobAssemblyPath(jobView.AssemblyName);
                //var assembly = domain.LoadFromAssemblyName(new AssemblyName(Path.GetFileNameWithoutExtension(jobLocation)));
                var assembly = domain.LoadStream(jobLocation);
                Type type = assembly.GetType(jobView.ClassName, true, true);
                var instance = Activator.CreateInstance(type);
                var j = instance as JobBase;
            }
           

            //domain.RemoveDll(jobLocation);
            //domain.RemoveAssembly(assembly);
            //DomainManager.Remove(jobView.AssemblyName);

            Console.WriteLine("over!");
            Console.ReadKey();
        }
    }
}
