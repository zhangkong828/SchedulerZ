using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SchedulerZ.Manager.API.Model.Request
{
    public class SchedulerJobRequest
    {
        public string Id { get; set; }

        [Required]
        [StringLength(30)]
        public string Name { get; set; }

        [Required]
        [StringLength(100)]
        public string FilePath { get; set; }

        /// <summary>
        /// Job描述
        /// </summary>
        [StringLength(500)]
        public string Remark { get; set; }

        /// <summary>
        /// 是否简易Job(周期运行)
        /// </summary>
        public bool IsSimple { get; set; }

        /// <summary>
        /// cron表达式
        /// </summary>
        [StringLength(20)]
        public string CronExpression { get; set; }

        /// <summary>
        /// 重复次数
        /// </summary>
        public int RepeatCount { get; set; }

        /// <summary>
        /// 间隔秒数
        /// </summary>
        public long IntervalSeconds { get; set; }

        [Required]
        [StringLength(100)]
        public string AssemblyName { get; set; }
        [Required]
        [StringLength(100)]
        public string ClassName { get; set; }

        /// <summary>
        /// 自定义参数
        /// </summary>
        public string CustomParamsJson { get; set; }

        /// <summary>
        /// 生效时间
        /// </summary>
        public DateTime? StartTime { get; set; }
        /// <summary>
        /// 失效时间
        /// </summary>
        public DateTime? EndTime { get; set; }

        /// <summary>
        /// 是否立即执行
        /// </summary>
        public bool RunNow { get; set; }
    }
}
