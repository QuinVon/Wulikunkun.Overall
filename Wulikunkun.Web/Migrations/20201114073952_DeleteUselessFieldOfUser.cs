using Microsoft.EntityFrameworkCore.Migrations;

namespace Web.Migrations
{
    public partial class DeleteUselessFieldOfUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Phone",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Salt",
                table: "AspNetUsers");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Phone",
                table: "AspNetUsers",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Salt",
                table: "AspNetUsers",
                type: "varchar(256)",
                maxLength: 256,
                nullable: true);
        }
    }
}
