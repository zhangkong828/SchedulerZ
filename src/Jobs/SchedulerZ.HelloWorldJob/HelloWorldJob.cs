using System;

namespace SchedulerZ.HelloWorldJob
{
    public class HelloWorldJob : JobBase
    {
        public HelloWorldJob() : base("HelloWorldJob")
        {
            var a = 1;
        }

        public override void Run(JobContext context)
        {
            Logger.Info("Hello World");
        }
    }
}
