using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SchedulerZ.Models
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
