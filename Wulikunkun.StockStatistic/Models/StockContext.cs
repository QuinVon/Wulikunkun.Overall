using MySql.Data.Entity;
using System.Data.Entity;

namespace Wulikunkun.StockStatistic.Models
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
            modelBuilder.Entity<ExchangeCalendar>().ToTable("exchangecalendar", "dbo");
        }
        public virtual DbSet<StockBasicInfo> StockBasicInfos { get; set; }
        public virtual DbSet<ExchangeCalendar> ExchangeCalendars { get; set; }
        public virtual DbSet<PriceDaily> PriceDailys { get; set; }
    }
}
