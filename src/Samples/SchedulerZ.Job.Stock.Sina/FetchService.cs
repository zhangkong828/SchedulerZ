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
    public class FetchService: IFetchService
    {
        private readonly IHttpClientFactory _clientFactory;

        public FetchService(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        public Task GetPage()
        {
            throw new NotImplementedException();
        }
    }
}
