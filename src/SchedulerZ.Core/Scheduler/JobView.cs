using System;
namespace SchedulerZ.Core.Scheduler
{
    public class JobView
    {
        public JobView()
        {
            this.Id = Guid.NewGuid().ToString();
            this.CreateTime = DateTime.Now;
        }

        public string Id { get; private set; }
        public string Name { get; set; }
        public string Remark { get; set; }
        public bool IsSimple { get; set; } = false;
        public string CronExpression { get; set; }
        public int RepeatCount { get; set; } = -1;
        public TimeSpan IntervalTimeSpan { get; set; } = TimeSpan.FromSeconds(0);
        public string AssemblyName { get; set; }
        public string ClassName { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public DateTime CreateTime { get; private set; }

        public bool Validate()
        {
            if (string.IsNullOrWhiteSpace(this.Name))
                return false;

            if (string.IsNullOrWhiteSpace(this.AssemblyName) || string.IsNullOrWhiteSpace(this.ClassName))
                return false;

            if (this.IsSimple)
            {
                if (this.RepeatCount != -1 && this.RepeatCount <= 0)
                    return false;

                if (this.IntervalTimeSpan < TimeSpan.FromSeconds(0))
                    return false;
            }
            else
            {
                if (string.IsNullOrWhiteSpace(this.CronExpression))
                    return false;
                return SchedulerManager.ValidExpression(this.CronExpression);
            }

            return true;
        }
    }
}
