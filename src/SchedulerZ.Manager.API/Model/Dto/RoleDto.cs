using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SchedulerZ.Manager.API.Model.Dto
{
    public class RoleDto
    {
        public long Id { get; set; }

        public string Identify { get; set; }

        public string Name { get; set; }

        public string Remark { get; set; }

        public DateTime CreateTime { get; set; }

        public bool IsDelete { get; set; }

        public List<RouterDto> Routers { get; set; }
    }
}
