using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchedulerZ.Manager.API.Filter;

namespace SchedulerZ.Manager.API.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    [TypeFilter(typeof(GlobalExceptionFilter))]
    [TypeFilter(typeof(GlobalActionFilter))]
    public class BaseApiController : ControllerBase
    {

        [NonAction]
        protected long GetUserId()
        {
            var uId = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Sid)?.Value;
            long.TryParse(uId, out long userId);
            return userId;
        }

        [NonAction]
        protected string GetUserName()
        {
            return User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Name)?.Value;
        }

    }
}