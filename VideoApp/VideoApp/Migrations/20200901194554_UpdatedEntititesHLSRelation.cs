using Microsoft.EntityFrameworkCore.Migrations;

namespace VideoApp.Web.Migrations
{
    public partial class UpdatedEntititesHLSRelation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ParentVideoFileId",
                table: "HLS",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_HLS_ParentVideoFileId",
                table: "HLS",
                column: "ParentVideoFileId");

            migrationBuilder.AddForeignKey(
                name: "FK_HLS_Videos_ParentVideoFileId",
                table: "HLS",
                column: "ParentVideoFileId",
                principalTable: "Videos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HLS_Videos_ParentVideoFileId",
                table: "HLS");

            migrationBuilder.DropIndex(
                name: "IX_HLS_ParentVideoFileId",
                table: "HLS");

            migrationBuilder.DropColumn(
                name: "ParentVideoFileId",
                table: "HLS");
        }
    }
}
