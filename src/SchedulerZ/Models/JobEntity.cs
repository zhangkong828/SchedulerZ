using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SchedulerZ.Models
{
    public class JobEntity
    {
        [Key]
        public string Id { get; set; }

        [Required]
        public string Name { get; set; }


        public string Remark { get; set; }
        public bool IsSimple { get; set; }
        public string CronExpression { get; set; }
        public int RepeatCount { get; set; }
        public long IntervalSeconds { get; set; }

        [Required]
        public string AssemblyName { get; set; }
        [Required]
        public string ClassName { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        [Required]
        public DateTime CreateTime { get; set; }
    }
}
