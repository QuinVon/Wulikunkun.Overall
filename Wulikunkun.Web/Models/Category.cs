using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Wulikunkun.Web.Models
{
    public class Category
    {
        public int Id { get; set; }
        [MaxLength(256)]
        public string Name { get; set; }
        public IEnumerable<Article> Articles { get; set; }

        /* 分类应当是由用户自己添加的 */
        public virtual string UserId { get; set; }
        public virtual ApplicationUser User { get; set; }
    }
}