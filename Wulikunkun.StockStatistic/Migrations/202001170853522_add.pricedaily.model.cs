namespace StockStatistics.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addpricedailymodel : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.PriceDailies",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Ts_Code = c.String(maxLength: 50, storeType: "nvarchar"),
                        Trade_Date = c.String(maxLength: 50, storeType: "nvarchar"),
                        Open = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Hight = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Low = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Close = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Pre_Close = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Change = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Pct_Chg = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Vol = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.PriceDailies");
        }
    }
}
