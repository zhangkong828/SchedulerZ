using SchedulerZ.Job.FundTest.Model;
using System;
using System.Collections.Generic;
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
            //Console.WriteLine(result);
            //await GetCompanyInfo("http://fund.eastmoney.com/Company/f10/jbgk_80000225.html");
            await GetFundInfo("http://fundf10.eastmoney.com/jbgk_003561.html");
        }

        private async Task<List<CompanyRequest>> GetAllFund()
        {
            var companys = new List<CompanyRequest>();

            var url = "http://fund.eastmoney.com/allfund_com.html";
            var result = await Get(url);

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

        private async Task GetCompanyInfo(string url)
        {
            var result = await Get(url, "utf-8");

            var company_infoReg = new Regex("class=\"company-info\">([\\s\\S]+?)</div>");
            var info = company_infoReg.Match(result).Groups[1].Value;

            var category_nameReg = new Regex("class=\"category-name.+?>([\\s\\S]+?)</td>");
            var category_valueReg = new Regex("class=\"category-value.+?>([\\s\\S]+?)</td>");
            var names = category_nameReg.Matches(info);
            var values = category_valueReg.Matches(info);
            if (names.Count == values.Count)
            {
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

                    Console.WriteLine($"[{name}] {value}");
                }
            }
        }

        private async Task GetFundInfo(string url)
        {
            var result = await Get(url, "utf-8");
            var txt_contReg = new Regex("class=\"txt_cont\">([\\s\\S]+?)</div>");
            var info = txt_contReg.Match(result).Groups[1].Value;
            info = info.Replace("<th>基金类型</th><td>", "</td><th>基金类型</th><td>").Replace("<th>份额规模</th><td>", "</td><th>份额规模</th><td>");

            var thReg = new Regex("th>([\\s\\S]+?)</th>");
            var tdReg = new Regex("td.*?>([\\s\\S]+?)</td>");
            var ths = thReg.Matches(info);
            var tds = tdReg.Matches(info);
            if (ths.Count == tds.Count)
            {
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

                    Console.WriteLine($"[{th}] {td}");
                }
            }
        }

        private async Task<string> Get(string url, string encoding = "GB2312")
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
                Console.WriteLine(ex);
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
