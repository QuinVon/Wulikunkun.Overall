using Microsoft.EntityFrameworkCore.Migrations;

namespace Web.Migrations
{
    public partial class 修改用户年龄字段类型 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<short>(
                name: "Age",
                table: "Users",
                nullable: true,
                oldClrType: typeof(short),
                oldType: "smallint");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<short>(
                name: "Age",
                table: "Users",
                type: "smallint",
                nullable: false,
                oldClrType: typeof(short),
                oldNullable: true);
        }
    }
}
