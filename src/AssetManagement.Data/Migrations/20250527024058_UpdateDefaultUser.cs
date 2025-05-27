using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AssetManagement.Data.Migrations
{
    /// <inheritdoc />
    [ExcludeFromCodeCoverage]
    public partial class UpdateDefaultUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("1f2b0a56-3c8e-4771-b227-69e2cd5e41a3"),
                column: "Password",
                value: "Xq4XCC8U9vadB+YSKnXCeOL45zqs4xg3TbdQoCi/5uUkzqIfBeayAIxn7sxp7XSK");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("c87d8f20-c381-4970-b367-09f2cf95d655"),
                column: "Password",
                value: "Xq4XCC8U9vadB+YSKnXCeOL45zqs4xg3TbdQoCi/5uUkzqIfBeayAIxn7sxp7XSK");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("1f2b0a56-3c8e-4771-b227-69e2cd5e41a3"),
                column: "Password",
                value: "BwhI/sZn44902IWu6WLuEZu3XNhC2/412ZEJb5Rte09IfHoIsgI8Na1pTrwzTXrz");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("c87d8f20-c381-4970-b367-09f2cf95d655"),
                column: "Password",
                value: "BwhI/sZn44902IWu6WLuEZu3XNhC2/412ZEJb5Rte09IfHoIsgI8Na1pTrwzTXrz");
        }
    }
}
