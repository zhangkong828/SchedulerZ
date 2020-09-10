using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SchedulerZ.Manager.API.Model.Request
{
    public class RefreshTokenRequest
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
    }
}
