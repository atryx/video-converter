using Microsoft.EntityFrameworkCore.Migrations;

namespace VideoApp.Web.Migrations
{
    public partial class InitialMigration2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AvailableResolutions",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: true),
                    Resolution = table.Column<string>(nullable: true),
                    Category = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AvailableResolutions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Videos",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Filename = table.Column<string>(nullable: true),
                    FormatName = table.Column<string>(nullable: true),
                    StartTime = table.Column<string>(nullable: true),
                    Duration = table.Column<string>(nullable: true),
                    Size = table.Column<string>(nullable: true),
                    BitRate = table.Column<string>(nullable: true),
                    CodecName = table.Column<string>(nullable: true),
                    Height = table.Column<int>(nullable: false),
                    Width = table.Column<int>(nullable: false),
                    SampleAspectRatio = table.Column<string>(nullable: true),
                    DisplayAspectRatio = table.Column<string>(nullable: true),
                    ParentVideoFileId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Videos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Videos_Videos_ParentVideoFileId",
                        column: x => x.ParentVideoFileId,
                        principalTable: "Videos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Videos_ParentVideoFileId",
                table: "Videos",
                column: "ParentVideoFileId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AvailableResolutions");

            migrationBuilder.DropTable(
                name: "Videos");
        }
    }
}
