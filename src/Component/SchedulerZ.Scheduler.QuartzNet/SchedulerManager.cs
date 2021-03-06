﻿using Quartz;
using Quartz.Impl.Matchers;
using Quartz.Impl.Triggers;
using SchedulerZ.Logging;
using SchedulerZ.Models;
using SchedulerZ.Route;
using SchedulerZ.Scheduler.QuartzNet.Impl;
using SchedulerZ.Store;
using SchedulerZ.Utility;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SchedulerZ.Scheduler.QuartzNet
{
    public class SchedulerManager : ISchedulerManager
    {
        private readonly ILogger _logger = TraceLogger.GetLogger();
        private readonly QuartzNetConfig _config;
        private readonly IScheduler _scheduler;
        private readonly IJobStore _jobStore;
        private readonly IServiceRoute _serviceRoute;
        public SchedulerManager(QuartzNetConfig config, IScheduler scheduler, IJobStore jobStore, IServiceRoute serviceRoute)
        {
            _config = config;
            _scheduler = scheduler;
            _jobStore = jobStore;
            _serviceRoute = serviceRoute;
        }

        public bool ValidExpression(string cronExpression)
        {
            return CronExpression.IsValidExpression(cronExpression);
        }

        public async Task<bool> PauseJob(string jobId)
        {
            JobKey jk = new JobKey(jobId);
            if (await _scheduler.CheckExists(jk))
            {
                await _scheduler.PauseJob(jk);
                _logger.Info($"job[{jobId}] is paused");
                return true;
            }
            return false;
        }

        public async Task<bool> ResumeJob(string jobId)
        {
            JobKey jk = new JobKey(jobId);
            if (await _scheduler.CheckExists(jk))
            {
                await _scheduler.ResumeJob(jk);
                _logger.Info($"job[{jobId}] is resumed");
                return true;
            }
            return false;
        }

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

        public async Task<bool> StartJob(JobEntity job)
        {
            if (!Validate(job))
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
                _logger.Error($"start job fail", ex);
                return false;
            }
        }

        public async Task<bool> StopJob(string jobId)
        {
            JobKey jk = new JobKey(jobId);
            if (await _scheduler.CheckExists(jk))
            {
                var job = await _scheduler.GetJobDetail(jk);
                var jobRuntime = job.JobDataMap["JobRuntime"] as JobRuntime;
                if (jobRuntime != null)
                {
                    jobRuntime.Instance?.Dispose();
                    jobRuntime.Dispose();
                }
                var result = await _scheduler.DeleteJob(jk);
                var removeResult = _scheduler.ListenerManager.RemoveJobListener(jk.Name);
                if (result && removeResult)
                    _logger.Info($"job[{jobId}] is stop");
                else
                {
                    if (result)
                        _logger.Info($"job[{jobId}] is stop. but job listener remove fail");
                }
                return result;
            }
            return true;
        }


        private bool Validate(JobEntity job)
        {
            if (string.IsNullOrWhiteSpace(job.Name))
                return false;

            if (string.IsNullOrWhiteSpace(job.AssemblyName) || string.IsNullOrWhiteSpace(job.ClassName))
                return false;

            if (job.IsSimple)
            {
                if (job.RepeatCount != -1 && job.RepeatCount <= 0)
                    return false;

                if (job.IntervalSeconds < 0)
                    return false;
            }
            else
            {
                if (string.IsNullOrWhiteSpace(job.CronExpression))
                    return false;
                return ValidExpression(job.CronExpression);
            }

            return true;
        }


        private async Task Start(JobEntity jobView)
        {
            var jobRuntime = await JobFactory.CreateJobRuntime(_serviceRoute, jobView);

            JobDataMap map = new JobDataMap
            {
                new KeyValuePair<string, object> ("JobRuntime",jobRuntime)
            };

            IJobDetail job = JobBuilder.Create().OfType(typeof(JobImplementation)).WithIdentity(jobView.Id).UsingJobData(map).Build();

            _scheduler.ListenerManager.AddJobListener(new JobListener(jobView.Id, JobWasExecuteCallBack), KeyMatcher<JobKey>.KeyEquals(new JobKey(jobView.Id)));

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
                    trigger.StartTimeUtc = Utils.ConvertToDateTimeOffset(jobView.StartTime.Value);
                }
                if (jobView.EndTime.HasValue)
                {
                    trigger.EndTimeUtc = Utils.ConvertToDateTimeOffset(jobView.EndTime.Value);
                }
                await _scheduler.ScheduleJob(job, trigger);
            }
            else
            {
                var triggerBuilder = TriggerBuilder.Create().WithIdentity(jobView.Id).WithSimpleSchedule(x => x.WithRepeatCount(jobView.RepeatCount).WithInterval(TimeSpan.FromSeconds(jobView.IntervalSeconds)));

                if (jobView.StartTime.HasValue)
                {
                    var start = Utils.ConvertToDateTimeOffset(jobView.StartTime.Value);
                    triggerBuilder = triggerBuilder.StartAt(start);
                }
                if (jobView.EndTime.HasValue)
                {
                    var end = Utils.ConvertToDateTimeOffset(jobView.EndTime.Value);
                    triggerBuilder = triggerBuilder.EndAt(end);
                }

                var trigger = triggerBuilder.Build();
                await _scheduler.ScheduleJob(job, trigger);
            }

            _logger.Info($"job [{jobView.Id}]{jobView.Name} start success");

        }


        private void JobWasExecuteCallBack(IJobExecutionContext context)
        {
            _jobStore.UpdateRunJob(context.JobDetail.Key.Name, context.FireTimeUtc, context.NextFireTimeUtc);
        }
    }
}
