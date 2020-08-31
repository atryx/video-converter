using Microsoft.EntityFrameworkCore.Migrations;

namespace VideoApp.Web.Migrations
{
    public partial class FixedEnums : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Videos",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "Videos",
                type: "int",
                nullable: false,
                oldClrType: typeof(string));
        }
    }
}
