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
        public async Task<bool> StartJob()
        {
            JobAssemblyLoadContext jalc = null;
            try
            {
                jalc = JobFactory.LoadAssemblyContext("assemblyName");
                try
                {
                    await Start(view, lc, callBack);
                    return true;
                }
                catch (SchedulerException sexp)
                {
                    //_logger.Error($"任务启动失败！开始第{i + 1}次重试...", sexp, view.Schedule.Id);
                }
                //最后一次尝试
                await Start(view, lc, callBack);
                return true;
            }
            catch (SchedulerException sex)
            {
                JobFactory.UnLoadAssemblyContext(jalc);
                //LogHelper.Error($"任务所有重试都失败了，已放弃启动！", sexp, view.Schedule.Id);
                return false;
            }
            catch (Exception exp)
            {
                JobFactory.UnLoadAssemblyContext(jalc);
                //LogHelper.Error($"任务启动失败！", exp, view.Schedule.Id);
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

        private async Task Start(JobAssemblyLoadContext jalc)
        {
            //throw new SchedulerException("SchedulerException");

            var id = Guid.NewGuid();
            //在应用程序域中创建实例返回并保存在job中，这是最终调用任务执行的实例
            JobBase instance = JobFactory.CreateJobInstance(jalc, id, "AssemblyName", "ClassName");
            if (instance == null)
            {
                throw new InvalidCastException($"任务实例创建失败，请确认目标任务是否派生自JobBase类型。程序集：AssemblyName，类型：ClassName");
            }
            // instance.logger = new LogWriter(); ;
            JobDataMap map = new JobDataMap
            {
                new KeyValuePair<string, object> ("Domain",jalc),
                new KeyValuePair<string, object> ("Instance",instance),
                //new KeyValuePair<string, object> ("name",view.Schedule.Title),
                //new KeyValuePair<string, object> ("Params",ConvertParamsJson(view.Schedule.CustomParamsJson)),
                //new KeyValuePair<string, object> ("keepers",view.Keepers),
                //new KeyValuePair<string, object> ("children",view.Children)
            };
            IJobDetail job = JobBuilder.Create<JobCommon>()
                .WithIdentity(id.ToString())
                .UsingJobData(map)
                .Build();

            //添加触发器
            //_scheduler.ListenerManager.AddJobListener(new JobRunListener(view.Schedule.Id.ToString(), callBack),KeyMatcher<JobKey>.KeyEquals(new JobKey(view.Schedule.Id.ToString())));

            if (view.Schedule.RunLoop)
            {
                if (!CronExpression.IsValidExpression(view.Schedule.CronExpression))
                {
                    throw new Exception("cron表达式验证失败");
                }
                CronTriggerImpl trigger = new CronTriggerImpl
                {
                    CronExpressionString = view.Schedule.CronExpression,
                    Name = view.Schedule.Title,
                    Key = new TriggerKey(view.Schedule.Id.ToString()),
                    Description = view.Schedule.Remark
                };
                if (view.Schedule.StartDate.HasValue)
                {
                    trigger.StartTimeUtc = TimeZoneInfo.ConvertTimeToUtc(view.Schedule.StartDate.Value);
                }
                if (view.Schedule.EndDate.HasValue)
                {
                    trigger.EndTimeUtc = TimeZoneInfo.ConvertTimeToUtc(view.Schedule.EndDate.Value);
                }
                await _scheduler.ScheduleJob(job, trigger);
            }
            else
            {
                DateTimeOffset start = TimeZoneInfo.ConvertTimeToUtc(DateTime.Now);
                if (view.Schedule.StartDate.HasValue)
                {
                    start = TimeZoneInfo.ConvertTimeToUtc(view.Schedule.StartDate.Value);
                }
                DateTimeOffset end = start.AddMinutes(1);
                if (view.Schedule.EndDate.HasValue)
                {
                    end = TimeZoneInfo.ConvertTimeToUtc(view.Schedule.EndDate.Value);
                }
                ITrigger trigger = TriggerBuilder.Create()
                    .WithIdentity(view.Schedule.Id.ToString())
                    .StartAt(start)
                    .WithSimpleSchedule(x => x
                    .WithRepeatCount(1).WithIntervalInMinutes(1))
                    .EndAt(end)
                    .Build();
                await _scheduler.ScheduleJob(job, trigger);
            }

            //LogHelper.Info($"任务[{view.Schedule.Title}]启动成功！", view.Schedule.Id);

        }
    }
}
