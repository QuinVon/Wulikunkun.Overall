﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace wulikunkun_dotnet_core_mvc.Models
{
    public enum Role
    {
        User,
        Admin
    }


    public class User
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(256)]
        public string UserName { get; set; }

        [MaxLength(256)]
        public string City { get; set; }
        [Required]
        [MaxLength(256)]
        public string Email { get; set; }
        public string Age { get; set; }
        [Required]
        [MaxLength(256)]
        public string Password { get; set; }
        [Required]
        [MaxLength(256)]
        public string Salt { get; set; }
        [Required]
        public string Phone { get; set; }
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
