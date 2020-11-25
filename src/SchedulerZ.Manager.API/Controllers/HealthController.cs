using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SchedulerZ.Manager.API.Controllers
{
    public class HealthController : BaseApiController
    {
        /// <summary>
        /// 健康检查
        /// </summary>
        [AllowAnonymous]
        [HttpGet]
        public IActionResult Check()
        {
            return Ok();
        }

    }
}
