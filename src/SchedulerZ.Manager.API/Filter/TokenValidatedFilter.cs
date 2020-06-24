using Microsoft.AspNetCore.Authentication.JwtBearer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SchedulerZ.Manager.API.Filter
{
    public class TokenValidatedFilter
    {
        public static Task OnTokenValidated(TokenValidatedContext context)
        {
            var uId = context.Principal.Claims.FirstOrDefault(x => x.Type == "userId")?.Value;
            if (!string.IsNullOrWhiteSpace(uId))
            {
                //var redisClient = context.HttpContext.RequestServices.GetService(typeof(CSRedisClient)) as CSRedisClient;
                //var key = CacheKey.AccessTokenKey(uId);// $"AccessToken:{uId}:App";
                //if (!redisClient.Exists(key))
                //    context.Fail("logout");
            }
            else
                context.Fail("uid Invalid");
            return Task.FromResult(0);
        }
    }
}
