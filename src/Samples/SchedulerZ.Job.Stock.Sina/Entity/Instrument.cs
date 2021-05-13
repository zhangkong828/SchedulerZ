using System;
using System.Collections.Generic;
using System.Text;

namespace SchedulerZ.Job.Stock.Sina.Entity
{
    public enum ExchangeType
    {
        /// <summary>
        /// 上交所
        /// </summary>
        SHSE = 1,

        /// <summary>
        /// 深交所
        /// </summary>
        SZSE = 2,

        /// <summary>
        /// 中金所
        /// </summary>
        CFFEX = 3,

        /// <summary>
        /// 上期所
        /// </summary>
        SHFE = 4,

        /// <summary>
        /// 大商所
        /// </summary>
        DCE = 5,

        /// <summary>
        /// 郑商所
        /// </summary>
        CZCE = 6,

        /// <summary>
        /// 上海国际能源交易中心
        /// </summary>
        INE = 7
    }

    public enum InstrumentType
    {
        /// <summary>
        /// 股票
        /// </summary>
        Stock = 1,

        /// <summary>
        /// 基金
        /// </summary>
        Fund = 2,

        /// <summary>
        /// 指数
        /// </summary>
        Index = 3,

        /// <summary>
        /// 期货
        /// </summary>
        Future = 4,

        /// <summary>
        /// 期权
        /// </summary>
        Option = 5,

        /// <summary>
        /// 虚拟合约
        /// </summary>
        Confuture = 6,
    }

    public class Instrument
    {
        public string Code { get; set; }

        public string Symbol { get; set; }


        /// <summary>
        /// 品种类型
        /// </summary>
        public InstrumentType Type { get; set; }

        /// <summary>
        /// 交易所
        /// </summary>
        public ExchangeType Exchange { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 最小变动单位
        /// </summary>
        public double PriceTick { get; set; }

        /// <summary>
        /// 上市日期
        /// </summary>
        public DateTime ListedDate { get; set; }

        /// <summary>
        /// 退市日期
        /// </summary>
        public DateTime DelistedDate { get; set; }
    }
}
