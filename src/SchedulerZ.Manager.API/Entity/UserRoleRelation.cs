using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SchedulerZ.Manager.API.Entity
{
    public class UserRoleRelation
    {
        public long UserId { get; set; }
        public long RoleId { get; set; }
        public User User { get; set; }
        public Role Role { get; set; }
    }
}
