using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SchedulerZ.Manager.API.Entity
{
    public class RoleRouterRelation
    {
        [Key]
        public long Id { get; set; }

        public long RoleId { get; set; }
        public long RouterId { get; set; }
        public Role Role { get; set; }
        public Router Router { get; set; }
    }
}
