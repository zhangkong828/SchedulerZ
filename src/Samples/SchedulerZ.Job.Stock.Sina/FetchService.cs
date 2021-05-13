using SchedulerZ.Job.Stock.Sina.Entity;
using SchedulerZ.Utility;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SchedulerZ.Job.Stock.Sina
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

        private async Task<string> Get(string url, string encoding = "utf-8")
        {
            int tryCount = 3;
            tryGet:
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
                if (tryCount <= 0)
                {
                    Console.WriteLine(ex);
                    return await Task.FromResult(string.Empty);
                }
                tryCount--;
                await Task.Delay(new Random().Next(1, 4) * 1000);
                goto tryGet;
            }
            return await Task.FromResult(string.Empty);
        }

        public async Task GetPage()
        {
            await QueryStock("茅台");
        }

        private async Task<List<Instrument>> QueryStock(string key)
        {
            var list = new List<Instrument>();
            var url = $"http://suggest3.sinajs.cn/suggest/type=&key={key}&name=callbacksuggestdata";
            var result = await Get(url, "GBK");
            if (string.IsNullOrEmpty(result)) throw new Exception();

            var text = result.Match("=\"(.+?)\"");
            if (string.IsNullOrEmpty(text)) throw new Exception();

            foreach (var item in text.Split(";", StringSplitOptions.RemoveEmptyEntries))
            {
                var stk = item.Split(",");
                var instrument = new Instrument();
                instrument.Code = stk[2];
                instrument.Symbol = stk[3];
                instrument.Name = stk[6];
                list.Add(instrument);
            }

            return list;
        }
    }
}
