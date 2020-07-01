using CSRedis;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using SchedulerZ.Manager.API.Model;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SchedulerZ.Manager.API.Filter
{
    public static class TokenValidatedFilter
    {
        public static Task OnTokenValidated(TokenValidatedContext context)
        {
            var userId = context.Principal.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Sid)?.Value;
            if (!string.IsNullOrWhiteSpace(userId))
            {
                var redisClient = context.HttpContext.RequestServices.GetService(typeof(CSRedisClient)) as CSRedisClient;
                if (redisClient != null)
                {
                    var key = CacheKey.Token(userId);
                    var token = redisClient.Get<Token>(key);
                    if (token != null)
                    {
                        var accessToken = (context.SecurityToken as JwtSecurityToken).RawData;
                        if (!string.IsNullOrWhiteSpace(accessToken))
                        {
                            if (token.AccessToken != accessToken)
                                context.ValidateFail("access token invalid");
                        }
                        else
                            context.ValidateFail("access token exception");
                    }
                    else
                        context.ValidateFail("token invalid");
                }
                else
                    context.ValidateFail("internal exception");
            }
            else
                context.ValidateFail("uid invalid");

            return Task.FromResult(0);
        }

        static void ValidateFail(this TokenValidatedContext context, string msg)
        {
            context.Response.Headers.Add("act", msg);
            context.Fail(msg);
        }

    }
}
