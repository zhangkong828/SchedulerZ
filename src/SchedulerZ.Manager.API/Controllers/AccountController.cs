using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using CSRedis;
using EasyCaching.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SchedulerZ.Manager.API.Data;
using SchedulerZ.Manager.API.Entity;
using SchedulerZ.Manager.API.Model;
using SchedulerZ.Manager.API.Model.Dto;
using SchedulerZ.Manager.API.Model.Request;
using SchedulerZ.Manager.API.Model.Response;
using SchedulerZ.Manager.API.Utility;

namespace SchedulerZ.Manager.API.Controllers
{
    public class AccountController : BaseApiController
    {
        private readonly IMapper _mapper;
        private readonly SchedulerZContext _context;
        private readonly ILogger<AccountController> _logger;

        private readonly JWTConfig _jwtConfig;

        private readonly CSRedisClient _redisClient;
        private readonly IEasyCachingProvider _cachingProvider;
        public AccountController(IMapper mapper, SchedulerZContext context, ILogger<AccountController> logger, IOptions<JWTConfig> jwtOptions, IEasyCachingProviderFactory cacheFactory, CSRedisClient redisClient)
        {
            _mapper = mapper;
            _context = context;
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
            var user = _context.Users.SingleOrDefault(x => x.Username == request.Username);
            if (user == null)
                return BaseResponse.GetBaseResponse(ResponseStatusType.Failed, "用户名错误");

            if (user.Password != request.Password)
                return BaseResponse.GetBaseResponse(ResponseStatusType.Failed, "密码错误");

            var tokenCacheKey = CacheKey.Token(user.Id.ToString());

            var token = _redisClient.Get<Token>(tokenCacheKey);
            var expireSeconds = _jwtConfig.RefreshTokenExpiresDays * 24 * 60 * 60;
            if (token == null)
            {
                token = GenerateToken(user);
                _redisClient.Set(tokenCacheKey, token, expireSeconds);
            }
            else
            {
                var expires = FormatHelper.ConvertToDateTime(token.AccessTokenExpires);
                if (expires <= DateTime.Now)
                {
                    var newToken = GenerateToken(user);
                    token.AccessToken = newToken.AccessToken;
                    token.AccessTokenExpires = newToken.AccessTokenExpires;
                    _redisClient.Set(tokenCacheKey, token, expireSeconds);
                }
            }
            UpdateLastLoginInfo(user);
            return BaseResponse<Token>.GetBaseResponse(token);
        }

        [NonAction]
        private bool UpdateLastLoginInfo(User user)
        {
            user.LastLoginIp = HttpContext.GetIpAddress();
            user.LastLoginTime = DateTime.Now;
            _context.Users.Update(user);
            return _context.SaveChanges() > 0;
        }

        [NonAction]
        private Token GenerateToken(User user)
        {
            var claims = new[]
               {
                    new Claim(ClaimTypes.Sid, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.Name)
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
            var code = _redisClient.Del(CacheKey.Token(GetUserId().ToString())) > 0 ? ResponseStatusType.Success : ResponseStatusType.Failed;
            return BaseResponse.GetBaseResponse(code);
        }

        /// <summary>
        /// 获取用户信息
        /// </summary>
        [HttpGet]
        public ActionResult<BaseResponse> Info()
        {
            var user = _context.Users.AsNoTracking().Include(x => x.UserRoleRelations).ThenInclude(x => x.Role).ThenInclude(x => x.RoleRouterRelations).ThenInclude(x => x.Router).FirstOrDefault(x => x.Id == GetUserId());

            var userDto = _mapper.Map<UserDto>(user);

            var roles = new List<RoleDto>();
            foreach (var role in user.UserRoleRelations)
            {
                var roleDto = _mapper.Map<RoleDto>(role.Role);
                roleDto.Routers = role.Role.RoleRouterRelations.Select(x => _mapper.Map<RouterDto>(x.Router)).ToList();
                roles.Add(roleDto);
            }

            return BaseResponse<UserInfoResponse>.GetBaseResponse(new UserInfoResponse() { User = userDto, Roles = roles });
        }

        /// <summary>
        /// 获取用户菜单
        /// </summary>
        [HttpGet]
        public ActionResult<BaseResponse> Nav()
        {
            var user = _context.Users.AsNoTracking().Include(x => x.UserRoleRelations).ThenInclude(x => x.Role).ThenInclude(x => x.RoleRouterRelations).ThenInclude(x => x.Router).FirstOrDefault(x => x.Id == GetUserId());

            var userDto = _mapper.Map<UserDto>(user);

            var roles = new List<RoleDto>();
            foreach (var role in user.UserRoleRelations)
            {
                var roleDto = _mapper.Map<RoleDto>(role.Role);
                roleDto.Routers = role.Role.RoleRouterRelations.Select(x => _mapper.Map<RouterDto>(x.Router)).ToList();
                roles.Add(roleDto);
            }

            return BaseResponse<UserInfoResponse>.GetBaseResponse(new UserInfoResponse() { User = userDto, Roles = roles });
        }
    }
}
