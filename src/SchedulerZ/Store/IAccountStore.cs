using SchedulerZ.Models;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace SchedulerZ.Store
{
    public interface IAccountStore
    {
        List<User> QueryUserListPage<TOrderKey>(int pageIndex, int pageSize, List<Expression<Func<User, bool>>> wheres, Expression<Func<User, TOrderKey>> orderBy, bool isAsc, out int total);
        User QueryUserById(long id);
        User QueryUserByName(string userName);
        User QueryUserInfo(long userId);
        bool AddUser(User user);
        bool UpdateUser(User user);
        bool DeleteUser(long id, bool isReal);

        bool AddUserRoleRelations(List<UserRoleRelation> addEntities);
        bool DeleteUserRoleRelations(long userId, List<long> deleteRoleIds);

        List<Router> QueryRouterList();
        bool UpdateRouter(Router router);
        bool AddRouter(Router router);
        bool DeleteRouter(long id, bool isReal);


        List<Role> QueryRoleListPage<TOrderKey>(int pageIndex, int pageSize, List<Expression<Func<Role, bool>>> wheres, Expression<Func<Role, TOrderKey>> orderBy, bool isAsc, out int total);
        Role QueryRoleById(long id);
        bool UpdateRole(Role role);
        bool AddRole(Role role);
        bool DeleteRole(long id, bool isReal);


        bool AddRoleRouterRelations(List<RoleRouterRelation> addEntities);
        bool DeleteRoleRouterRelations(long roleId, List<long> deleteRouterIds);
    }
}
