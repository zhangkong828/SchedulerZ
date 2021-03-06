﻿using SchedulerZ.Models;
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

        Task<bool> UploadFile(string filePath, ServiceRouteDescriptor service);

        Task<bool> UploadFile(List<string> filePaths, ServiceRouteDescriptor service);

        Task<bool> DownloadFile(string fileName, string saveDirectoryPath, ServiceRouteDescriptor service);

        Task<bool> DownloadFile(List<string> fileNames, string saveDirectoryPath, ServiceRouteDescriptor service);
    }
}
