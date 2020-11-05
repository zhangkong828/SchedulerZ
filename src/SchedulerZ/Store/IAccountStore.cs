using SchedulerZ.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SchedulerZ.Store
{
    public interface IAccountStore
    {
        User QueryUserById(long id);
        User QueryUserByName(string userName);
        User QueryUserInfo(long userId);

        bool UpdateUser(User user);

        
    }
}
