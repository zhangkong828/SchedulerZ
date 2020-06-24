using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SchedulerZ.Manager.API.Model;
using SchedulerZ.Manager.API.Model.Request;
using SchedulerZ.Manager.API.Utility;

namespace SchedulerZ.Manager.API.Controllers
{
    public class AccountController : BaseApiController
    {
        private readonly ILogger<AccountController> _logger;

        private readonly JWTConfig _jwtConfig;
        public AccountController(ILogger<AccountController> logger, IOptions<JWTConfig> jwtOptions)
        {
            _logger = logger;
            _jwtConfig = jwtOptions.Value;
        }

        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet]
        public ActionResult<BaseResponse> Login(LoginRequest request)
        {
            if (request.Username != "admin" && request.Password != "admin")
                return BaseResponse<Token>.GetBaseResponse(ResponseStatusType.Failed, "用户名或密码错误");

            var token = GenerateToken();
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
    }
}
