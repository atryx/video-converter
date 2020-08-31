using Microsoft.EntityFrameworkCore.Migrations;

namespace VideoApp.Web.Migrations
{
    public partial class AddedStatus : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Videos",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "Videos");
        }
    }
}
