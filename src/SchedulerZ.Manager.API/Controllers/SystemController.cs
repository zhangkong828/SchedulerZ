using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Linq.Expressions;
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
using SchedulerZ.Manager.API.Extensions;
using SchedulerZ.Manager.API.Model;
using SchedulerZ.Manager.API.Model.Dto;
using SchedulerZ.Manager.API.Model.Request;
using SchedulerZ.Manager.API.Utility;
using SchedulerZ.Models;
using SchedulerZ.Store;

namespace SchedulerZ.Manager.API.Controllers
{
    public class SystemController : BaseApiController
    {
        private readonly IMapper _mapper;
        private readonly IAccountStore _accountStoreService;
        public SystemController(IMapper mapper, IAccountStore accountStoreService)
        {
            _mapper = mapper;
            _accountStoreService = accountStoreService;
        }

        /// <summary>
        /// 用户列表
        /// </summary>
        [HttpPost]
        public ActionResult<BaseResponse> QueryUserList(UserListRequest request)
        {
            var filters = new List<Expression<Func<User, bool>>>();

            if (!string.IsNullOrWhiteSpace(request.Name))
                filters.Add(x => x.Name.Contains(request.Name));

            if (request.Status > 0)
                filters.Add(x => x.Status == request.Status);

            var result = _accountStoreService.QueryUserListPage(request.PageIndex, request.PageSize, filters, x => x.CreateTime, false, out int total);

            var pageData = new PageData<UserDto>()
            {
                PageIndex = request.PageIndex,
                PageSize = request.PageSize,
                TotalCount = total,
                List = _mapper.Map<List<UserDto>>(result)
            };
            return BaseResponse<PageData<UserDto>>.GetBaseResponse(pageData);
        }

        /// <summary>
        /// 修改用户
        /// </summary>
        [HttpPost]
        public ActionResult<BaseResponse> ModifyUser(UserDto request)
        {
            var entity = _mapper.Map<User>(request);
            bool result = false;
            if (entity.Id > 0)
            {
                var user = _accountStoreService.QueryUserById(request.Id);
                user.Name = entity.Name;
                user.Avatar = entity.Avatar;
                user.Status = entity.Status;

                if (request.RoleIds != null && request.RoleIds.Count > 0)
                {
                    Utils.ListBatchAddOrDelete<long>(user.UserRoleRelations.Select(x => x.RoleId).ToList(), request.RoleIds, out List<long> deleteList, out List<long> addList);

                    if (deleteList.Any())
                    {
                        _accountStoreService.DeleteUserRoleRelations(user.Id, deleteList);
                    }

                    if (addList.Any())
                    {
                        var addEntities = new List<UserRoleRelation>();
                        addList.ForEach(id =>
                        {
                            addEntities.Add(new UserRoleRelation() { UserId = user.Id, RoleId = id });
                        });
                        _accountStoreService.AddUserRoleRelations(addEntities);
                    }
                }

                result = _accountStoreService.UpdateUser(user);
            }
            return BaseResponse<BaseResponseData>.GetBaseResponse(new BaseResponseData(result));
        }

        /// <summary>
        /// 删除用户
        /// </summary>
        [HttpPost]
        public ActionResult<BaseResponse> DeleteUser(long id)
        {
            var result = _accountStoreService.DeleteUser(id, false);
            return BaseResponse<BaseResponseData>.GetBaseResponse(new BaseResponseData(result));
        }

        /// <summary>
        /// 权限列表
        /// </summary>
        [HttpPost]
        public ActionResult<BaseResponse> QueryPermissionList(PermissionListRequest request)
        {
            var routerList = _mapper.Map<List<RouterDto>>(_accountStoreService.QueryRouterList());

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
                    item.Children = routerList.ConvertToTree<long, RouterDto>(item.Id);
                });
            }

            var pageData = new PageData<RouterDto>()
            {
                PageIndex = request.PageIndex,
                PageSize = request.PageSize,
                TotalCount = total,
                List = result
            };
            return BaseResponse<PageData<RouterDto>>.GetBaseResponse(pageData);
        }

        /// <summary>
        /// 权限列表树
        /// </summary>
        [HttpPost]
        public ActionResult<BaseResponse> QueryPermissionTreeList()
        {
            var routerList = _mapper.Map<List<RouterDto>>(_accountStoreService.QueryRouterList());
            List<TreeData> list = routerList.OrderBy(x => x.Sort).Select(x => new TreeData() { Title = x.Title, Value = x.Id, Key = x.Id, ParentId = x.ParentId }).ToList();

            TreeData tree = new TreeData()
            {
                Title = "根目录",
                Value = 0,
                Key = 0,
                Children = list.ConvertToTree<long, TreeData>(0)
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
            bool result;
            if (request.Id > 0)
            {
                result = _accountStoreService.UpdateRouter(entity);
            }
            else
            {
                entity.CreateTime = DateTime.Now;
                result = _accountStoreService.AddRouter(entity);
            }
            return BaseResponse<BaseResponseData>.GetBaseResponse(new BaseResponseData(result));
        }

        /// <summary>
        /// 删除权限
        /// </summary>
        [HttpPost]
        public ActionResult<BaseResponse> DeletePermission(long id)
        {
            var result = _accountStoreService.DeleteRouter(id, false);
            return BaseResponse<BaseResponseData>.GetBaseResponse(new BaseResponseData(result));
        }


        /// <summary>
        /// 角色列表
        /// </summary>
        [HttpPost]
        public ActionResult<BaseResponse> QueryRoleList(RoleListRequest request)
        {
            var filters = new List<Expression<Func<Role, bool>>>();

            if (!string.IsNullOrWhiteSpace(request.Name))
                filters.Add(x => x.Name.Contains(request.Name) || x.Identify.Contains(request.Name));

            var result = _accountStoreService.QueryRoleListPage(request.PageIndex, request.PageSize, filters, x => x.CreateTime, false, out int total);

            var list = new List<RoleDto>();
            foreach (var role in result)
            {
                var roleDto = _mapper.Map<RoleDto>(role);
                roleDto.Routers = role.RoleRouterRelations.Select(x => _mapper.Map<RouterDto>(x.Router)).Where(x => x.IsDelete == false).OrderBy(x => x.Sort).ToList();
                list.Add(roleDto);
            }
            var pageData = new PageData<RoleDto>()
            {
                PageIndex = request.PageIndex,
                PageSize = request.PageSize,
                TotalCount = total,
                List = list
            };
            return BaseResponse<PageData<RoleDto>>.GetBaseResponse(pageData);
        }

        /// <summary>
        /// 修改角色
        /// </summary>
        [HttpPost]
        public ActionResult<BaseResponse> ModifyRole(RoleDto request)
        {
            var entity = _mapper.Map<Role>(request);
            bool result;
            if (request.Id > 0)
            {
                var old = _accountStoreService.QueryRoleById(request.Id);
                if (request.RouterIds != null && request.RouterIds.Count > 0)
                {
                    Utils.ListBatchAddOrDelete<long>(old.RoleRouterRelations.Select(x => x.RouterId).ToList(), request.RouterIds, out List<long> deleteList, out List<long> addList);

                    if (deleteList.Any())
                    {
                        _accountStoreService.DeleteRoleRouterRelations(old.Id, deleteList);
                    }

                    if (addList.Any())
                    {
                        var addEntities = new List<RoleRouterRelation>();
                        addList.ForEach(id =>
                        {
                            addEntities.Add(new RoleRouterRelation() { RoleId = old.Id, RouterId = id });
                        });
                        _accountStoreService.AddRoleRouterRelations(addEntities);
                    }
                }
                result = _accountStoreService.UpdateRole(entity);
            }
            else
            {
                entity.CreateTime = DateTime.Now;

                var roleRouterRelations = new List<RoleRouterRelation>();
                request.RouterIds.ForEach(id =>
                {
                    roleRouterRelations.Add(new RoleRouterRelation() { RouterId = id });
                });
                entity.RoleRouterRelations = roleRouterRelations;

                result = _accountStoreService.AddRole(entity);

            }
            return BaseResponse<BaseResponseData>.GetBaseResponse(new BaseResponseData(result));
        }

        /// <summary>
        /// 删除角色
        /// </summary>
        [HttpPost]
        public ActionResult<BaseResponse> DeleteRole(long id)
        {
            var result = _accountStoreService.DeleteRole(id, false);
            return BaseResponse<BaseResponseData>.GetBaseResponse(new BaseResponseData(result));
        }
    }
}
