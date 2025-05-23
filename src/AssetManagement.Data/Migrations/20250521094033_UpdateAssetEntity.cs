using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AssetManagement.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateAssetEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "State",
                table: "Assets",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "InstallDate",
                table: "Assets",
                type: "datetimeoffset",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<int>(
                name: "Location",
                table: "Assets",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Specification",
                table: "Assets",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InstallDate",
                table: "Assets");

            migrationBuilder.DropColumn(
                name: "Location",
                table: "Assets");

            migrationBuilder.DropColumn(
                name: "Specification",
                table: "Assets");

            migrationBuilder.AlterColumn<string>(
                name: "State",
                table: "Assets",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");
        }
    }
}
