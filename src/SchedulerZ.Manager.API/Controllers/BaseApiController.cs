using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
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
    }
}