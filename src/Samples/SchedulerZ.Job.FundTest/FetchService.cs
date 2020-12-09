using SchedulerZ.Job.FundTest.Model;
using SchedulerZ.Utility;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace SchedulerZ.Job.FundTest
{
    public interface IFetchService
    {
        Task GetPage();
    }

    public class FetchService : IFetchService
    {
        private readonly IHttpClientFactory _clientFactory;

        public FetchService(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        public async Task GetPage()
        {
            //await GetAllFund();
            //var company=await GetCompanyInfo("http://fund.eastmoney.com/Company/f10/jbgk_80000225.html");
            //var fund = await GetFundInfo("http://fundf10.eastmoney.com/jbgk_003561.html");

            ConcurrentDictionary<FundCompany, List<Fund>> dic = new ConcurrentDictionary<FundCompany, List<Fund>>();

            //var companys = await GetAllFund();
            //var result = Parallel.ForEach(companys, async company =>
            //  {
            //      var companyInfo = await TaskHelper.RetryOnFault(() => GetCompanyInfo(company.Url), 3, () => Task.Delay(1000));

            //      if (companyInfo != null)
            //      {
            //          Console.WriteLine(companyInfo.Name);
            //          var funds = new List<Fund>();
            //          foreach (var item in company.Funds)
            //          {
            //              var fundInfo = await TaskHelper.RetryOnFault(() => GetFundInfo(item.Url), 3, () => Task.Delay(1000));
            //              if (fundInfo != null)
            //              {
            //                  fundInfo.Code = item.Code;
            //                  Console.WriteLine($"[{fundInfo.Code}]{fundInfo.Name}");
            //                  funds.Add(fundInfo);
            //              }
            //              await Task.Delay(500);
            //          }

            //          dic.TryAdd(companyInfo, funds);
            //      }
            //  });

            //if (result.IsCompleted)
            //{
            //    Console.WriteLine($"over! total company:{dic.Keys.Count}");
            //}

            await GetFundIOPV("320007");
        }

        private async Task<List<CompanyRequest>> GetAllFund()
        {
            var companys = new List<CompanyRequest>();

            var url = "http://fund.eastmoney.com/allfund_com.html";
            var result = await Get(url, "GB2312");

            var ul_topReg = new Regex("class=\"ul_top\".+?>(.+?)</ul>");
            var num_rightReg = new Regex("class=\"num_right\".+?>(.+?)</ul>");

            var tops = ul_topReg.Matches(result);
            var rights = num_rightReg.Matches(result);
            if (tops.Count == rights.Count)
            {
                var companyReg = new Regex("class=\"comName\" href=\"http://fund.eastmoney.com/company/(.+?)\\.html\">(.+?)</a>");
                var fundReg = new Regex("title=\".+?\" href=\".+?\">（(.+?)）(.+?)</a>");

                for (int i = 0; i < tops.Count; i++)
                {
                    var top = tops[i].Groups[1].Value;
                    var right = rights[i].Groups[1].Value;
                    var companyMatch = companyReg.Match(top);
                    var fundMatchs = fundReg.Matches(right);

                    var companyId = companyMatch.Groups[1].Value;
                    var companyName = companyMatch.Groups[2].Value;

                    var company = new CompanyRequest()
                    {
                        Id = companyId,
                        Name = companyName,
                        Funds = new List<FundRequest>()
                    };
                    foreach (Match item in fundMatchs)
                    {
                        var fundCode = item.Groups[1].Value;
                        var fundName = item.Groups[2].Value;
                        company.Funds.Add(new FundRequest() { Code = fundCode, Name = fundName });
                    }
                    companys.Add(company);
                }
            }
            return companys;
        }

        private async Task<FundCompany> GetCompanyInfo(string url)
        {
            FundCompany company = null;
            var result = await Get(url);
            if (string.IsNullOrEmpty(result)) throw new NullReferenceException();
            var company_infoReg = new Regex("class=\"company-info\">([\\s\\S]+?)</div>");
            var info = company_infoReg.Match(result).Groups[1].Value;

            var category_nameReg = new Regex("class=\"category-name.+?>([\\s\\S]+?)</td>");
            var category_valueReg = new Regex("class=\"category-value.+?>([\\s\\S]+?)</td>");
            var names = category_nameReg.Matches(info);
            var values = category_valueReg.Matches(info);
            if (names.Count == values.Count)
            {
                company = new FundCompany() { Id = Guid.NewGuid().ToString("n") };
                for (int i = 0; i < names.Count; i++)
                {
                    var name = names[i].Groups[1].Value;
                    var value = values[i].Groups[1].Value;

                    if (name.Contains("经营范围"))
                    {
                        name = "经营范围";
                    }
                    else if (name.Contains("基金数量"))
                    {
                        name = "基金数量";
                        value = Regex.Match(value, "<a.+?>(.+?)</a>").Groups[1].Value;
                    }
                    else if (name.Contains("经理人数"))
                    {
                        value = Regex.Match(value, "<a.+?>(.+?)</a>").Groups[1].Value;
                    }

                    if (value == "---") value = string.Empty;
                    value = value.Trim();

                    //Console.WriteLine($"[{name}] {value}");

                    #region Set Value

                    if (name == "法定名称")
                        company.Name = value;
                    else if (name == "英文名称")
                        company.NameEN = value;
                    else if (name == "公司属性")
                        company.Attributes = value;
                    else if (name == "成立日期")
                        company.Establishment = value;
                    else if (name == "注册资本")
                        company.RegisteredCapital = value;
                    else if (name == "法人代表")
                        company.LegalRepresentative = value;
                    else if (name == "注册地址")
                        company.RegisteredAddress = value;
                    else if (name == "办公地址")
                        company.BusinessAddress = value;
                    else if (name == "网站地址")
                        company.WebSite = value;
                    else if (name == "电话号码")
                        company.PhoneNumber = value;
                    else if (name == "传真号码")
                        company.FaxNumber = value;
                    else if (name == "客服邮箱")
                        company.Email = value;
                    else if (name == "经营范围")
                        company.BusinessScope = value;
                    else if (name == "基金数量")
                        company.FundNumber = value;
                    else if (name == "管理规模")
                        company.AssetsUnderManagement = value;
                    else if (name == "经理人数")
                        company.ManagerNumber = value;

                    #endregion
                }
            }
            if (company == null || string.IsNullOrWhiteSpace(company.Name)) throw new NullReferenceException();
            return company;
        }

        private async Task<Fund> GetFundInfo(string url)
        {
            Fund fund = null;

            var result = await Get(url);
            if (string.IsNullOrEmpty(result)) throw new NullReferenceException();
            var txt_contReg = new Regex("class=\"txt_cont\">([\\s\\S]+?)</div>");
            var info = txt_contReg.Match(result).Groups[1].Value;
            info = info.Replace("<th>基金类型</th><td>", "</td><th>基金类型</th><td>").Replace("<th>份额规模</th><td>", "</td><th>份额规模</th><td>");

            var thReg = new Regex("th>([\\s\\S]+?)</th>");
            var tdReg = new Regex("td.*?>([\\s\\S]+?)</td>");
            var ths = thReg.Matches(info);
            var tds = tdReg.Matches(info);
            if (ths.Count == tds.Count)
            {
                fund = new Fund() { Id = Guid.NewGuid().ToString("n") };
                for (int i = 0; i < ths.Count; i++)
                {
                    var th = ths[i].Groups[1].Value;
                    var td = tds[i].Groups[1].Value;

                    if (td.Contains("<a"))
                    {
                        if (th == "份额规模")
                            td = GetPlainText(td);
                        else
                            td = Regex.Match(td, "<a.+?>(.+?)</a>").Groups[1].Value;
                    }
                    else if (td.Contains("<span"))
                    {
                        td = Regex.Match(td, "<span.+?>(.+?)</span>").Groups[1].Value;
                    }

                    //Console.WriteLine($"[{th}] {td}");

                    #region Set Value

                    if (th == "基金全称")
                        fund.FullName = td;
                    else if (th == "基金简称")
                        fund.Name = td;
                    else if (th == "基金代码")
                        fund.Code = td;
                    else if (th == "基金类型")
                    {
                        switch (td)
                        {
                            case "混合型":
                                fund.Type = (int)FundType.HybridFund;
                                break;
                            case "股票型":
                                fund.Type = (int)FundType.StockFund;
                                break;
                            case "货币型":
                                fund.Type = (int)FundType.MonetaryFund;
                                break;
                            case "债券型":
                                fund.Type = (int)FundType.BondFund;
                                break;
                        }
                    }
                    else if (th == "发行日期")
                        fund.IssueDate = td;
                    else if (th == "成立日期/规模")
                        fund.EstablishmentDate = td;
                    else if (th == "资产规模")
                        fund.AssetSize = td;
                    else if (th == "份额规模")
                        fund.ShareSize = td;
                    else if (th == "基金管理人")
                        fund.FundCompanyName = td;
                    else if (th == "基金托管人")
                        fund.FundRrustee = td;
                    else if (th == "基金经理人")
                        fund.FundManager = td;
                    else if (th == "成立来分红")
                        fund.Dividend = td;
                    else if (th == "管理费率")
                        fund.ManagementFeeRate = td;
                    else if (th == "托管费率")
                        fund.CustodianFeeRate = td;
                    else if (th == "销售服务费率")
                        fund.SalesServiceRate = td;
                    else if (th == "最高认购费率")
                        fund.MaximumSubscriptionRate = td;
                    else if (th == "最高申购费率")
                        fund.MaximunPurchaseRate = td;
                    else if (th == "最高赎回费率")
                        fund.MaximunRedemptionRate = td;
                    else if (th == "业绩比较基准")
                        fund.PerformanceBenchmark = td;

                    #endregion
                }
            }
            if (fund == null || string.IsNullOrWhiteSpace(fund.Name)) throw new NullReferenceException();
            return fund;
        }

        private async Task<IOPVData> GetFundIOPV(string code)
        {
            IOPVData data = null;
            var result = await GetFundIOPVs(code);
            if (result != null)
            {
                data = result.Where(x => x.FCODE == code).FirstOrDefault();
            }
            return data;
        }

        private async Task<List<IOPVData>> GetFundIOPVs(string code)
        {
            List<IOPVData> datas = null;
            var url = $"https://fundmobapi.eastmoney.com/FundMNewApi/FundMNFInfo?pageIndex=1&pageSize={code.Length}&appType=ttjj&product=EFund&plat=Android&deviceid={Guid.NewGuid().ToString("n")}&Version=1&Fcodes={code}";
            var result = await Get(url);
            if (string.IsNullOrEmpty(result)) throw new NullReferenceException();

            var response = Utils.JsonDeserialize<IOPVResponse>(result);
            if (response.ErrCode == 0 && response.Success)
            {
                datas = response.Datas;
            }
            return datas;
        }

        private async Task<string> Get(string url, string encoding = "utf-8")
        {
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Get, url);
                var client = _clientFactory.CreateClient();
                var response = await client.SendAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    var bytes = await response.Content.ReadAsByteArrayAsync();
                    return Encoding.GetEncoding(encoding).GetString(bytes);
                }
            }
            catch (Exception ex)
            {
                //
            }
            return await Task.FromResult(string.Empty);
        }

        private string GetPlainText(string text)
        {
            string plainText = text;

            if (!string.IsNullOrEmpty(plainText))
            {
                plainText = Regex.Replace(plainText, @"<script[^>]*?>[\s\S]*?</script>", string.Empty, RegexOptions.IgnoreCase);
                plainText = Regex.Replace(plainText, @"<!--[\s\S]*-->", string.Empty, RegexOptions.IgnoreCase);
                plainText = Regex.Replace(plainText, @"<[^>]*>", string.Empty, RegexOptions.IgnoreCase);
                plainText = HttpUtility.HtmlDecode(plainText);
                plainText = Regex.Replace(plainText, @"\s+", string.Empty, RegexOptions.IgnoreCase);
            }

            return plainText;
        }
    }
}
