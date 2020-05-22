using System;
using System.Collections.Generic;
using System.Text;

namespace SchedulerZ.Scheduler.QuartzNet
{
    public class QuartzNetConfig
    {
        public string JobDirectory { get; set; } = "Jobs";
        public int ThreadPoolCount { get; set; } = 20;
    }
}
