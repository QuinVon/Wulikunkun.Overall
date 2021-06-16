using Microsoft.EntityFrameworkCore.Migrations;

namespace Web.Migrations
{
    public partial class deleteArticleDeleted : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Articles");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<short>(
                name: "IsDeleted",
                table: "Articles",
                type: "bit",
                nullable: false,
                defaultValue: (short)0);
        }
    }
}
