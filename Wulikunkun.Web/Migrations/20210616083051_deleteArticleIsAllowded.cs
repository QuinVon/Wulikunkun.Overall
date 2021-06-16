using Microsoft.EntityFrameworkCore.Migrations;

namespace Web.Migrations
{
    public partial class deleteArticleIsAllowded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsAllowed",
                table: "Articles");

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "Articles",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "Articles",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddColumn<short>(
                name: "IsAllowed",
                table: "Articles",
                type: "bit",
                nullable: false,
                defaultValue: (short)0);
        }
    }
}
