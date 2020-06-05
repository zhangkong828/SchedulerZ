using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace SchedulerZ.Manager.API.Model
{
    public enum ResponseStatusType
    {
        /// <summary>
        /// 失败
        /// </summary>
        [Description("失败")]
        Failed = 0,

        /// <summary>
        /// 成功
        /// </summary>
        [Description("成功")]
        Success = 1,

        /// <summary>
        /// 服务器异常
        /// </summary>
        [Description("服务器异常")]
        ServerException = 10000,

        /// <summary>
        /// 参数错误
        /// </summary>
        [Description("参数错误")]
        ParameterError = 10001,

        /// <summary>
        /// 无权限操作
        /// </summary>
        [Description("无权限操作")]
        Forbidden = 10002,
    }
}
