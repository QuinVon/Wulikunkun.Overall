using Microsoft.EntityFrameworkCore.Migrations;

namespace Web.Migrations
{
    public partial class 添加文章查看次数词字段 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ViewTimes",
                table: "Articles",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ViewTimes",
                table: "Articles");
        }
    }
}
