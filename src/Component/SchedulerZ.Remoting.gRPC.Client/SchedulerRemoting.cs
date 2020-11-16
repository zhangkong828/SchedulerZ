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

        public Task<bool> StartJob(JobEntity job, ServiceRouteDescriptor service)
        {
            var client = _clientFactory.Get(service);

            var jobRequest = Utils.MapperPropertyValue<JobEntity, Job>(job);
            var response = client.StartJob(jobRequest);
            return Task.FromResult(response.Success);
        }

        public Task<bool> PauseJob(string jobId, ServiceRouteDescriptor service)
        {
            var client = _clientFactory.Get(service);

            var response = client.PauseJob(new Job() { Id = jobId });
            return Task.FromResult(response.Success);
        }

        public Task<bool> ResumeJob(string jobId, ServiceRouteDescriptor service)
        {
            var client = _clientFactory.Get(service);

            var response = client.ResumeJob(new Job() { Id = jobId });
            return Task.FromResult(response.Success);
        }

        public Task<bool> StopJob(string jobId, ServiceRouteDescriptor service)
        {
            var client = _clientFactory.Get(service);

            var response = client.StopJob(new Job() { Id = jobId });
            return Task.FromResult(response.Success);
        }

        public Task<bool> RunJobOnceNow(string jobId, ServiceRouteDescriptor service)
        {
            var client = _clientFactory.Get(service);

            var response = client.RunJobOnceNow(new Job() { Id = jobId });
            return Task.FromResult(response.Success);
        }
    }
}
