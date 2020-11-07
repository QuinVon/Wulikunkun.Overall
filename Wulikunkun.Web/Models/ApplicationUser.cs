using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace Wulikunkun.Web.Models
{
    public enum Role
    {
        CommonUser,
        VipUser,
        SuperVipUser,
        Admin,
        SuperAdmin,
        Forbidden
    }


    public class ApplicationUser:IdentityUser
    {
        // IdentityUser已经包含Id和Email，所以这里进行注释
        // [Key] public int Id { get; set; }

        // [Required] [MaxLength(256)] public string Email { get; set; }
        public sbyte? Age { get; set; }
        [Required] [MaxLength(256)] public string Password { get; set; }
        [Required] [MaxLength(256)] public string Salt { get; set; }
        public int? Phone { get; set; }
        [MaxLength(256)] public string Province { get; set; }
        [MaxLength(256)] public string School { get; set; }
        [Required] public DateTime RegisterTime { get; set; }
        public int? ActiveCode { get; set; }
        [Required] public bool IsActive { get; set; }
        [Required] public Role UserRole { get; set; }
        public IEnumerable<Article> Articles { get; set; }
        public IEnumerable<Log> Logs { get; set; }
    }
}