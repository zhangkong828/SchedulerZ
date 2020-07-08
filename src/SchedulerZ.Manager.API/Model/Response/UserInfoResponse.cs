using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SchedulerZ.Manager.API.Model.Response
{
    public class UserInfoResponse
    {
        public User User { get; set; }
        public List<Role> Roles { get; set; }
    }
}
