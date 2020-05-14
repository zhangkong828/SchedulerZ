using Microsoft.Extensions.Hosting;
using Quartz;
using SchedulerZ.Logging;
using SchedulerZ.Scheduler.QuartzNet.Impl;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SchedulerZ.Scheduler.QuartzNet
{
    public class QuartzNetHostedService : IHostedService
    {
        private readonly ILogger _logger;
        private readonly IScheduler _scheduler;
        public QuartzNetHostedService(IScheduler scheduler, ILoggerProvider loggerProvider)
        {
            _scheduler = scheduler;
            _logger = loggerProvider.CreateLogger("QuartzNetHostedService");
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            if (!_scheduler.IsStarted)
            {
                _scheduler.ListenerManager.AddTriggerListener(new JobTriggerListener(_logger));
                await _scheduler.Start(cancellationToken);
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
    }
}
