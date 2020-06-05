using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SchedulerZ.Logging;
using SchedulerZ.Manager.API.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SchedulerZ.Manager.API.Filter
{
    public class GlobalExceptionFilter : IExceptionFilter
    {
        private readonly ILogger _log;
        public GlobalExceptionFilter(ILoggerProvider loggerProvider)
        {
            _log = loggerProvider.CreateLogger("Exception");
        }

        public void OnException(ExceptionContext context)
        {
            var controller = context.RouteData.Values["controller"];
            var action = context.RouteData.Values["action"];
            var path = context.HttpContext.Request.Path;
            var queryString = context.HttpContext.Request.QueryString.Value;
            _log.Error($"[url]:{path + queryString}\r\n[controller]:{controller}\r\n[action]:{action}", context.Exception);

            context.ExceptionHandled = true;
            context.Result = new JsonResult(BaseResponse.GetBaseResponse(ResponseStatusType.ServerException));
        }
    }
}
