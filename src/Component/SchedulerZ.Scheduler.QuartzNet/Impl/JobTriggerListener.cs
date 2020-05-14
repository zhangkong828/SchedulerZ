using Quartz;
using SchedulerZ.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SchedulerZ.Scheduler.QuartzNet.Impl
{
    public class JobTriggerListener : ITriggerListener
    {
        private readonly ILogger _logger;

        public string Name { get { return "SchedulerZ.JobTriggerListener"; } }

        public JobTriggerListener(ILogger logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Trigger命中时
        /// </summary>
        /// <param name="trigger"></param>
        /// <param name="context"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task TriggerFired(ITrigger trigger, IJobExecutionContext context, CancellationToken cancellationToken = default)
        {
            var jobId = trigger.JobKey.Name;
            _logger.Debug($"{jobId}开始");
            return Task.CompletedTask;
        }

        /// <summary>
        /// 否决Job执行,Trigger触发后，Job执行前调用本方法
        /// 如果返回true,则Job不执行
        /// </summary>
        /// <param name="trigger"></param>
        /// <param name="context"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<bool> VetoJobExecution(ITrigger trigger, IJobExecutionContext context, CancellationToken cancellationToken = default)
        {
            var jobId = trigger.JobKey.Name;
            _logger.Debug($"{jobId}执行前");
            return Task.FromResult(false);
        }

        /// <summary>
        /// Trigger完成
        /// </summary>
        /// <param name="trigger"></param>
        /// <param name="context"></param>
        /// <param name="triggerInstructionCode"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task TriggerComplete(ITrigger trigger, IJobExecutionContext context, SchedulerInstruction triggerInstructionCode, CancellationToken cancellationToken = default)
        {
            var jobId = trigger.JobKey.Name;
            _logger.Debug($"{jobId}完成");
            return Task.CompletedTask;
        }


        /// <summary>
        /// Trigger错过触发
        /// </summary>
        /// <param name="trigger"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task TriggerMisfired(ITrigger trigger, CancellationToken cancellationToken = default)
        {
            var jobId = trigger.JobKey.Name;
            _logger.Debug($"{jobId}错过");
            return Task.CompletedTask;
        }
    }
}
