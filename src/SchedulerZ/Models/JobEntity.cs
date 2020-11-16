using SchedulerZ.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SchedulerZ.Models
{
    public class JobEntity
    {
        public JobEntity()
        {
            Id = ObjectId.Default().NextString();
            CreateTime = DateTime.Now;
        }

        [Key]
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
        /// false 通过CronExpression控制
        /// true 通过RepeatCount、IntervalSeconds控制
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

        [Required]
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// Job状态
        /// </summary>
        [Required]
        public int Status { get; set; }

        /// <summary>
        /// 上次运行时间
        /// </summary>
        public DateTime? LastRunTime { get; set; }

        /// <summary>
        /// 下次运行时间
        /// </summary>
        public DateTime? NextRunTime { get; set; }

        /// <summary>
        /// 总运行成功次数
        /// </summary>
        public int TotalRunCount { get; set; }

        public string NodeHost { get; set; }
        public int NodePort { get; set; }
    }

    /// <summary>
    /// 任务状态
    /// </summary>
    public enum JobStatus
    {
        /// <summary>
        /// 已删除
        /// </summary>
        [Description("已删除")]
        Deleted = -1,

        /// <summary>
        /// 已停止
        /// </summary>
        [Description("已停止")]
        Stop = 0,

        /// <summary>
        /// 运行中
        /// </summary>
        [Description("运行中")]
        Running = 1,

        /// <summary>
        /// 已暂停
        /// </summary>
        [Description("已暂停")]
        Paused = 2

    }
}
