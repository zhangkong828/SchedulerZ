using System;
using System.Collections.Generic;
using System.Text;

namespace SchedulerZ.Job.FundTest
{
    /// <summary>
    /// 公司
    /// </summary>
    public class Company
    {
        public string Id { get; set; }

        /// <summary>
        /// 法定名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 英文名称
        /// </summary>
        public string NameEN { get; set; }

        /// <summary>
        /// 公司属性
        /// </summary>
        public string Attributes { get; set; }

        /// <summary>
        /// 成立日期
        /// </summary>
        public string Establishment { get; set; }

        /// <summary>
        /// 注册资本
        /// </summary>
        public string RegisteredCapital { get; set; }

        /// <summary>
        /// 法定代表人
        /// </summary>
        public string LegalRepresentative { get; set; }

        /// <summary>
        /// 注册地址
        /// </summary>
        public string RegisteredAddress { get; set; }

        /// <summary>
        /// 办公地址
        /// </summary>
        public string BusinessAddress { get; set; }

        /// <summary>
        /// 网站地址
        /// </summary>
        public string WebSite { get; set; }

        /// <summary>
        /// 电话号码
        /// </summary>
        public string PhoneNumber { get; set; }

        /// <summary>
        /// 传真号码
        /// </summary>
        public string FaxNumber { get; set; }

        /// <summary>
        /// 电子邮箱
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// 经营范围
        /// </summary>
        public string BusinessScope { get; set; }

    }
}
