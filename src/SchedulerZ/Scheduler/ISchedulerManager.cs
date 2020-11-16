using SchedulerZ.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SchedulerZ.Scheduler
{
    public interface ISchedulerManager
    {
        Task<bool> StartJob(JobEntity job);

        Task<bool> PauseJob(string jobId);

        Task<bool> ResumeJob(string jobId);

        Task<bool> StopJob(string jobId);
        
        Task<bool> RunJobOnceNow(string jobId);

        bool ValidExpression(string cronExpression);
    }
}
