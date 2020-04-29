﻿using Quartz;
using Quartz.Impl;
using Quartz.Impl.Triggers;
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

        private static SchedulerManager _schedulerManager;
        public static SchedulerManager Scheduler
        {
            get
            {
                if (_schedulerManager == null)
                {
                    lock (_lock)
                    {
                        if (_schedulerManager == null)
                        {
                            _schedulerManager = new SchedulerManager();
                            InitScheduler();
                        }
                    }
                }
                return _schedulerManager;
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
        public async Task<bool> StartJob(JobView job)
        {
            if (!job.Validate())
            {
                return false;
            }

            try
            {
                await Start(job);
                return true;
            }
            catch (Exception ex)
            {
                _logger.Error($"Start Job Fail", ex);
                return false;
            }
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



        private async Task Start(JobView jobView)
        {
            var jobRuntime = JobFactory.CreateJobRuntime(jobView);

            JobDataMap map = new JobDataMap
            {
                new KeyValuePair<string, object> ("JobRuntime",jobRuntime)
            };

            IJobDetail job = JobBuilder.Create().OfType(typeof(JobImplementation)).WithIdentity(jobView.Id).UsingJobData(map).Build();

            //添加触发器
            //_scheduler.ListenerManager.AddJobListener(new JobRunListener(view.Schedule.Id.ToString(), callBack),KeyMatcher<JobKey>.KeyEquals(new JobKey(view.Schedule.Id.ToString())));

            if (!jobView.IsSimple)
            {
                if (!ValidExpression(jobView.CronExpression))
                {
                    throw new Exception("cron表达式验证失败");
                }
                var trigger = new CronTriggerImpl
                {
                    CronExpressionString = jobView.CronExpression,
                    Name = jobView.Name,
                    Key = new TriggerKey(jobView.Id),
                    Description = jobView.Remark
                };
                if (jobView.StartTime.HasValue)
                {
                    trigger.StartTimeUtc = TimeZoneInfo.ConvertTimeToUtc(jobView.StartTime.Value);
                }
                if (jobView.EndTime.HasValue)
                {
                    trigger.EndTimeUtc = TimeZoneInfo.ConvertTimeToUtc(jobView.EndTime.Value);
                }
                await _scheduler.ScheduleJob(job, trigger);
            }
            else
            {
                var triggerBuilder = TriggerBuilder.Create().WithIdentity(jobView.Id).WithSimpleSchedule(x => x.WithRepeatCount(jobView.RepeatCount).WithInterval(jobView.IntervalTimeSpan));

                if (jobView.StartTime.HasValue)
                {
                    var start = TimeZoneInfo.ConvertTimeToUtc(jobView.StartTime.Value);
                    triggerBuilder = triggerBuilder.StartAt(start);
                }
                if (jobView.EndTime.HasValue)
                {
                    var end = TimeZoneInfo.ConvertTimeToUtc(jobView.EndTime.Value);
                    triggerBuilder = triggerBuilder.EndAt(end);
                }

                var trigger = triggerBuilder.Build();
                await _scheduler.ScheduleJob(job, trigger);
            }

            _logger.Info($"job [{jobView.Id}]{jobView.Name} start success");

        }
    }
}
