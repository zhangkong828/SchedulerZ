using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SchedulerZ.Manager.API.Model.Dto
{
    public class UserDto
    {
        public long Id { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        public string Name { get; set; }

        public string Avatar { get; set; }

        public string LastLoginIp { get; set; }

        public DateTime LastLoginTime { get; set; }

        public int Status { get; set; }

        public DateTime CreateTime { get; set; }

        public bool IsDelete { get; set; }
    }
}
