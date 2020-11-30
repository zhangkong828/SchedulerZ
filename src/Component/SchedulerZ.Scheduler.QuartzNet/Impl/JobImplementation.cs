using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Quartz;

namespace SchedulerZ.Scheduler.QuartzNet.Impl
{
    //[PersistJobDataAfterExecution]
    //[DisallowConcurrentExecution]
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
                    JobContext jobContext = new JobContext(jobRuntime.JobView, jobRuntime.Domain);
                    try
                    {
                        jobRuntime.Execute(jobContext);
                        context.Result = jobRuntime;
                    }
                    catch
                    {
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
