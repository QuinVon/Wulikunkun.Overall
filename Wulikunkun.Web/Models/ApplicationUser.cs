using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace Wulikunkun.Web.Models
{
    // 将自己设置角色注释掉
    // public enum Role
    // {
    //     CommonUser,
    //     VipUser,
    //     SuperVipUser,
    //     Admin,
    //     SuperAdmin,
    //     Forbidden
    // }

    // 对IdentityUesr的扩展不必放入ApplicationDbContext中
    public class ApplicationUser : IdentityUser
    {
        // IdentityUser已经包含Id和Email，所以这里进行注释
        // [Key] public int Id { get; set; }

        // [Required] [MaxLength(256)] public string Email { get; set; }
        public sbyte? Age { get; set; }
        [MaxLength(256)] public string Province { get; set; }
        [MaxLength(256)] public string School { get; set; }
        [Required] public DateTime RegisterTime { get; set; }
        [Required] public bool IsActive { get; set; }

        // [Required] public Role UserRole { get; set; }
        public IEnumerable<Article> Articles { get; set; }
        public IEnumerable<Log> Logs { get; set; }
        public IEnumerable<Category> Categories { get; set; }
    }
}