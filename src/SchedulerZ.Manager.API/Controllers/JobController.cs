using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using CSRedis;
using EasyCaching.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SchedulerZ.LoadBalancer;
using SchedulerZ.Manager.API.Model;
using SchedulerZ.Manager.API.Model.Request;
using SchedulerZ.Manager.API.Model.Response;
using SchedulerZ.Manager.API.Utility;
using SchedulerZ.Models;
using SchedulerZ.Remoting;
using SchedulerZ.Scheduler;
using SchedulerZ.Store;

namespace SchedulerZ.Manager.API.Controllers
{
    public class JobController : BaseApiController
    {
        private readonly ILogger<JobController> _logger;

        private readonly CSRedisClient _redisClient;
        private readonly IEasyCachingProvider _cachingProvider;
        private readonly IJobStore _jobStore;
        private readonly ISchedulerRemoting _schedulerRemoting;
        private readonly ILoadBalancerFactory _loadBalancerFactory;
        public JobController(ILogger<JobController> logger, CSRedisClient redisClient, IJobStore jobStore, ISchedulerRemoting schedulerRemoting, ILoadBalancerFactory loadBalancerFactory)
        {
            _logger = logger;
            _redisClient = redisClient;

            _jobStore = jobStore;
            _schedulerRemoting = schedulerRemoting;
            _loadBalancerFactory = loadBalancerFactory;
        }

        /// <summary>
        /// 创建Job
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<BaseResponse>> CreateJob(SchedulerJobRequest request)
        {
            if (!ValidateJob(request)) return BaseResponse.GetResponse("Job参数错误");
            var job = Utils.MapperPropertyValue<SchedulerJobRequest, JobEntity>(request);
            var result = _jobStore.AddJob(job);
            if (result)
            {
                if (request.RunNow)
                {
                    var status = await StartJob(job);
                    return BaseResponse.GetResponse(status.Success, $"任务创建成功! 状态：{status.Message}");
                }
                return BaseResponse.GetResponse(true, "任务创建成功");
            }
            return BaseResponse.GetResponse("任务创建失败");
        }

        /// <summary>
        /// 编辑Job
        /// </summary>
        [HttpPost]
        public ActionResult<BaseResponse> EditJob(SchedulerJobRequest request)
        {
            if (!ValidateJob(request)) return BaseResponse.GetResponse("Job参数错误");
            var job = _jobStore.QueryJob(request.Id);
            if (job == null) return BaseResponse.GetResponse("任务不存在");
            if (job.Status != (int)JobStatus.Stop) return BaseResponse.GetResponse("任务在停止状态下才能编辑");

            job.Name = request.Name;
            job.FilePath = request.FilePath;
            job.Remark = request.Remark;
            job.IsSimple = request.IsSimple;
            job.CronExpression = request.CronExpression;
            job.RepeatCount = request.RepeatCount;
            job.IntervalSeconds = request.IntervalSeconds;
            job.AssemblyName = request.AssemblyName;
            job.ClassName = request.ClassName;
            job.CustomParamsJson = request.CustomParamsJson;
            job.StartTime = request.StartTime;
            job.EndTime = request.EndTime;

            var result = _jobStore.UpdateJob(job);
            return BaseResponse.GetResponse(result);
        }

        /// <summary>
        /// 启动任务
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<BaseResponse>> StartJob(string id)
        {
            var job = _jobStore.QueryJob(id);
            if (job == null) return BaseResponse.GetResponse("任务不存在");
            var result = await StartJob(job);
            return BaseResponse.GetResponse(result);
        }

        /// <summary>
        /// 立即运行一次
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<BaseResponse>> RunOnceJob(string id)
        {
            var job = _jobStore.QueryJob(id);
            if (job == null) return BaseResponse.GetResponse("任务不存在");

            if (job.Status == (int)JobStatus.Running)
            {

            }
            return BaseResponse.GetResponse("任务不在运行状态下");
        }

        /// <summary>
        /// 暂停任务
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<BaseResponse>> PauseJob(string id)
        {
            return BaseResponse.GetResponse("");
        }

        /// <summary>
        /// 恢复任务
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<BaseResponse>> ResumeJob(string id)
        {
            return BaseResponse.GetResponse("");
        }

        /// <summary>
        /// 停止任务
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<BaseResponse>> StopJob(string id)
        {
            return BaseResponse.GetResponse("");
        }

        /// <summary>
        /// 删除任务
        /// </summary>
        [HttpPost]
        public ActionResult<BaseResponse> DeleteJob(string id)
        {
            return BaseResponse.GetResponse("");
        }

        [NonAction]
        public bool ValidateJob(SchedulerJobRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Name) || string.IsNullOrWhiteSpace(request.FilePath) || string.IsNullOrWhiteSpace(request.AssemblyName) || string.IsNullOrWhiteSpace(request.ClassName))
            {
                return false;
            }

            if (request.IsSimple)
            {
                if (request.RepeatCount != -1 && request.RepeatCount <= 0)
                    return false;

                if (request.IntervalSeconds < 0)
                    return false;
            }
            else
            {
                if (string.IsNullOrWhiteSpace(request.CronExpression))
                    return false;
            }

            return true;
        }


        [NonAction]
        public async Task<BaseResponseData> StartJob(JobEntity job)
        {
            var result = new BaseResponseData();
            if (job.Status != (int)JobStatus.Stop)
            {
                result.Message = "任务在停止状态下才能启动";
                return result;
            }
            if (job.EndTime.HasValue && job.EndTime < DateTime.Now)
            {
                result.Message = "任务结束时间不能小于当前时间";
                return result;
            }

            var service = await _loadBalancerFactory.Get().Lease("worker");
            if (service == null)
            {
                result.Message = "没有可用的worker";
                return result;
            }

            var status = await _schedulerRemoting.StartJob(job, service);
            //更新状态
            if (status)
            {

            }

            result.Success = status;
            result.Message = result.Success ? "启动成功" : "启动失败";
            return result;
        }
    }
}
