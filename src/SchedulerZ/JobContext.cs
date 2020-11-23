using SchedulerZ.Domain;
using SchedulerZ.Logging;
using SchedulerZ.Models;
using SchedulerZ.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SchedulerZ
{
    public class JobContext
    {
        public JobContext(JobEntity jobView, AssemblyDomain domain)
        {
            JobView = jobView;
            Domain = domain;

            try
            {
                if (!string.IsNullOrWhiteSpace(JobView.CustomParamsJson))
                    JobDataMap = Utils.JsonDeserialize<List<JobParam>>(JobView.CustomParamsJson).ToDictionary(x => x.Key, x => x.Value);
            }
            catch
            {
                JobDataMap = new Dictionary<string, object>();
            }
        }

        public JobEntity JobView { get; set; }
        public AssemblyDomain Domain { get; set; }

        public Dictionary<string, object> JobDataMap { private get; set; }

        public T GetJobData<T>(string name)
        {
            if (JobDataMap == null) return default;

            try
            {
                object value;
                JobDataMap.TryGetValue(name, out value);
                return (T)Convert.ChangeType(value, typeof(T));
            }
            catch
            {
                return default;
            }
        }
    }

    public class JobParam
    {
        public string Key { get; set; }
        public object Value { get; set; }
    }
}
