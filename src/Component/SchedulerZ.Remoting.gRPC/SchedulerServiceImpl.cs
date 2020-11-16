using Grpc.Core;
using SchedulerZ.gRPC;
using SchedulerZ.Models;
using SchedulerZ.Scheduler;
using SchedulerZ.Utility;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SchedulerZ.Remoting.gRPC
{
    public class SchedulerServiceImpl : SchedulerZ.gRPC.SchedulerService.SchedulerServiceBase
    {
        private readonly ISchedulerManager _schedulerManager;
        public SchedulerServiceImpl(ISchedulerManager schedulerManager)
        {
            _schedulerManager = schedulerManager;
        }

        public override async Task<SchedulerResponse> StartJob(Job request, ServerCallContext context)
        {
            var jobEntity = Utils.MapperPropertyValue<Job, JobEntity>(request);
            var result = await _schedulerManager.StartJob(jobEntity);
            return new SchedulerResponse() { Success = result };
        }

        public override async Task<SchedulerResponse> PauseJob(Job request, ServerCallContext context)
        {
            var result = await _schedulerManager.PauseJob(request.Id);
            return new SchedulerResponse() { Success = result };
        }

        public override async Task<SchedulerResponse> ResumeJob(Job request, ServerCallContext context)
        {
            var result = await _schedulerManager.ResumeJob(request.Id);
            return new SchedulerResponse() { Success = result };
        }

        public override async Task<SchedulerResponse> StopJob(Job request, ServerCallContext context)
        {
            var result = await _schedulerManager.StopJob(request.Id);
            return new SchedulerResponse() { Success = result };
        }

        public override async Task<SchedulerResponse> RunJobOnceNow(Job request, ServerCallContext context)
        {
            var result = await _schedulerManager.RunJobOnceNow(request.Id);
            return new SchedulerResponse() { Success = result };
        }
    }
}
