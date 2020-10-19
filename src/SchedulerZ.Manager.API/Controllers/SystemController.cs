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
using SchedulerZ.Manager.API.Extensions;
using SchedulerZ.Manager.API.Model;
using SchedulerZ.Manager.API.Model.Dto;
using SchedulerZ.Manager.API.Model.Request;

namespace SchedulerZ.Manager.API.Controllers
{
    public class SystemController : BaseApiController
    {
        private readonly IMapper _mapper;
        private readonly SchedulerZContext _context;
        public SystemController(IMapper mapper, SchedulerZContext context)
        {
            _mapper = mapper;
            _context = context;
        }

        /// <summary>
        /// 用户列表
        /// </summary>
        [HttpPost]
        public ActionResult<BaseResponse> QueryUserList(UserListRequest request)
        {
            var list = _context.Users.AsNoTracking().Where(x => !x.IsDelete);

            if (!string.IsNullOrWhiteSpace(request.Name))
                list = list.Where(x => x.Name.Contains(request.Name));

            if (request.Status > 0)
                list = list.Where(x => x.Status == request.Status);

            var total = list.Count();
            var result = list.OrderByDescending(x => x.CreateTime).Skip((request.PageIndex - 1) * request.PageSize).Take(request.PageSize).ToList();

            var pageData = new PageData<UserDto>()
            {
                PageIndex = request.PageIndex,
                PageSize = request.PageSize,
                TotalCount = total,
                List = _mapper.Map<List<UserDto>>(result),
            };
            return BaseResponse<PageData<UserDto>>.GetBaseResponse(pageData);
        }

        /// <summary>
        /// 权限列表
        /// </summary>
        [HttpPost]
        public ActionResult<BaseResponse> QueryPermissionList(PermissionListRequest request)
        {
            var user = _context.Users.AsNoTracking().Include(x => x.UserRoleRelations).ThenInclude(x => x.Role).ThenInclude(x => x.RoleRouterRelations).ThenInclude(x => x.Router).FirstOrDefault(x => x.Id == GetUserId());

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
            }

            long total = 0;
            List<RouterDto> result = new List<RouterDto>();

            if (!string.IsNullOrWhiteSpace(request.Name))
            {
                routerList = routerList.Where(x => x.Name.Contains(request.Name) || x.Title.Contains(request.Name)).ToList();
                total = routerList.Count();
                result = routerList.OrderBy(x => x.Sort).Skip((request.PageIndex - 1) * request.PageSize).Take(request.PageSize).ToList();
            }
            else
            {
                total = routerList.Where(x => x.ParentId == 0).Count();
                result = routerList.Where(x => x.ParentId == 0).OrderBy(x => x.Sort).Skip((request.PageIndex - 1) * request.PageSize).Take(request.PageSize).ToList();

                result.ForEach(item =>
                {
                    item.Children = routerList.Where(x => x.ParentId == item.Id).OrderBy(x => x.Sort).ToList();
                });
            }

            var pageData = new PageData<RouterDto>()
            {
                PageIndex = request.PageIndex,
                PageSize = request.PageSize,
                TotalCount = total,
                List = result,
            };
            return BaseResponse<PageData<RouterDto>>.GetBaseResponse(pageData);
        }

        /// <summary>
        /// 权限列表树
        /// </summary>
        [HttpPost]
        public ActionResult<BaseResponse> QueryPermissionTreeList()
        {
            var user = _context.Users.AsNoTracking().Include(x => x.UserRoleRelations).ThenInclude(x => x.Role).ThenInclude(x => x.RoleRouterRelations).ThenInclude(x => x.Router).FirstOrDefault(x => x.Id == GetUserId());

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
            }

            List<TreeData> list = routerList.OrderBy(x => x.Sort).Select(x => new TreeData() { Title = x.Title, Value = x.Id, Key = x.Id, ParentId = x.ParentId }).ToList();

            TreeData tree = new TreeData()
            {
                Title = "根目录",
                Value = 0,
                Key = 0,
                Children = list.ConvertToTree(0)
            };

            return BaseResponse<List<TreeData>>.GetBaseResponse(new List<TreeData>() { tree });
        }


        /// <summary>
        /// 修改权限
        /// </summary>
        [HttpPost]
        public ActionResult<BaseResponse> ModifyPermission(RouterDto request)
        {
            var entity = _mapper.Map<Router>(request);
            if (request.Id > 0)
            {
                _context.Update(entity);
            }
            else
            {
                _context.Add(entity);
            }
            var result = _context.SaveChanges() > 0;
            return BaseResponse<BaseResponseData>.GetBaseResponse(new BaseResponseData(result));
        }

        /// <summary>
        /// 删除权限
        /// </summary>
        [HttpPost]
        public ActionResult<BaseResponse> DeletePermission(long id)
        {
            var router = _context.Routers.Find(id);
            if (router == null)
            {
                return BaseResponse<BaseResponseData>.GetBaseResponse(new BaseResponseData("不存在"));
            }

            _context.Routers.Remove(router);
            var result = _context.SaveChanges() > 0;
            return BaseResponse<BaseResponseData>.GetBaseResponse(new BaseResponseData(result));
        }
    }
}
