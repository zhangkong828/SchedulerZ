using System;
using System.Collections.Generic;
using System.Text;

namespace SchedulerZ.Job.FundTest
{
    /// <summary>
    /// 基金
    /// </summary>
    public class Fund
    {
        public string Id { get; set; }

        /// <summary>
        /// 基金全称
        /// </summary>
        public string FullName { get; set; }

        /// <summary>
        /// 基金简称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 基金代码
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 基金类型
        /// </summary>
        public int Type { get; set; }

        /// <summary>
        /// 发行日期
        /// </summary>
        public string IssueDate { get; set; }

        /// <summary>
        /// 成立日期
        /// </summary>
        public string EstablishmentDate { get; set; }

        /// <summary>
        /// 资产规模
        /// </summary>
        public string AssetSize { get; set; }

        /// <summary>
        /// 份额规模
        /// </summary>
        public string ShareSize { get; set; }

        /// <summary>
        /// 所属基金公司Id
        /// </summary>
        public string FundCompanyId { get; set; }
        /// <summary>
        /// 所属基金公司名称
        /// </summary>
        public string FundCompanyName { get; set; }

        /// <summary>
        /// 基金托管人
        /// </summary>
        public string FundRrustee { get; set; }

        /// <summary>
        /// 基金管理人
        /// </summary>
        public string FundManager { get; set; }

        /// <summary>
        /// 成立来分红
        /// </summary>
        public string Dividend { get; set; }

        /// <summary>
        /// 管理费率
        /// </summary>
        public string ManagementFeeRate { get; set; }

        /// <summary>
        /// 托管费率
        /// </summary>
        public string CustodianFeeRate { get; set; }

        /// <summary>
        /// 销售服务费率
        /// </summary>
        public string SalesServiceRate { get; set; }

        /// <summary>
        /// 最高认购费率
        /// </summary>
        public string MaximumSubscriptionRate { get; set; }

        /// <summary>
        /// 最高申购费率
        /// </summary>
        public string MaximunPurchaseRate { get; set; }

        /// <summary>
        /// 最高赎回费率
        /// </summary>
        public string MaximunRedemptionRate { get; set; }

        /// <summary>
        /// 业绩比较基准
        /// </summary>
        public string PerformanceBenchmark { get; set; }
    }

    public enum FundType
    {
        /// <summary>
        /// 股票型基金
        /// </summary>
        StockFund,
        /// <summary>
        /// 货币型基金
        /// </summary>
        MonetaryFund,
        /// <summary>
        /// 债券型基金
        /// </summary>
        BondFund,
        /// <summary>
        /// 混合型基金
        /// </summary>
        HybridFund
    }
}
