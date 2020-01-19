using Quartz;
using Quartz.Impl;
using Quartz.Spi;
using SchedulerZ.Component;
using SchedulerZ.Logging;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Threading.Tasks;

namespace SchedulerZ.Core.Scheduler
{
    public class SchedulerManager
    {
        private static readonly object _lock = new object();
        private static IScheduler _scheduler;

        private readonly ILogger _logger = Configuration.LoggerProvider.CreateLogger("SchedulerManager");

        public static IScheduler Scheduler
        {
            get
            {
                if (_scheduler == null)
                {
                    lock (_lock)
                    {
                        if (_scheduler == null)
                        {
                            InitScheduler();
                        }
                    }
                }
                return _scheduler;
            }
        }
        private SchedulerManager() { }

        private static void InitScheduler()
        {
            NameValueCollection properties = new NameValueCollection();
            properties["quartz.scheduler.instanceName"] = "SchedulerZ.SchedulerManager";
            properties["quartz.threadPool.type"] = "Quartz.Simpl.SimpleThreadPool, Quartz";
            properties["quartz.threadPool.threadCount"] = "50";
            properties["quartz.threadPool.threadPriority"] = "Normal";

            ISchedulerFactory factory = new StdSchedulerFactory(properties);
            _scheduler = factory.GetScheduler().GetAwaiter().GetResult();
        }


        public void StartScheduler()
        {
            if (!_scheduler.IsStarted)
            {
                _scheduler.ListenerManager.AddTriggerListener(new JobTriggerListener());
                _scheduler.Start();
            }
        }

        public void Shutdown(bool waitForJobsToComplete = false)
        {
            try
            {
                if (!_scheduler.IsShutdown)
                {
                    _scheduler.Shutdown(waitForJobsToComplete);
                    _scheduler = null;
                }
            }
            catch (Exception ex)
            {
                _logger.Fatal("SchedulerManager.Shutdown", ex);
            }
        }

        /// <summary>
        /// 启动Job
        /// </summary>
        public void StartJob()
        {

        }

        /// <summary>
        /// 暂停Job
        /// </summary>
        public async Task<bool> PauseJob(string jobId)
        {
            JobKey jk = new JobKey(jobId);
            if (await _scheduler.CheckExists(jk))
            {
                await _scheduler.PauseJob(jk);
                //var jobDetail = await _scheduler.GetJobDetail(jk);
                //if (jobDetail.JobType.GetInterface("IInterruptableJob") != null)
                //{
                //    await _scheduler.Interrupt(jk);
                //}
                _logger.Info($"Job[{jobId}] is paused");
                return true;
            }
            return false;
        }

        /// <summary>
        /// 恢复Job
        /// </summary>
        public async Task<bool> ResumeJob(string jobId)
        {
            JobKey jk = new JobKey(jobId);
            if (await _scheduler.CheckExists(jk))
            {
                await _scheduler.ResumeJob(jk);
                _logger.Info($"Job[{jobId}] is resumed");
                return true;
            }
            return false;
        }

        /// <summary>
        /// 停止Job
        /// </summary>
        public async Task<bool> StopJob(string jobId)
        {
            JobKey jk = new JobKey(jobId);
            if (await _scheduler.CheckExists(jk))
            {
                var tk = new TriggerKey(jobId);
                await _scheduler.UnscheduleJob(tk);
                _logger.Info($"Job[{jobId}] is stop");
                return true;
            }
            return false;
        }

        /// <summary>
        /// 删除Job
        /// </summary>
        public async Task<bool> DeleteJob(string jobId)
        {
            JobKey jk = new JobKey(jobId);
            if (await _scheduler.CheckExists(jk))
            {
                var tk = new TriggerKey(jobId);
                await _scheduler.UnscheduleJob(tk);
                await _scheduler.DeleteJob(jk);
                //卸载Job  TODO

                _logger.Info($"Job[{jobId}] is delete");
                return true;
            }
            return false;
        }

        /// <summary>
        /// 立即运行一次Job
        /// </summary>
        public async Task<bool> RunJobOnceNow(string jobId)
        {
            JobKey jk = new JobKey(jobId);
            if (await _scheduler.CheckExists(jk))
            {
                await _scheduler.TriggerJob(jk);
                return true;
            }
            return false;
        }


        /// <summary>
        /// 校验字符串是否为正确的Cron表达式
        /// </summary>
        /// <param name="cronExpression">带校验表达式</param>
        /// <returns></returns>
        public static bool ValidExpression(string cronExpression)
        {
            return CronExpression.IsValidExpression(cronExpression);
        }

        /// <summary>
        /// 获取未来几次运行的运行时间
        /// </summary>
        /// <param name="CronExpressionString">Cron表达式</param>
        /// <param name="numTimes">运行次数</param>
        /// <returns>运行时间段</returns>
        public static List<DateTime> GetTaskeFireTime(string CronExpressionString, int numTimes)
        {
            List<DateTime> list = new List<DateTime>();
            if (numTimes < 0) return list;

            ITrigger trigger = TriggerBuilder.Create().WithCronSchedule(CronExpressionString).Build();
            IReadOnlyList<DateTimeOffset> dates = TriggerUtils.ComputeFireTimes(trigger as IOperableTrigger, null, numTimes);

            foreach (DateTimeOffset dtf in dates)
            {
                list.Add(TimeZoneInfo.ConvertTimeFromUtc(dtf.DateTime, TimeZoneInfo.Local));
            }
            return list;
        }
    }
}
