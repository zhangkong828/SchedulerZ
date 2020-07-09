using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using CSRedis;
using EasyCaching.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SchedulerZ.Manager.API.Model;
using SchedulerZ.Manager.API.Model.Request;
using SchedulerZ.Manager.API.Model.Response;
using SchedulerZ.Manager.API.Utility;

namespace SchedulerZ.Manager.API.Controllers
{
    public class AccountController : BaseApiController
    {
        private readonly ILogger<AccountController> _logger;

        private readonly JWTConfig _jwtConfig;

        private readonly CSRedisClient _redisClient;
        private readonly IEasyCachingProvider _cachingProvider;
        public AccountController(ILogger<AccountController> logger, IOptions<JWTConfig> jwtOptions, IEasyCachingProviderFactory cacheFactory, CSRedisClient redisClient)
        {
            _logger = logger;
            _jwtConfig = jwtOptions.Value;
            _cachingProvider = cacheFactory.GetCachingProvider("default");
            _redisClient = redisClient;
        }

        /// <summary>
        /// 登录
        /// </summary>
        [AllowAnonymous]
        [HttpPost]
        public ActionResult<BaseResponse> Login(LoginRequest request)
        {
            //admin 123456
            if (request.Username != "admin" && request.Password != "21232f297a57a5a743894a0e4a801fc3")
                return BaseResponse.GetBaseResponse(ResponseStatusType.Failed, "用户名或密码错误");

            var userId = "1";
            var tokenCacheKey = CacheKey.Token(userId);

            var token = _redisClient.Get<Token>(tokenCacheKey);
            var expireSeconds = _jwtConfig.RefreshTokenExpiresDays * 24 * 60 * 60;
            if (token == null)
            {
                token = GenerateToken();
                _redisClient.Set(tokenCacheKey, token, expireSeconds);
            }
            else
            {
                var expires = FormatHelper.ConvertToDateTime(token.AccessTokenExpires);
                if (expires <= DateTime.Now)
                {
                    var newToken = GenerateToken();
                    token.AccessToken = newToken.AccessToken;
                    token.AccessTokenExpires = newToken.AccessTokenExpires;
                    _redisClient.Set(tokenCacheKey, token, expireSeconds);
                }
            }
            return BaseResponse<Token>.GetBaseResponse(token);
        }

        [NonAction]
        private Token GenerateToken()
        {
            var claims = new[]
               {
                    new Claim(ClaimTypes.Sid, "1"),
                    new Claim(ClaimTypes.Name, "test"),
                    new Claim(ClaimTypes.Role, "admin")
                };

            var now = DateTime.Now;
            var accessSxpires = now.Add(TimeSpan.FromMinutes(_jwtConfig.AccessTokenExpiresMinutes));
            var token = new JwtSecurityToken(
                issuer: _jwtConfig.Issuer,
                audience: _jwtConfig.Audience,
                claims: claims,
                notBefore: now,
                expires: accessSxpires,
                signingCredentials: new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtConfig.IssuerSigningKey)), SecurityAlgorithms.HmacSha256));

            var accessToken = new JwtSecurityTokenHandler().WriteToken(token);
            var refreshToken = FormatHelper.EncodeBase64(ObjectId.Default().NextId().ToString());
            var refreshTokenExpires = now.Add(TimeSpan.FromDays(_jwtConfig.RefreshTokenExpiresDays));
            return new Token()
            {
                AccessToken = accessToken,
                AccessTokenExpires = accessSxpires.ConvertToUnixOfTime(),
                RefreshToken = refreshToken,
                RefreshTokenExpires = refreshTokenExpires.ConvertToUnixOfTime()
            };
        }

        /// <summary>
        /// 登出
        /// </summary>
        [HttpPost]
        public ActionResult<BaseResponse> Logout()
        {
            var code = _redisClient.Del(CacheKey.Token(GetUserId())) > 0 ? ResponseStatusType.Success : ResponseStatusType.Failed;
            return BaseResponse.GetBaseResponse(code);
        }

        [HttpGet]
        public ActionResult<BaseResponse> Info()
        {
            var user = new User()
            {
                Id = 1,
                Username = "admin",
                Password = "",
                Name = "管理员",
                Avatar = "https://gw.alipayobjects.com/zos/rmsportal/jZUIxmJycoymBprLOUbT.png",
                LastLoginIp = "192.168.1.180",
                LastLoginTime = DateTime.Now,
                Status = 1,
                CreateTime = DateTime.Now,
                IsDelete = false
            };
            var roles = new List<Role>();

            var admin = new Role()
            {
                Id = 1,
                RoleId = "admin",
                RoleName = "管理员",
                CreateTime = DateTime.Now,

                Routers = new List<Router>() {
                    new Router(){
                        Id=1,
                        Name="仪表盘",
                        Permission="dashboard"
                    },
                    new Router(){
                        Id=3,
                        Name="表单页",
                        Permission="form"
                    },
                    new Router(){
                        Id=2,
                        Name="业务布局",
                        Permission="support"
                    }
                }
            };
            roles.Add(admin);
            return BaseResponse<UserInfoResponse>.GetBaseResponse(new UserInfoResponse() { User = user, Roles = roles });
        }
    }
}
