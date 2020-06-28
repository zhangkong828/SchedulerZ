using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using EasyCaching.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SchedulerZ.Manager.API.Filter;

namespace SchedulerZ.Manager.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    [TypeFilter(typeof(GlobalExceptionFilter))]
    [TypeFilter(typeof(GlobalActionFilter))]
    public class BaseApiController : ControllerBase
    {

        [NonAction]
        protected string GetUserId()
        {
            return User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Sid)?.Value;
        }

        [NonAction]
        protected string GetUserName()
        {
            return User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Name)?.Value;
        }

        [NonAction]
        protected string GetUserRole()
        {
            return User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Role)?.Value;
        }

    }
}