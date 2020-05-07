﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SchedulerZ.Route
{
    public interface IServiceRoute
    {
        /// <summary>
        /// 获取服务
        /// </summary>
        Task<IEnumerable<ServiceRouteDescriptor>> DiscoverServices(string name);

        /// <summary>
        /// 注册服务
        /// </summary>
        Task<bool> RegisterService(ServiceRouteDescriptor service);
    }
}
