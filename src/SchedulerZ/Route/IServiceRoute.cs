using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SchedulerZ.Route
{
    public interface IServiceRoute
    {
        /// <summary>
        /// 发现服务
        /// </summary>
        Task<IEnumerable<ServiceRouteDescriptor>> DiscoverServices(string name);

        /// <summary>
        /// 注册服务
        /// </summary>
        Task<bool> RegisterService(ServiceRouteDescriptor service);

        /// <summary>
        /// 移除服务
        /// </summary>
        Task<bool> DeRegisterService(ServiceRouteDescriptor service);


        /// <summary>
        /// 所有服务
        /// </summary>
        Task<IEnumerable<ServiceRouteDescriptor>> QueryServices();

        /// <summary>
        /// 所有节点
        /// </summary>
        Task<IEnumerable<NodeDescriptor>> QueryNodes();

        Task<string> GetKV(string key);

        Task<bool> SetKV(string key, string value);

    }
}
