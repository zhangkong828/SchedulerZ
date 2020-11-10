using Microsoft.EntityFrameworkCore;
using SchedulerZ.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace SchedulerZ.Store.MySQL.Impl
{
    public class AccountStoreService : IAccountStore
    {
        private readonly SchedulerZContext _context;
        public AccountStoreService(SchedulerZContext context)
        {
            _context = context;
        }

        public User QueryUserById(long id)
        {
            return _context.Users.AsNoTracking().Include(x => x.UserRoleRelations).ThenInclude(x => x.Role).SingleOrDefault(x => x.Id == id);
        }

        public User QueryUserByName(string userName)
        {
            return _context.Users.AsNoTracking().Include(x => x.UserRoleRelations).ThenInclude(x => x.Role).SingleOrDefault(x => x.Username == userName);
        }

        public User QueryUserInfo(long userId)
        {
            return _context.Users.AsNoTracking().Include(x => x.UserRoleRelations).ThenInclude(x => x.Role).ThenInclude(x => x.RoleRouterRelations).ThenInclude(x => x.Router).FirstOrDefault(x => x.Id == userId);
        }

        public List<User> QueryUserListPage<TOrderKey>(int pageIndex, int pageSize, List<Expression<Func<User, bool>>> wheres, Expression<Func<User, TOrderKey>> orderBy, bool isAsc, out int total)
        {
            var query = _context.Users.AsNoTracking().Where(x => !x.IsDelete);
            if (wheres.Any())
            {
                foreach (var item in wheres)
                {
                    query = query.Where(item);
                }
            }
            total = query.Count();
            query = isAsc ? query.OrderBy(orderBy) : query.OrderByDescending(orderBy);
            return query.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
        }

        public bool AddUser(User user)
        {
            _context.Users.Add(user);
            return _context.SaveChanges() > 0;
        }

        public bool UpdateUser(User user)
        {
            _context.Users.Update(user);
            return _context.SaveChanges() > 0;
        }

        public bool DeleteUser(long id, bool isReal)
        {
            var user = _context.Users.Find(id);
            if (user == null) return false;

            if (isReal)
            {
                _context.Users.Remove(user);
            }
            else
            {
                user.IsDelete = true;
                _context.Users.Update(user);
            }
            return _context.SaveChanges() > 0;
        }



        public List<Router> QueryRouterList()
        {
            return _context.Routers.AsNoTracking().Where(x => x.IsDelete == false).ToList();
        }

        public bool UpdateRouter(Router router)
        {
            _context.Routers.Update(router);
            return _context.SaveChanges() > 0;
        }

        public bool AddRouter(Router router)
        {
            _context.Routers.Add(router);
            return _context.SaveChanges() > 0;
        }

        public bool DeleteRouter(long id, bool isReal)
        {
            var router = _context.Routers.Find(id);
            if (router == null) return false;

            if (isReal)
            {
                _context.Routers.Remove(router);
            }
            else
            {
                router.IsDelete = true;
                _context.Routers.Update(router);
            }
            return _context.SaveChanges() > 0;
        }


        public List<Role> QueryRoleListPage<TOrderKey>(int pageIndex, int pageSize, List<Expression<Func<Role, bool>>> wheres, Expression<Func<Role, TOrderKey>> orderBy, bool isAsc, out int total)
        {
            var query = _context.Roles.AsNoTracking().Include(x => x.RoleRouterRelations).ThenInclude(x => x.Router).Where(x => !x.IsDelete);

            if (wheres.Any())
            {
                foreach (var item in wheres)
                {
                    query = query.Where(item);
                }
            }
            total = query.Count();
            query = isAsc ? query.OrderBy(orderBy) : query.OrderByDescending(orderBy);
            return query.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
        }

        public Role QueryRoleById(long id)
        {
            return _context.Roles.AsNoTracking().Include(x => x.RoleRouterRelations).ThenInclude(x => x.Router).FirstOrDefault(x => x.Id == id);
        }

        public bool UpdateRole(Role role)
        {
            _context.Roles.Update(role);
            return _context.SaveChanges() > 0;
        }

        public bool AddRole(Role role)
        {
            _context.Roles.Add(role);
            return _context.SaveChanges() > 0;
        }

        public bool DeleteRole(long id, bool isReal)
        {
            var role = _context.Roles.Find(id);
            if (role == null) return false;

            if (isReal)
            {
                _context.Roles.Remove(role);
            }
            else
            {
                role.IsDelete = true;
                _context.Roles.Update(role);
            }
            return _context.SaveChanges() > 0;
        }



        public bool AddRoleRouterRelations(List<RoleRouterRelation> addEntities)
        {
            _context.RoleRouterRelations.AddRange(addEntities);
            return _context.SaveChanges() > 0;
        }

        public bool DeleteRoleRouterRelations(long roleId, List<long> deleteRouterIds)
        {
            var deleteEntities = _context.RoleRouterRelations.Where(x => deleteRouterIds.Contains(x.RouterId) && x.RoleId == roleId).ToList();
            _context.RoleRouterRelations.RemoveRange(deleteEntities);
            return _context.SaveChanges() > 0;
        }

        public bool AddUserRoleRelations(List<UserRoleRelation> addEntities)
        {
            _context.UserRoleRelations.AddRange(addEntities);
            return _context.SaveChanges() > 0;
        }

        public bool DeleteUserRoleRelations(long userId, List<long> deleteRoleIds)
        {
            var deleteEntities = _context.UserRoleRelations.Where(x => deleteRoleIds.Contains(x.RoleId) && x.UserId == userId).ToList();
            _context.UserRoleRelations.RemoveRange(deleteEntities);
            return _context.SaveChanges() > 0;
        }
    }
}
