﻿using System;

namespace SchedulerZ.HelloWorldJob
{
    public class HelloWorld : JobBase
    {

        public override void Run(JobContext context)
        {
            //Logger.Info("Hello World");
            Logger.Info($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} Hello World");
        }

        
    }
}
