using SchedulerZ.Manager.API.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SchedulerZ.Manager.API.Data
{
    public static class DbInitializer
    {
        public static void Initialize(SchedulerZContext context)
        {
            context.Database.EnsureCreated();

            // Look for any students.
            if (context.Users.Any())
            {
                return;   // DB has been seeded
            }

            var user = new User()
            {
                Username = "admin",
                Password = "e10adc3949ba59abbe56e057f20f883e",
                Name = "Admin",
                Avatar = "https://gw.alipayobjects.com/zos/rmsportal/jZUIxmJycoymBprLOUbT.png",
                LastLoginIp = "192.168.1.180",
                LastLoginTime = DateTime.Now,
                Status = 1,
                CreateTime = DateTime.Now,
                IsDelete = false
            };
            context.Users.Add(user);
            context.SaveChanges();

            var role = new Role()
            {
                Identify = "admin",
                Name = "管理员",
                CreateTime = DateTime.Now,
            };
            context.Roles.Add(role);
            context.SaveChanges();

            var routers = new Router[] {
                new Router(){
                    Title="仪表盘",
                    Path="/dashboard",
                    Name="仪表盘",
                    Component="RouteView",
                    Permission="dashboard",
                    Icon="dashboard",
                    Show=true,
                    Redirect="/dashboard/workplace",
                    ParentId=0,
                    CreateTime=DateTime.Now
                },
                new Router(){
                    Title="业务布局11",
                    Path="/system",
                    Name="业务布局11",
                    Component="other/IconSelectorView",
                    Permission="dashboard",
                    Icon="slack",
                    Show=true,
                    Redirect="/other/icon-selector",
                    ParentId=0,
                    CreateTime=DateTime.Now
                }
            };
            context.Routers.AddRange(routers);
            context.SaveChanges();

            var userRoleRelation = new UserRoleRelation()
            {
                UserId = user.Id,
                RoleId = role.Id
            };
            context.UserRoleRelations.Add(userRoleRelation);

            var roleRouterRelations = new List<RoleRouterRelation>();
            foreach (var item in routers)
            {
                roleRouterRelations.Add(new RoleRouterRelation()
                {
                    RoleId = role.Id,
                    RouterId = item.Id
                });
            }
            context.RoleRouterRelations.AddRange(roleRouterRelations);
            context.SaveChanges();
        }
    }
}
