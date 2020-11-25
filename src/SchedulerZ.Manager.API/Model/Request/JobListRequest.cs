using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SchedulerZ.Manager.API.Model.Request
{
    public class JobListRequest
    {
        public string Name { get; set; }

        public int Status { get; set; }

        [Required]
        public int PageIndex { get; set; }

        [Required]
        public int PageSize { get; set; }
    }
}
