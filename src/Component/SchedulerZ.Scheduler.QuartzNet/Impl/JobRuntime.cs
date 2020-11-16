using SchedulerZ.Domain;
using SchedulerZ.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SchedulerZ.Scheduler.QuartzNet.Impl
{
    public class JobRuntime
    {
        public JobEntity JobView { get; set; }

        public AssemblyDomain Domain { get; set; }

        public JobBase Instance { get; set; }

        public void Execute(JobContext jobContext)
        {
            Instance.Execute(jobContext);
        }

        public void Dispose()
        {
            DomainManager.Remove(JobView.Id);
            Instance = null;
        }
    }
}
