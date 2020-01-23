using System;
using System.Threading.Tasks;
using Quartz;

namespace SchedulerZ.Core.Scheduler
{
    [PersistJobDataAfterExecution]
    [DisallowConcurrentExecution]
    public class JobCommon:IJob
    {
        public JobCommon()
        {
        }

        public Task Execute(IJobExecutionContext context)
        {

            return Task.CompletedTask;
        }
    }
}
