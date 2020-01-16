using MySql.Data.Entity;
using System.Data.Entity;

namespace StockStatistics.Models
{
    [DbConfigurationType(typeof(MySqlEFConfiguration))]
    class StockContext : DbContext
    {
        public StockContext() : base("name=TencentCloudMysql")
        {

        }
    }
}
