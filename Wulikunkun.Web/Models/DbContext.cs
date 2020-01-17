using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace wulikunkun_dotnet_core_mvc.Models
{
    public class WangKunDbContext : DbContext
    {

        public WangKunDbContext(DbContextOptions<WangKunDbContext> options) : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
        }

        public DbSet<User> Users { get; set; }
    }
}
