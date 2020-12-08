using System;
using System.Collections.Generic;
using System.Text;

namespace SchedulerZ.Job.FundTest.Model
{
    public class CompanyRequest
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Url { get { return $"http://fund.eastmoney.com/Company/f10/jbgk_{Id}.html"; } }

        public List<FundRequest> Funds { get; set; }
    }

    public class FundRequest
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string Url { get { return $"http://fundf10.eastmoney.com/jbgk_{Code}.html"; } }
    }

    public class IOPVResponse
    {
        public List<IOPVData> Datas { get; set; }
        public int ErrCode { get; set; }
        public bool Success { get; set; }
        public string ErrMsg { get; set; }
        public string Message { get; set; }
        public string ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
        public string ErrorMsgLst { get; set; }
        public int TotalCount { get; set; }
        public Expansion Expansion { get; set; }
    }

    public class Expansion
    {
        public string GZTIME { get; set; }
        public string FSRQ { get; set; }
    }

    public class IOPVData
    {
        /// <summary>
        /// 代码
        /// </summary>
        public string FCODE { get; set; }
        /// <summary>
        /// 简称
        /// </summary>
        public string SHORTNAME { get; set; }

        /// <summary>
        /// 最新日期
        /// </summary>
        public string PDATE { get; set; }
        /// <summary>
        /// 单位净值
        /// </summary>
        public string NAV { get; set; }
        /// <summary>
        /// 累计净值
        /// </summary>
        public string ACCNAV { get; set; }
        /// <summary>
        /// 日涨幅
        /// </summary>
        public string NAVCHGRT { get; set; }

        /// <summary>
        /// 预估净值
        /// </summary>
        public string GSZ { get; set; }
        /// <summary>
        /// 预估涨幅
        /// </summary>
        public string GSZZL { get; set; }
        /// <summary>
        /// 预估时间
        /// </summary>
        public string GZTIME { get; set; }
       
        public string NEWPRICE { get; set; }
        public string CHANGERATIO { get; set; }
        public string ZJL { get; set; }
        public string HQDATE { get; set; }
        public bool ISHAVEREDPACKET { get; set; }
    }

}
