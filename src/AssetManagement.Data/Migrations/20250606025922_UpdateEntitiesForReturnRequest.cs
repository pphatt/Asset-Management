using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AssetManagement.Data.Migrations
{
    /// <inheritdoc />
    [ExcludeFromCodeCoverage]
    public partial class UpdateEntitiesForReturnRequest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ReturnRequests_AssignmentId",
                table: "ReturnRequests");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "ReturnedDate",
                table: "ReturnRequests",
                type: "datetimeoffset",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset");

            migrationBuilder.CreateIndex(
                name: "IX_ReturnRequests_AssignmentId",
                table: "ReturnRequests",
                column: "AssignmentId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ReturnRequests_AssignmentId",
                table: "ReturnRequests");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "ReturnedDate",
                table: "ReturnRequests",
                type: "datetimeoffset",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)),
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ReturnRequests_AssignmentId",
                table: "ReturnRequests",
                column: "AssignmentId");
        }
    }
}
