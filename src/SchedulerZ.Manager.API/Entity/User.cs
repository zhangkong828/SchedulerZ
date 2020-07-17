using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SchedulerZ.Manager.API.Entity
{
    public class User
    {
        [Key]
        public long Id { get; set; }

        [Required]
        [StringLength(30)]
        public string Username { get; set; }

        [Required]
        [StringLength(50)]
        public string Password { get; set; }

        [Required]
        [StringLength(20)]
        public string Name { get; set; }

        [StringLength(100)]
        public string Avatar { get; set; }

        [StringLength(30)]
        public string LastLoginIp { get; set; }

        public DateTime LastLoginTime { get; set; }

        [Required]
        public int Status { get; set; }

        public DateTime CreateTime { get; set; }

        public bool IsDelete { get; set; }

        public List<UserRoleRelation> UserRoleRelations { get; set; }
    }
}
