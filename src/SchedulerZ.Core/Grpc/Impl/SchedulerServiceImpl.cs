using Grpc.Core;
using SchedulerZ.Core.Scheduler;
using SchedulerZ.gRPC;
using SchedulerZ.Utility;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SchedulerZ.Core.Grpc.Impl
{
    public class SchedulerServiceImpl : SchedulerZ.gRPC.SchedulerService.SchedulerServiceBase
    {
        public override async Task<SchedulerResult> StartJob(Job request, ServerCallContext context)
        {
            var jobView = Utils.MapperPropertyValue<Job, JobView>(request);
            var result = await SchedulerManager.Scheduler.StartJob(jobView);
            return new SchedulerResult() { Success = result };
        }

        public override async Task<SchedulerResult> PauseJob(Job request, ServerCallContext context)
        {
            var result = await SchedulerManager.Scheduler.PauseJob(request.Id);
            return new SchedulerResult() { Success = result };
        }

        public override async Task<SchedulerResult> ResumeJob(Job request, ServerCallContext context)
        {
            var result = await SchedulerManager.Scheduler.ResumeJob(request.Id);
            return new SchedulerResult() { Success = result };
        }

        public override async Task<SchedulerResult> StopJob(Job request, ServerCallContext context)
        {
            var result = await SchedulerManager.Scheduler.StopJob(request.Id);
            return new SchedulerResult() { Success = result };
        }

        public override async Task<SchedulerResult> DeleteJob(Job request, ServerCallContext context)
        {
            var result = await SchedulerManager.Scheduler.DeleteJob(request.Id);
            return new SchedulerResult() { Success = result };
        }

        public override async Task<SchedulerResult> RunJobOnceNow(Job request, ServerCallContext context)
        {
            var result = await SchedulerManager.Scheduler.RunJobOnceNow(request.Id);
            return new SchedulerResult() { Success = result };
        }
    }
}
