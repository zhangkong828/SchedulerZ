using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SchedulerZ.Manager.API.Entity
{
    public class Router
    {
        [Key]
        public long Id { get; set; }

        [Required]
        [StringLength(30)]
        public string Name { get; set; }

        [Required]
        [StringLength(30)]
        public string Permission { get; set; }

        [StringLength(30)]
        public string Icon { get; set; }

        public int Sort { get; set; }

        [StringLength(100)]
        public string Remark { get; set; }

        [Required]
        public long ParentId { get; set; }

        public DateTime CreateTime { get; set; }

        public bool IsDelete { get; set; }

        public List<RoleRouterRelation> RoleRouterRelations { get; set; }
    }
}
