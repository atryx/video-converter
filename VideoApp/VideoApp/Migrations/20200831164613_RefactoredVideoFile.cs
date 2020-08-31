using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace VideoApp.Web.Migrations
{
    public partial class RefactoredVideoFile : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CodecName",
                table: "Videos");

            migrationBuilder.DropColumn(
                name: "DisplayAspectRatio",
                table: "Videos");

            migrationBuilder.DropColumn(
                name: "FormatName",
                table: "Videos");

            migrationBuilder.DropColumn(
                name: "SampleAspectRatio",
                table: "Videos");

            migrationBuilder.DropColumn(
                name: "StartTime",
                table: "Videos");

            migrationBuilder.AlterColumn<long>(
                name: "Size",
                table: "Videos",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<TimeSpan>(
                name: "Duration",
                table: "Videos",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "BitRate",
                table: "Videos",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Codec",
                table: "Videos",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Framerate",
                table: "Videos",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<string>(
                name: "PixelFormat",
                table: "Videos",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Ratio",
                table: "Videos",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Codec",
                table: "Videos");

            migrationBuilder.DropColumn(
                name: "Framerate",
                table: "Videos");

            migrationBuilder.DropColumn(
                name: "PixelFormat",
                table: "Videos");

            migrationBuilder.DropColumn(
                name: "Ratio",
                table: "Videos");

            migrationBuilder.AlterColumn<string>(
                name: "Size",
                table: "Videos",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(long));

            migrationBuilder.AlterColumn<string>(
                name: "Duration",
                table: "Videos",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(TimeSpan));

            migrationBuilder.AlterColumn<string>(
                name: "BitRate",
                table: "Videos",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(long));

            migrationBuilder.AddColumn<string>(
                name: "CodecName",
                table: "Videos",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DisplayAspectRatio",
                table: "Videos",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FormatName",
                table: "Videos",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SampleAspectRatio",
                table: "Videos",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StartTime",
                table: "Videos",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
