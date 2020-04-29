using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SchedulerZ.Core.Route
{
    public interface IServiceRoute
    {
        /// <summary>
        /// 获取服务
        /// </summary>
        Task<IEnumerable<ServiceRouteDescriptor>> GetServices(string key);

        /// <summary>
        /// 注册服务
        /// </summary>
        Task<bool> RegisterService(ServiceRouteDescriptor service);

        /// <summary>
        /// 移除服务
        /// </summary>
        Task<bool> UnregisterService(ServiceRouteDescriptor service);

    }
}
