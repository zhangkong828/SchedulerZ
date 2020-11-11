using System;
using System.Collections.Generic;
using System.Text;

namespace SchedulerZ
{
    public class ConfigOptions
    {
        /// <summary>
        /// Job目录
        /// </summary>
        public string JobDirectory { get; set; } = "Packages";
        /// <summary>
        /// Job压缩文件的后缀
        /// </summary>
        public string JobAllowedFileExtension { get; set; } = ".zip";

        /// <summary>
        /// 故障转移次数
        /// </summary>
        public int FailoverCluster { get; set; } = 3;
        /// <summary>
        /// 是否强制开启熔断
        /// </summary>
        public bool CircuitBreakerForceOpen { get; set; }

        /// <summary>
        /// 错误率达到多少开启熔断保护
        /// </summary>
        public int BreakeErrorThresholdPercentage { get; set; } = 50;
        /// <summary>
        /// 熔断多少毫秒后去尝试请求
        /// </summary>
        public int BreakeSleepWindowInMilliseconds { get; set; } = 60000;
        /// <summary>
        /// 是否强制关闭熔断
        /// </summary>
        public bool BreakerForceClosed { get; set; }

        /// <summary>
        /// 10秒钟内至少多少请求失败，熔断器才发挥起作用
        /// </summary>
        public int BreakerRequestVolumeThreshold { get; set; } = 20;

        /// <summary>
        /// 信号量最大并发度
        /// </summary>
        public int MaxConcurrentRequests { get; set; } = 200;
    }
}
