using Microsoft.EntityFrameworkCore;
using SchedulerZ.Models;
using System;
using System.Collections.Generic;
using System.Linq;
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
            return _context.Users.AsNoTracking().SingleOrDefault(x => x.Id == id);
        }

        public User QueryUserByName(string userName)
        {
            return _context.Users.AsNoTracking().SingleOrDefault(x => x.Username == userName);
        }

        public User QueryUserInfo(long userId)
        {
            return _context.Users.AsNoTracking().Include(x => x.UserRoleRelations).ThenInclude(x => x.Role).ThenInclude(x => x.RoleRouterRelations).ThenInclude(x => x.Router).FirstOrDefault(x => x.Id == userId);
        }

        public bool UpdateUser(User user)
        {
            _context.Users.Update(user);
            return _context.SaveChanges() > 0;
        }
    }
}
