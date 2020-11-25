using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using CSRedis;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SchedulerZ.Manager.API.Model;
using SchedulerZ.Manager.API.Model.Request;
using SchedulerZ.Manager.API.Model.Response;
using SchedulerZ.Manager.API.Utility;
using SchedulerZ.Route;

namespace SchedulerZ.Manager.API.Controllers
{
    public class RouteController : BaseApiController
    {
        private readonly ILogger<RouteController> _logger;
        
        private readonly IServiceRoute _serviceRoute;
        public RouteController(ILogger<RouteController> logger, IHostEnvironment hostEnvironment, IServiceRoute serviceRoute)
        {
            _logger = logger;

            _serviceRoute = serviceRoute;
        }

        [HttpPost]
        public async Task<ActionResult<BaseResponse>> NodeList(NodeListRequest request)
        {
            var nodes = await _serviceRoute.QueryNodes();

            if (!string.IsNullOrWhiteSpace(request.Name))
            {
                nodes = nodes.Where(x => x.Name.Contains(request.Name));
            }

            var total = nodes.Count();
            var result = nodes.ToList().Skip((request.PageIndex - 1) * request.PageSize).Take(request.PageSize).ToList();
            var pageData = new PageData<NodeDescriptor>()
            {
                PageIndex = request.PageIndex,
                PageSize = request.PageSize,
                TotalCount = total,
                List = result
            };
            return BaseResponse<PageData<NodeDescriptor>>.GetBaseResponse(pageData);
        }

        [HttpPost]
        public async Task<ActionResult<BaseResponse>> ServiceList(ServiceListRequest request)
        {
            var services = await _serviceRoute.QueryServices();

            if (!string.IsNullOrWhiteSpace(request.Name))
            {
                services = services.Where(x => x.Name.Contains(request.Name));
            }

            var total = services.Count();
            var result = services.ToList().Skip((request.PageIndex - 1) * request.PageSize).Take(request.PageSize).ToList();
            var pageData = new PageData<ServiceRouteDescriptor>()
            {
                PageIndex = request.PageIndex,
                PageSize = request.PageSize,
                TotalCount = total,
                List = result
            };
            return BaseResponse<PageData<ServiceRouteDescriptor>>.GetBaseResponse(pageData);
        }

    }
}
