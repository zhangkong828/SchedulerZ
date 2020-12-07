using System;
using System.Collections.Generic;
using System.Text;

namespace SchedulerZ.Job.FundTest
{
    /// <summary>
    /// 基金公司
    /// </summary>
    public class FundCompany : Company
    {
        /// <summary>
        /// 基金数量
        /// </summary>
        public int FundNumber { get; set; }

        /// <summary>
        /// 基金管理规模
        /// </summary>
        public string AssetsUnderManagement { get; set; }

        /// <summary>
        /// 经理人数
        /// </summary>
        public int ManagerNumber { get; set; }
    }
}
