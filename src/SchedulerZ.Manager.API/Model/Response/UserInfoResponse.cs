using SchedulerZ.Manager.API.Model.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SchedulerZ.Manager.API.Model.Response
{
    public class UserInfoResponse
    {
        public UserDto User { get; set; }
        public List<RoleDto> Roles { get; set; }
    }
}
