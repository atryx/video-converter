using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace VideoApp.Web.Migrations
{
    public partial class UpdateEntities : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Thumbnails",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: true),
                    Format = table.Column<string>(nullable: true),
                    Timestamp = table.Column<TimeSpan>(nullable: false),
                    ParentVideoFileId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Thumbnails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Thumbnails_Videos_ParentVideoFileId",
                        column: x => x.ParentVideoFileId,
                        principalTable: "Videos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Thumbnails_ParentVideoFileId",
                table: "Thumbnails",
                column: "ParentVideoFileId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Thumbnails");
        }
    }
}
