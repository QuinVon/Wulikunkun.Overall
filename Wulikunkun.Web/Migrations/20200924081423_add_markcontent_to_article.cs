using Microsoft.EntityFrameworkCore.Migrations;

namespace Web.Migrations
{
    public partial class add_markcontent_to_article : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Content",
                table: "Articles");

            migrationBuilder.AddColumn<string>(
                name: "HtmlContent",
                table: "Articles",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MarkContent",
                table: "Articles",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HtmlContent",
                table: "Articles");

            migrationBuilder.DropColumn(
                name: "MarkContent",
                table: "Articles");

            migrationBuilder.AddColumn<string>(
                name: "Content",
                table: "Articles",
                type: "text",
                nullable: true);
        }
    }
}
