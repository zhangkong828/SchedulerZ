using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SchedulerZ.Manager.API.Model
{
    public class Router
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Permission { get; set; }
        public string Icon { get; set; }
        public int Sort { get; set; }
        public string Remark { get; set; }
        public long ParentId { get; set; }
        public DateTime CreateTime { get; set; }
        public bool IsDelete { get; set; }
    }
}
