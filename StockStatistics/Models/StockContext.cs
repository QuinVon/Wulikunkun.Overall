using MySql.Data.Entity;
using System.Data.Entity;

namespace Wangkun.StockStatistic.Models
{
    [DbConfigurationType(typeof(MySqlEFConfiguration))]
    class StockContext : DbContext
    {
        public StockContext() : base("name=TencentCloudMysql")
        {

        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<StockBasicInfo>().ToTable("stockbasicinfo", "dbo");
        }
        public virtual DbSet<StockBasicInfo> StockBasicInfos { get; set; }

    }
}
