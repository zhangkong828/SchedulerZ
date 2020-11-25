using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SchedulerZ.Caching;
using SchedulerZ.Manager.API.Model;
using SchedulerZ.Manager.API.Model.Dto;
using SchedulerZ.Manager.API.Model.Request;
using SchedulerZ.Manager.API.Model.Response;
using SchedulerZ.Manager.API.Utility;
using SchedulerZ.Models;
using SchedulerZ.Store;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace SchedulerZ.Manager.API.Controllers
{
    public class AccountController : BaseApiController
    {
        private readonly IMapper _mapper;
        private readonly IAccountStore _accountStoreService;
        private readonly ICaching _caching;
        private readonly ILogger<AccountController> _logger;

        private readonly JWTConfig _jwtConfig;

        public AccountController(IMapper mapper, IAccountStore accountStoreService, ICachingProvider cachingProvider, ILogger<AccountController> logger, IOptions<JWTConfig> jwtOptions)
        {
            _mapper = mapper;
            _accountStoreService = accountStoreService;
            _caching = cachingProvider.CreateCaching();

            _logger = logger;
            _jwtConfig = jwtOptions.Value;
        }

        /// <summary>
        /// 登录
        /// </summary>
        [AllowAnonymous]
        [HttpPost]
        public ActionResult<BaseResponse> Login(LoginRequest request)
        {
            //admin 123456
            var user = _accountStoreService.QueryUserByName(request.Username);
            if (user == null)
                return BaseResponse.GetBaseResponse(ResponseStatusType.Failed, "用户名错误");

            if (user.Password != request.Password)
                return BaseResponse.GetBaseResponse(ResponseStatusType.Failed, "密码错误");

            var tokenCacheKey = CacheKey.Token(user.Id.ToString());

            var token = _caching.Get<Token>(tokenCacheKey);
            if (token == null)
            {
                //新登录用户 创建新Token
                token = GenerateToken(user);
                _caching.Set(tokenCacheKey, token, TimeSpan.FromDays(_jwtConfig.RefreshTokenExpiresDays));
            }
            else
            {
                //老用户
                var expires = FormatHelper.ConvertToDateTime(token.AccessTokenExpires);
                if (expires <= DateTime.Now)
                {
                    //AccessTokeng过期 重新生成
                    var newToken = GenerateToken(user);
                    //只更新AccessToken，老的RefreshToken保持不变
                    token.AccessToken = newToken.AccessToken;
                    token.AccessTokenExpires = newToken.AccessTokenExpires;

                    var refreshTokenExpires = FormatHelper.ConvertToDateTime(token.RefreshTokenExpires);
                    var expireTimeSpan = refreshTokenExpires - DateTime.Now;
                    _caching.Set(tokenCacheKey, token, expireTimeSpan);
                }
            }
            UpdateLastLoginInfo(user);
            return BaseResponse<Token>.GetBaseResponse(token);
        }

        [NonAction]
        private bool UpdateLastLoginInfo(User user)
        {
            user.LastLoginTime = DateTime.Now;
            user.LastLoginIp = HttpContext.GetIpAddress();
            return _accountStoreService.UpdateUser(user);
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
        /// 刷新Token
        /// </summary>
        [AllowAnonymous]
        [HttpPost]
        public ActionResult<BaseResponse> RefreshToken(RefreshTokenRequest request)
        {
            var jwtHandler = new JwtSecurityTokenHandler();
            if (jwtHandler.CanReadToken(request.AccessToken))
            {
                var jwt = new JwtSecurityTokenHandler().ReadJwtToken(request.AccessToken);
                var uId = jwt.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Sid)?.Value;
                if (!string.IsNullOrWhiteSpace(uId) && long.TryParse(uId, out long userId))
                {
                    var tokenCacheKey = CacheKey.Token(userId.ToString());
                    var token = _caching.Get<Token>(tokenCacheKey);
                    if (token != null && token.RefreshToken == request.RefreshToken)
                    {
                        var user = _accountStoreService.QueryUserById(userId);
                        if (user != null)
                        {
                            var newToken = GenerateToken(user);
                            token.AccessToken = newToken.AccessToken;
                            token.AccessTokenExpires = newToken.AccessTokenExpires;

                            var refreshTokenExpires = FormatHelper.ConvertToDateTime(token.RefreshTokenExpires);
                            var expireTimeSpan = refreshTokenExpires - DateTime.Now;
                            _caching.Set(tokenCacheKey, token, expireTimeSpan);

                            return BaseResponse<Token>.GetBaseResponse(token);
                        }
                    }
                }
            }
            return BaseResponse.GetBaseResponse(ResponseStatusType.Unauthorized, "刷新失败");
        }

        /// <summary>
        /// 登出
        /// </summary>
        [AllowAnonymous]
        [HttpPost]
        public ActionResult<BaseResponse> Logout()
        {
            _caching.Remove(CacheKey.Token(GetUserId().ToString()));
            return BaseResponse.GetBaseResponse(ResponseStatusType.Success);
        }

        /// <summary>
        /// 获取用户信息
        /// </summary>
        [HttpGet]
        public ActionResult<BaseResponse> Info()
        {
            var user = _accountStoreService.QueryUserInfo(GetUserId());
            var userDto = _mapper.Map<UserDto>(user);

            var roles = user.UserRoleRelations.Select(x => _mapper.Map<RoleDto>(x.Role)).ToList();

            return BaseResponse<UserInfoResponse>.GetBaseResponse(new UserInfoResponse() { User = userDto, Roles = roles });
        }

        /// <summary>
        /// 获取用户菜单
        /// </summary>
        [HttpGet]
        public ActionResult<BaseResponse> GetUserNav()
        {
            var user = _accountStoreService.QueryUserInfo(GetUserId());

            var roles = new List<RoleDto>();
            foreach (var role in user.UserRoleRelations)
            {
                var roleDto = _mapper.Map<RoleDto>(role.Role);
                roleDto.Routers = role.Role.RoleRouterRelations.Select(x => _mapper.Map<RouterDto>(x.Router)).Where(x => x.IsDelete == false).ToList();
                roles.Add(roleDto);
            }

            List<RouterDto> routerList = new List<RouterDto>();
            if (roles.Count > 0)
            {
                routerList = roles[0].Routers;

                //所有角色对应路由的并集
                if (roles.Count > 1)
                {
                    for (int i = 1; i < roles.Count; i++)
                    {
                        routerList = routerList.Union(roles[i].Routers).ToList();
                    }
                }
                routerList = routerList.OrderBy(x => x.Sort).ToList();
            }
            return BaseResponse<List<RouterDto>>.GetBaseResponse(routerList);
        }


    }
}
