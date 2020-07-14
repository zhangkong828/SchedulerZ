using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SchedulerZ.Manager.API.Entity
{
    public class RoleRouterRelation
    {
        public long RoleId { get; set; }
        public long RouterId { get; set; }
        public Role Role { get; set; }
        public Router Router { get; set; }
    }
}
