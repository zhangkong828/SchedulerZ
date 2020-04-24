using System;
using System.Collections.Generic;
using System.Text;

namespace SchedulerZ
{
    public class JobContext
    {
        private JobBase _instance;

        public JobContext(JobBase instance)
        {
            _instance = instance;
        }

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
}
