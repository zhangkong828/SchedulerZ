using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SchedulerZ.Manager.API.Model
{
    public class Role
    {
        public long Id { get; set; }
        public string RoleId { get; set; }
        public string RoleName { get; set; }
        public string Remark { get; set; }
        public DateTime CreateTime { get; set; }
        public bool IsDelete { get; set; }
    }
}
