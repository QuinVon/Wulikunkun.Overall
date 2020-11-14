using Microsoft.EntityFrameworkCore.Migrations;

namespace Web.Migrations
{
    public partial class DeleteActiveCodeOfUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ActiveCode",
                table: "AspNetUsers");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ActiveCode",
                table: "AspNetUsers",
                type: "int",
                nullable: true);
        }
    }
}
