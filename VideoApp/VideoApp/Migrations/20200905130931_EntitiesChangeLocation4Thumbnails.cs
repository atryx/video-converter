using Microsoft.EntityFrameworkCore.Migrations;

namespace VideoApp.Web.Migrations
{
    public partial class EntitiesChangeLocation4Thumbnails : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FileLocation",
                table: "Thumbnails");

            migrationBuilder.AddColumn<string>(
                name: "FileDirectory",
                table: "Thumbnails",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FileDirectory",
                table: "Thumbnails");

            migrationBuilder.AddColumn<string>(
                name: "FileLocation",
                table: "Thumbnails",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
