using SchedulerZ.Component;
using SchedulerZ.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace SchedulerZ
{
    public abstract class JobBase
    {
        public ILogger Logger;
        public JobBase(string jobKey)
        {
            Logger = Configuration.LoggerProvider.CreateLogger(jobKey);

            JobKey = jobKey;
        }

        public string JobKey { get; set; }

        public abstract void Run(JobContext context);

        public void Execute(JobContext context)
        {
            try
            {
                Run(context);
            }
            catch (Exception ex)
            {
                Logger.Error("执行Job异常", ex);
            }

        }

    }
}
