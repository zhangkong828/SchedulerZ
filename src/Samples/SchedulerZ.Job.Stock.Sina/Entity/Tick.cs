using System;
using System.Collections.Generic;
using System.Text;

namespace SchedulerZ.Job.Stock.Sina.Entity
{
    public class Tick
    {
        public string Name { get; set; }

        /// <summary>
        /// 今日开盘价
        /// </summary>
        public double Open { get; set; }
        /// <summary>
        /// 昨日收盘价
        /// </summary>
        public double Close { get; set; }
        /// <summary>
        /// 当前价格
        /// </summary>
        public double Now { get; set; }
        /// <summary>
        /// 今日最高价
        /// </summary>
        public double High { get; set; }
        /// <summary>
        /// 今日最低价
        /// </summary>
        public double Low { get; set; }
        /// <summary>
        /// 交易量
        /// </summary>
        public long Volume { get; set; }
        /// <summary>
        /// 交易额
        /// </summary>
        public double Amount { get; set; }
    }
}
