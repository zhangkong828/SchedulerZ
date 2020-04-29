using SchedulerZ.Core.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace SchedulerZ.Core.Scheduler
{
    public class JobRuntime
    {
        public JobView JobView { get; set; }

        public AssemblyDomain Domain { get; set; }

        public JobBase Instance { get; set; }
    }
}
