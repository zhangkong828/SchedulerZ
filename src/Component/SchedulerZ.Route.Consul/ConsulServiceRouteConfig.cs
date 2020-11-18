using SchedulerZ.Utility;
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

        /// <summary>
        /// 服务健康检测时间 毫秒
        /// </summary>
        public int ServiceCheckInterval { get; set; } = 10000;

        /// <summary>
        /// 移除服务时间 毫秒
        /// </summary>
        public int ServiceCriticalInterval { get; set; } = 3000;

        /// <summary>
        /// 节点检测间隔时间 毫秒
        /// </summary>
        public int NodeCheckInterval { get; set; } = 60000;

        /// <summary>
        /// 节点检测超时时间 毫秒
        /// </summary>
        public int NodeCheckTimeOut { get; set; } = 5000;


    }

    public class RegisterServiceConfig
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 地址
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// 端口
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// 健康检查类型
        /// </summary>
        public string HealthCheckType { get; set; }

        /// <summary>
        /// 健康检查地址
        /// </summary>
        public string HealthCheck { get; set; }
    }
}
