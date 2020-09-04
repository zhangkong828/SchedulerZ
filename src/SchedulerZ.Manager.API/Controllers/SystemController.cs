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
        public ActionResult<BaseResponse> GetUserList(UserListRequest request)
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

    }
}
