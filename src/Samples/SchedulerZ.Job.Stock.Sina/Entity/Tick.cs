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

        /// <summary>
        /// 买一量
        /// </summary>
        public long BidVolume1 { get; set; }
        /// <summary>
        /// 买一价
        /// </summary>
        public double Bid1 { get; set; }

        public long BidVolume2 { get; set; }
        public double Bid2 { get; set; }

        public long BidVolume3 { get; set; }
        public double Bid3 { get; set; }

        public long BidVolume4{ get; set; }
        public double Bid4 { get; set; }

        public long BidVolume5 { get; set; }
        public double Bid5 { get; set; }

        /// <summary>
        /// 卖一量
        /// </summary>
        public long AskVolume1 { get; set; }
        /// <summary>
        /// 卖一价
        /// </summary>
        public double Ask1 { get; set; }

        public long AskVolume2 { get; set; }
        public double Ask2 { get; set; }

        public long AskVolume3 { get; set; }
        public double Ask3 { get; set; }

        public long AskVolume4 { get; set; }
        public double Ask4 { get; set; }

        public long AskVolume5 { get; set; }
        public double Ask5 { get; set; }

        /// <summary>
        /// 日期
        /// </summary>
        public DateTime Date { get; set; }
        /// <summary>
        /// 时间
        /// </summary>
        public DateTime Time { get; set; }
    }
}
