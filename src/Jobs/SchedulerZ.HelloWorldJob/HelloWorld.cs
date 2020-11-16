using System;

namespace SchedulerZ.HelloWorldJob
{
    public class HelloWorld : JobBase
    {

        public override void Run(JobContext context)
        {
            var name = context.GetJobData<string>("name");
            var msg = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} Hello {name}";
            Console.WriteLine(msg);
            //Logger.Info();
        }

        
    }
}
