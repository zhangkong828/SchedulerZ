using System;

namespace SchedulerZ.HelloWorldJob
{
    public class HelloWorld : JobBase
    {
        public HelloWorld()
        {
            var a = 1;
        }

        public override void Run(JobContext context)
        {
            //Logger.Info("Hello World");
            Console.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} Hello World");
        }

        
    }
}
