using SchedulerZ.Component;
using SchedulerZ.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace SchedulerZ
{
    /// <summary>
    /// 所有job必须继承此类
    /// </summary>
    public abstract class JobBase: MarshalByRefObject, IDisposable
    {
        public ILogger Logger;
        private bool _isRunning = false;

        //public JobBase(string jobKey)
        //{
        //    //Logger = Configuration.LoggerProvider.CreateLogger(jobKey);
        //    Logger = new ConsoleLoggerProvider().CreateLogger(jobKey);
        //    JobKey = jobKey;
        //}

        public string JobKey { get; set; }

        public abstract void Run(JobContext context);

        public void Execute(JobContext context)
        {
            if (!_isRunning)
            {
                _isRunning = true;
                try
                {
                    Run(context);
                }
                catch (Exception ex)
                {
                    Logger?.Error("执行Job异常", ex);
                }
                finally
                {
                    _isRunning = false;
                }
            }
        }

        public void Dispose()
        {
            
        }
    }
}
