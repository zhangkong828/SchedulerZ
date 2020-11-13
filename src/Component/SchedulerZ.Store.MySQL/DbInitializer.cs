using SchedulerZ.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SchedulerZ.Store.MySQL
{
    public class DbInitializer
    {
        public static void Initialize(SchedulerZContext context)
        {
            context.Database.EnsureCreated();

            if (context.Users.Any())
            {
                return;
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
                    Name="Dashboard",
                    Component="RouteView",
                    Permission="dashboard",
                    Icon="dashboard",
                    Show=true,
                    Redirect="/dashboard/workplace",
                    ParentId=0,
                    Sort=1000,
                    CreateTime=DateTime.Now
                },
                 new Router(){
                    Title="工作台",
                    Path="/dashboard/workplace",
                    Name="Workplace",
                    Component="Workplace",
                    Permission="dashboard",
                    Icon="",
                    Show=true,
                    Redirect="",
                    ParentId=1,
                    Sort=1001,
                    CreateTime=DateTime.Now
                },
                new Router(){
                    Title="系统管理",
                    Path="/system",
                    Name="System",
                    Component="RouteView",
                    Permission="system",
                    Icon="slack",
                    Show=true,
                    Redirect="/system/user",
                    ParentId=0,
                    Sort=2000,
                    CreateTime=DateTime.Now
                },
                new Router(){
                    Title="用户管理",
                    Path="/system/user",
                    Name="User",
                    Component="User",
                    Permission="system",
                    Icon="",
                    Show=true,
                    Redirect="",
                    ParentId=3,
                    Sort=2001,
                    CreateTime=DateTime.Now
                },
                new Router(){
                    Title="角色管理",
                    Path="/system/role",
                    Name="Role",
                    Component="Role",
                    Permission="system",
                    Icon="",
                    Show=true,
                    Redirect="",
                    ParentId=3,
                    Sort=2002,
                    CreateTime=DateTime.Now
                },
                new Router(){
                    Title="权限管理",
                    Path="/system/permission",
                    Name="Permission",
                    Component="Permission",
                    Permission="system",
                    Icon="",
                    Show=true,
                    Redirect="",
                    ParentId=3,
                    Sort=2003,
                    CreateTime=DateTime.Now
                },
                new Router(){
                    Title="节点管理",
                    Path="/node",
                    Name="Node",
                    Component="RouteView",
                    Permission="",
                    Icon="global",
                    Show=true,
                    Redirect="",
                    ParentId=0,
                    Sort=3000,
                    CreateTime=DateTime.Now
                },
                new Router(){
                    Title="节点列表",
                    Path="/node/list",
                    Name="NodeList",
                    Component="NodeList",
                    Permission="",
                    Icon="",
                    Show=true,
                    Redirect="",
                    ParentId=7,
                    Sort=3001,
                    CreateTime=DateTime.Now
                },
                new Router(){
                    Title="服务列表",
                    Path="/node/services",
                    Name="Services",
                    Component="Services",
                    Permission="",
                    Icon="",
                    Show=true,
                    Redirect="",
                    ParentId=7,
                    Sort=3002,
                    CreateTime=DateTime.Now
                },
                new Router(){
                    Title="任务列表",
                    Path="/jobs",
                    Name="Jobs",
                    Component="Jobs",
                    Permission="",
                    Icon="schedule",
                    Show=true,
                    Redirect="",
                    ParentId=0,
                    Sort=4000,
                    CreateTime=DateTime.Now
                },
                new Router(){
                    Title="系统日志",
                    Path="/logs",
                    Name="Logs",
                    Component="Logs",
                    Permission="",
                    Icon="file-search",
                    Show=true,
                    Redirect="",
                    ParentId=0,
                    Sort=5000,
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
