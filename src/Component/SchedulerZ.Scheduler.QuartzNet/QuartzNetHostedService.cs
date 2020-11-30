using Microsoft.Extensions.Hosting;
using Quartz;
using SchedulerZ.Logging;
using SchedulerZ.Scheduler.QuartzNet.Impl;
using SchedulerZ.Store;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SchedulerZ.Scheduler.QuartzNet
{
    public class QuartzNetHostedService : IHostedService
    {
        private readonly ILogger _logger = TraceLogger.GetLogger();
        private readonly QuartzNetConfig _config;
        private readonly IScheduler _scheduler;
        private readonly IJobStore _jobStore;
        private readonly ISchedulerManager _schedulerManager;
        public QuartzNetHostedService(QuartzNetConfig config, IScheduler scheduler, IJobStore jobStore, ISchedulerManager schedulerManager)
        {
            _config = config;
            _scheduler = scheduler;
            _jobStore = jobStore;
            _schedulerManager = schedulerManager;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            if (!_scheduler.IsStarted)
            {
                _scheduler.ListenerManager.AddTriggerListener(new JobTriggerListener(_logger));

                await _scheduler.Start(cancellationToken);
                await _scheduler.Clear();

                //恢复任务
                RecoveryRunning();
            }
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            try
            {
                if (!_scheduler.IsShutdown)
                {
                    await _scheduler.Shutdown(cancellationToken);
                }
            }
            catch (Exception ex)
            {
                _logger.Fatal("SchedulerManager.Shutdown", ex);
            }
        }



        private void RecoveryRunning()
        {
            //查询绑定了本节点且在运行中的任务
            var jobs = _jobStore.QueryRunningJob(Config.NodeHost, Config.NodePort);
            jobs.AsParallel().ForAll(async job =>
            {
                await _schedulerManager.StartJob(job);
            });
        }
    }
}
