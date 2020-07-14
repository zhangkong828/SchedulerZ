using Microsoft.EntityFrameworkCore;
using SchedulerZ.Manager.API.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SchedulerZ.Manager.API.Data
{
    public class SchedulerZContext : DbContext
    {
        public SchedulerZContext(DbContextOptions<SchedulerZContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Router> Routers { get; set; }
        public DbSet<UserRoleRelation> UserRoleRelations { get; set; }
        public DbSet<RoleRouterRelation> RoleRouterRelations { get; set; }
    }
}
