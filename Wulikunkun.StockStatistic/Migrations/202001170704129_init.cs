namespace StockStatistics.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class init : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.exchangecalendar",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Exchange = c.String(maxLength: 256, storeType: "nvarchar"),
                        Cal_Date = c.String(unicode: false),
                        Is_Open = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.stockbasicinfo",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Ts_Code = c.String(maxLength: 50, storeType: "nvarchar"),
                        Name = c.String(maxLength: 256, storeType: "nvarchar"),
                        Area = c.String(maxLength: 256, storeType: "nvarchar"),
                        Industry = c.String(maxLength: 256, storeType: "nvarchar"),
                        List_Date = c.String(maxLength: 256, storeType: "nvarchar"),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.stockbasicinfo");
            DropTable("dbo.exchangecalendar");
        }
    }
}
