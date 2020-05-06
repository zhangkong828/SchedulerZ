using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SchedulerZ.Stores
{
    public class UserEntity
    {
        [Key]
        public string Id { get; set; }

        [Required]
        public string UserName { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public string RealName { get; set; }

        [MaxLength(15)]
        public string Phone { get; set; }

        [MaxLength(100), EmailAddress(ErrorMessage = "邮箱格式错误")]
        public string Email { get; set; }

        [Required]
        public int Status { get; set; }

        public DateTime CreateTime { get; set; }

        public DateTime LastLoginTime { get; set; }
    }
}
