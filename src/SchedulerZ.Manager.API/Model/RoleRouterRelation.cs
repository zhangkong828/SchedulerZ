using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SchedulerZ.Manager.API.Model
{
    public class RoleRouterRelation
    {
        public long Id { get; set; }
        public long RoleId { get; set; }
        public long RouterId { get; set; }
    }
}
