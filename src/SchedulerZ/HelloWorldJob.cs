using System;
using System.Collections.Generic;
using System.Text;

namespace SchedulerZ
{
    public class HelloWorldJob : JobBase
    {
        public HelloWorldJob() : base("HelloWorldJob")
        {

        }

        public override void Run(JobContext context)
        {
            Logger.Info("Hello World");
        }
    }
}
