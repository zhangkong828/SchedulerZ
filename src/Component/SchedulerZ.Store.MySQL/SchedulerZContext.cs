using Microsoft.EntityFrameworkCore;
using SchedulerZ.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SchedulerZ.Store.MySQL
{
    public class SchedulerZContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Router> Routers { get; set; }
        public DbSet<UserRoleRelation> UserRoleRelations { get; set; }
        public DbSet<RoleRouterRelation> RoleRouterRelations { get; set; }


    }
}
