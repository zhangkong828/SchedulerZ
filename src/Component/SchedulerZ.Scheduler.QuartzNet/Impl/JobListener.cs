using Quartz;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SchedulerZ.Scheduler.QuartzNet.Impl
{
    public class JobListener : IJobListener
    {
        private readonly Action<IJobExecutionContext> _callBack;
        public JobListener(string jobId, Action<IJobExecutionContext> callBack)
        {
            Name = $"SchedulerZ.JobListener.{jobId}";
            _callBack = callBack;
        }

        public string Name { get; set; }

        public Task JobExecutionVetoed(IJobExecutionContext context, CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        public Task JobToBeExecuted(IJobExecutionContext context, CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        public Task JobWasExecuted(IJobExecutionContext context, JobExecutionException jobException, CancellationToken cancellationToken = default)
        {
            _callBack?.Invoke(context);
            return Task.CompletedTask;
        }
    }
}
