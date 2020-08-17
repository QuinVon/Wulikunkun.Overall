using System;
using Microsoft.EntityFrameworkCore.Migrations;
using MySql.Data.EntityFrameworkCore.Metadata;

namespace Web.Migrations
{
    public partial class Init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    Email = table.Column<string>(maxLength: 256, nullable: false),
                    Age = table.Column<string>(nullable: true),
                    Password = table.Column<string>(maxLength: 256, nullable: false),
                    Salt = table.Column<string>(maxLength: 256, nullable: false),
                    Phone = table.Column<string>(nullable: false),
                    Province = table.Column<string>(maxLength: 256, nullable: true),
                    School = table.Column<string>(maxLength: 256, nullable: true),
                    RegisterTime = table.Column<DateTime>(nullable: false),
                    UserRole = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
