using SchedulerZ.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SchedulerZ.Scheduler.QuartzNet
{
    public class SchedulerManager : ISchedulerManager
    {
        public Task<bool> DeleteJob(string jobId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> PauseJob(string jobId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> ResumeJob(string jobId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> RunJobOnceNow(string jobId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> StartJob(JobEntity job)
        {
            throw new NotImplementedException();
        }

        public Task<bool> StopJob(string jobId)
        {
            throw new NotImplementedException();
        }
    }
}
