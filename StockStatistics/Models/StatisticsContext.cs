using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using MySql.Data.Entity;

namespace StockStatistics.Models
{
    [DbConfigurationType(typeof(MySqlEFConfiguration))]
    public class StatisticsContext : DbContext
    {
        public StatisticsContext() : base("name=TencentMySqlDB")
        {

        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }

    }
}
