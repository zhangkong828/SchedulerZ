using Quartz;
using Quartz.Impl.Triggers;
using SchedulerZ.Logging;
using SchedulerZ.Models;
using SchedulerZ.Scheduler.QuartzNet.Impl;
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
        public SchedulerManager(QuartzNetConfig config, IScheduler scheduler)
        {
            _config = config;
            _scheduler = scheduler;
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
                _logger.Error($"Start Job Fail", ex);
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
                await _scheduler.DeleteJob(jk);
                _logger.Info($"Job[{jobId}] is stop");
                return true;
            }
            return false;
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
                var triggerBuilder = TriggerBuilder.Create().WithIdentity(jobView.Id).WithSimpleSchedule(x => x.WithRepeatCount(jobView.RepeatCount).WithInterval(TimeSpan.FromSeconds(jobView.IntervalSeconds)));

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
