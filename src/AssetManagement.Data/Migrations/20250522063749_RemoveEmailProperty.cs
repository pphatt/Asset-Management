using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AssetManagement.Data.Migrations
{
    /// <inheritdoc />
    public partial class RemoveEmailProperty : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Email",
                table: "Users");

            migrationBuilder.RenameColumn(
                name: "LastModificatedDate",
                table: "Users",
                newName: "LastModifiedDate");

            migrationBuilder.RenameColumn(
                name: "LastModificatedBy",
                table: "Users",
                newName: "LastModifiedBy");

            migrationBuilder.RenameColumn(
                name: "LastModificatedDate",
                table: "Assets",
                newName: "LastModifiedDate");

            migrationBuilder.RenameColumn(
                name: "LastModificatedBy",
                table: "Assets",
                newName: "LastModifiedBy");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "LastModifiedDate",
                table: "Users",
                newName: "LastModificatedDate");

            migrationBuilder.RenameColumn(
                name: "LastModifiedBy",
                table: "Users",
                newName: "LastModificatedBy");

            migrationBuilder.RenameColumn(
                name: "LastModifiedDate",
                table: "Assets",
                newName: "LastModificatedDate");

            migrationBuilder.RenameColumn(
                name: "LastModifiedBy",
                table: "Assets",
                newName: "LastModificatedBy");

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Users",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("1f2b0a56-3c8e-4771-b227-69e2cd5e41a3"),
                column: "Email",
                value: "minhnguyen@example.com");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("2b6d4d81-7305-4cd3-9104-760aa2d9b80a"),
                column: "Email",
                value: "linhtran@example.com");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("3c57b69c-d9a0-4678-b94f-ad4533be6e81"),
                column: "Email",
                value: "tuanpham@example.com");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("4f38d797-4168-4c55-8884-76f1f2d3f79b"),
                column: "Email",
                value: "hoale@example.com");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("c87d8f20-c381-4970-b367-09f2cf95d655"),
                column: "Email",
                value: "nghiadinh@example.com");
        }
    }
}
