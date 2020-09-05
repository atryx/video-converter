using Microsoft.EntityFrameworkCore.Migrations;

namespace VideoApp.Web.Migrations
{
    public partial class EntitiesChangeLocationName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FileLocation",
                table: "Videos");

            migrationBuilder.RenameColumn(
                name: "FileName",
                table: "HLS",
                newName: "Filename");

            migrationBuilder.AddColumn<string>(
                name: "FileDirectory",
                table: "Videos",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FileDirectory",
                table: "HLS",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FileDirectory",
                table: "Videos");

            migrationBuilder.DropColumn(
                name: "FileDirectory",
                table: "HLS");

            migrationBuilder.RenameColumn(
                name: "Filename",
                table: "HLS",
                newName: "FileName");

            migrationBuilder.AddColumn<string>(
                name: "FileLocation",
                table: "Videos",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
