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
}
