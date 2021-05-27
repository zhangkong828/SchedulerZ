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
           // await QueryStock("茅台");
            await QueryStockQuotation("sh600519");
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

        private async Task<Tick> QueryStockQuotation(string key)
        {
            var url = $"http://hq.sinajs.cn/list={key}";
            var result = await Get(url, "GBK");
            if (string.IsNullOrEmpty(result)) throw new Exception();

            var text = result.Match("=\"(.+?)\"");
            if (string.IsNullOrEmpty(text)) throw new Exception();

            var stk = text.Split(",");
            var tick = new Tick();
            tick.Name = stk[0];
            tick.Open = stk[1].ToDouble();
            tick.Close = stk[2].ToDouble();
            tick.Now = stk[3].ToDouble();
            tick.High = stk[4].ToDouble();
            tick.Low = stk[5].ToDouble();
            //竞买价
            //竞卖价
            tick.Volume = stk[8].ToLong();
            tick.Amount = stk[9].ToDouble();
            tick.BidVolume1 = stk[10].ToLong();
            tick.Bid1 = stk[11].ToDouble();
            tick.BidVolume2 = stk[12].ToLong();
            tick.Bid2 = stk[13].ToDouble();
            tick.BidVolume3 = stk[14].ToLong();
            tick.Bid3 = stk[15].ToDouble();
            tick.BidVolume4 = stk[16].ToLong();
            tick.Bid4 = stk[17].ToDouble();
            tick.BidVolume5 = stk[18].ToLong();
            tick.Bid5 = stk[19].ToDouble();
            tick.AskVolume1 = stk[20].ToLong();
            tick.Ask1 = stk[21].ToDouble();
            tick.AskVolume2 = stk[22].ToLong();
            tick.Ask2 = stk[23].ToDouble();
            tick.AskVolume3 = stk[24].ToLong();
            tick.Ask3 = stk[25].ToDouble();
            tick.AskVolume4 = stk[26].ToLong();
            tick.Ask4 = stk[27].ToDouble();
            tick.AskVolume5 = stk[28].ToLong();
            tick.Ask5 = stk[29].ToDouble();
            tick.Date = stk[30].ToDateTime();
            tick.Time = stk[31].ToDateTime();
            return tick;
        }
    }
}
