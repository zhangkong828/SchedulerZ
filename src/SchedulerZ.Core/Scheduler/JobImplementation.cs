using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Quartz;

namespace SchedulerZ.Core.Scheduler
{
    [PersistJobDataAfterExecution]
    [DisallowConcurrentExecution]
    public class JobImplementation : IJob
    {
        public JobImplementation()
        {
        }

        public Task Execute(IJobExecutionContext context)
        {
            IJobDetail job = context.JobDetail;
            try
            {
                if (job.JobDataMap["JobRuntime"] is JobRuntime jobRuntime)
                {
                    //Guid traceId = GreateRunTrace();
                    Stopwatch stopwatch = new Stopwatch();
                    JobContext jobContext = new JobContext(jobRuntime.Instance);
                    //tctx.Node = node;
                    //tctx.TaskId = _sid;
                    //tctx.TraceId = traceId;
                    jobContext.JobDataMap = job.JobDataMap["Params"] as Dictionary<string, object>;
                    //if (context.MergedJobDataMap["PreviousResult"] is object prev)
                    //{
                    //    tctx.PreviousResult = prev;
                    //}
                    try
                    {
                        stopwatch.Restart();
                        jobRuntime.Execute(jobContext);
                        stopwatch.Stop();
                        //UpdateRunTrace(traceId, Math.Round(stopwatch.Elapsed.TotalSeconds, 3), ScheduleRunResult.Success);
                        //LogHelper.Info($"任务[{job.JobDataMap["name"]}]运行成功！用时{stopwatch.Elapsed.TotalMilliseconds.ToString()}ms", _sid, traceId);
                        //保存运行结果用于子任务触发
                        //context.Result = tctx.Result;
                    }
                    //catch (RunConflictException conflict)
                    //{
                    //    stopwatch.Stop();
                    //    UpdateRunTrace(traceId, Math.Round(stopwatch.Elapsed.TotalSeconds, 3), ScheduleRunResult.Conflict);
                    //    throw conflict;
                    //}
                    catch (Exception e)
                    {
                        stopwatch.Stop();
                        //UpdateRunTrace(traceId, Math.Round(stopwatch.Elapsed.TotalSeconds, 3), ScheduleRunResult.Failed);
                        //LogHelper.Error($"任务\"{job.JobDataMap["name"]}\"运行失败！", e, _sid, traceId);
                        //这里抛出的异常会在JobListener的JobWasExecuted事件中接住
                        //如果吃掉异常会导致程序误以为本次任务执行成功
                        throw;
                    }
                }

            }
            catch (Exception)
            {

                throw;
            }

            return Task.CompletedTask;
        }
    }
}
