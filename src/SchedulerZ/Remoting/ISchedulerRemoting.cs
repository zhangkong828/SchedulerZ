using SchedulerZ.Models;
using SchedulerZ.Route;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SchedulerZ.Remoting
{
    public interface ISchedulerRemoting
    {
        Task<bool> StartJob(JobEntity job, ServiceRouteDescriptor service);

        Task<bool> PauseJob(string jobId, ServiceRouteDescriptor service);

        Task<bool> ResumeJob(string jobId, ServiceRouteDescriptor service);

        Task<bool> StopJob(string jobId, ServiceRouteDescriptor service);
        
        Task<bool> RunJobOnceNow(string jobId, ServiceRouteDescriptor service);
    }
}
