using System;

namespace SchedulerZ.HelloWorldJob
{
    public class HelloWorld : JobBase
    {

        public override void Run(JobContext context)
        {
            var name = context.GetJobData<string>("name");
            var msg = $"Hello {name}";
            Logger.Info(msg);
        }

        
    }
}
