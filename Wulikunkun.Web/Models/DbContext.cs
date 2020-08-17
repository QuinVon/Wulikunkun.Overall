using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wulikunkun.Web.Models
{
    public class WangKunDbContext : DbContext
    {

        public WangKunDbContext(DbContextOptions<WangKunDbContext> options) : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Article>().HasOne<User>(_ => _.User).WithMany(_ => _.Articles).HasForeignKey(_ => _.UserId);
            modelBuilder.Entity<Article>().HasOne<Category>(_ => _.Category).WithMany(_ => _.Articles).HasForeignKey(_ => _.CategoryId);
            modelBuilder.Entity<Log>().HasOne<User>(_ => _.User).WithMany(_ => _.Logs).HasForeignKey(_ => _.UserId);
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Log> Logs { get; set; }
        public DbSet<Article> Articles { get; set; }
        public DbSet<Category> Categories { get; set; }
    }
}
