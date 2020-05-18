using SchedulerZ.gRPC;
using SchedulerZ.Models;
using SchedulerZ.Route;
using SchedulerZ.Utility;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SchedulerZ.Remoting.gRPC.Client
{
    public class SchedulerRemoting : ISchedulerRemoting
    {
        private readonly IGrpcClientFactory<SchedulerService.SchedulerServiceClient> _clientFactory;
        public SchedulerRemoting(IGrpcClientFactory<SchedulerService.SchedulerServiceClient> clientFactory)
        {
            _clientFactory = clientFactory;
        }

        public Task<bool> StartJob(JobEntity job)
        {
            var service = new ServiceRouteDescriptor()
            {
                Name = "test"
            };

            var jobRequest = Utils.MapperPropertyValue<JobEntity, Job>(job);
            var client = _clientFactory.Get(service);
            var response = client.StartJob(jobRequest);
            return Task.FromResult(response.Success);
        }

        public Task<bool> PauseJob(string jobId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> ResumeJob(string jobId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> StopJob(string jobId)
        {
            throw new NotImplementedException();
        }
        public Task<bool> DeleteJob(string jobId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> RunJobOnceNow(string jobId)
        {
            throw new NotImplementedException();
        }
    }
}
