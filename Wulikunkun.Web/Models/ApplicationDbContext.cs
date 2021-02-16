using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wulikunkun.Web.Models
{
    public class ApplicationDbContext : IdentityDbContext
    {

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Article>().HasOne<ApplicationUser>(article => article.User).WithMany(user => user.Articles).HasForeignKey(article => article.UserId);
            modelBuilder.Entity<Article>().HasOne<Category>(article => article.Category).WithMany(category => category.Articles).HasForeignKey(article => article.CategoryId);
            modelBuilder.Entity<ApplicationUser>().Property(user => user.Id).HasMaxLength(36);
            modelBuilder.Entity<IdentityRole>().Property(role => role.Id).HasMaxLength(36);
        }

        public DbSet<Log> Logs { get; set; }
        public DbSet<Article> Articles { get; set; }
        public DbSet<Category> Categories { get; set; }
    }
}
