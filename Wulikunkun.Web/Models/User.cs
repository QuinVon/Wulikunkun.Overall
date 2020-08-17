﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Wulikunkun.Web.Models
{
    public enum Role
    {
        CommonUser,
        VipUser,
        SuperVipUser,
        Admin,
        SuperAdmin
    }


    public class User
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(256)]
        public string Email { get; set; }
        public sbyte Age { get; set; }

        [Required]
        [MaxLength(256)]
        public string Password { get; set; }
        [Required]
        [MaxLength(256)]
        public string Salt { get; set; }

        [Required]
        public int Phone { get; set; }
        [MaxLength(256)]
        public string Province { get; set; }
        [MaxLength(256)]
        public string School { get; set; }
        [Required]
        public DateTime RegisterTime
        {
            get; set;
        }

        [Required]
        public Role UserRole { get; set; }
    }
}
