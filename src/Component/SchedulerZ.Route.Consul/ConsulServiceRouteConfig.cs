using System;
using System.Collections.Generic;
using System.Text;

namespace SchedulerZ.Route.Consul
{
    public class ConsulServiceRouteConfig
    {
        public string Scheme { get; set; } = "http";
        public string Host { get; set; } = "127.0.0.1";
        public int Port { get; set; } = 8500;

        public int HealthCheckTimeOut { get; set; } = 60;

        /// <summary>
        /// 健康检测时间
        /// </summary>
        public TimeSpan CheckInterval { get; set; } = TimeSpan.FromSeconds(10);

        /// <summary>
        /// 移除服务时间
        /// </summary>
        public TimeSpan CriticalInterval { get; set; } = TimeSpan.FromSeconds(20);
    }
}
