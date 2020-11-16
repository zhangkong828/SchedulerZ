using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Quartz;

namespace SchedulerZ.Scheduler.QuartzNet.Impl
{
    [PersistJobDataAfterExecution]
    [DisallowConcurrentExecution]
    public class JobImplementation : IJob
    {
        public JobImplementation()
        {
        }

        public Task Execute(IJobExecutionContext context)
        {
            IJobDetail job = context.JobDetail;
            try
            {
                if (job.JobDataMap["JobRuntime"] is JobRuntime jobRuntime)
                {
                    Stopwatch stopwatch = new Stopwatch();
                    JobContext jobContext = new JobContext(jobRuntime.JobView,jobRuntime.Domain);
                    try
                    {
                        stopwatch.Restart();
                        jobRuntime.Execute(jobContext);
                        stopwatch.Stop();
                        context.Result = jobRuntime;
                    }
                    catch
                    {
                        stopwatch.Stop();
                        throw;
                    }
                }

            }
            catch
            {

                throw;
            }

            return Task.CompletedTask;
        }
    }
}
