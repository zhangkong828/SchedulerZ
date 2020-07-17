using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SchedulerZ.Manager.API.Entity
{
    public class Role
    {
        [Key]
        public long Id { get; set; }

        [Required]
        [StringLength(30)]
        public string Identify { get; set; }

        [Required]
        [StringLength(30)]
        public string Name { get; set; }

        [StringLength(100)]
        public string Remark { get; set; }

        public DateTime CreateTime { get; set; }

        public bool IsDelete { get; set; }

        public List<UserRoleRelation> UserRoleRelations { get; set; }

        public List<RoleRouterRelation> RoleRouterRelations { get; set; }
    }
}
