using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace AssetManagement.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateAssetEntitySeed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Assets_Categories_CategoryId",
                table: "Assets");

            migrationBuilder.AddColumn<Guid>(
                name: "CategoryId1",
                table: "Assets",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "CreatedBy", "CreatedDate", "DeletedBy", "DeletedDate", "IsDeleted", "LastModificatedBy", "LastModificatedDate", "Name", "Slug" },
                values: new object[,]
                {
                    { new Guid("123e4567-e89b-12d3-a456-426614174001"), null, new DateTime(2025, 5, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, null, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Bluetooth Mouse", "Bluetooth Mouse" },
                    { new Guid("123e4567-e89b-12d3-a456-426614174002"), null, new DateTime(2025, 5, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, null, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Headset", "Headset" },
                    { new Guid("123e4567-e89b-12d3-a456-426614174003"), null, new DateTime(2025, 5, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, null, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Ipad", "Ipad" },
                    { new Guid("123e4567-e89b-12d3-a456-426614174004"), null, new DateTime(2025, 5, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, null, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Iphone", "Iphone" },
                    { new Guid("123e4567-e89b-12d3-a456-426614174005"), null, new DateTime(2025, 5, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, null, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Laptop", "Laptop" },
                    { new Guid("123e4567-e89b-12d3-a456-426614174006"), null, new DateTime(2025, 5, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, null, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Mobile", "Mobile" },
                    { new Guid("123e4567-e89b-12d3-a456-426614174007"), null, new DateTime(2025, 5, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, null, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Monitor", "Monitor" },
                    { new Guid("123e4567-e89b-12d3-a456-426614174008"), null, new DateTime(2025, 5, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, null, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Personal Computer", "Personal Computer" },
                    { new Guid("123e4567-e89b-12d3-a456-426614174009"), null, new DateTime(2025, 5, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, null, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Tablet", "Tablet" }
                });

            migrationBuilder.InsertData(
                table: "Assets",
                columns: new[] { "Id", "AssetCode", "CategoryId", "CategoryId1", "CreatedBy", "CreatedDate", "DeletedBy", "DeletedDate", "InstallDate", "IsDeleted", "LastModificatedBy", "LastModificatedDate", "Location", "Name", "Specification", "State" },
                values: new object[,]
                {
                    { new Guid("223e4567-e89b-12d3-a456-426614174001"), "LAP-001", new Guid("123e4567-e89b-12d3-a456-426614174005"), null, null, new DateTime(2025, 5, 21, 0, 0, 0, 0, DateTimeKind.Utc), null, null, new DateTimeOffset(new DateTime(2023, 5, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, "Dell XPS 13", "Intel i7, 16GB RAM, 512GB SSD", 1 },
                    { new Guid("223e4567-e89b-12d3-a456-426614174002"), "LAP-002", new Guid("123e4567-e89b-12d3-a456-426614174005"), null, null, new DateTime(2025, 5, 21, 0, 0, 0, 0, DateTimeKind.Utc), null, null, new DateTimeOffset(new DateTime(2024, 5, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, "MacBook Pro 14", "M1 Pro, 32GB RAM, 1TB SSD", 1 },
                    { new Guid("223e4567-e89b-12d3-a456-426614174003"), "MON-001", new Guid("123e4567-e89b-12d3-a456-426614174007"), null, null, new DateTime(2025, 5, 21, 0, 0, 0, 0, DateTimeKind.Utc), null, null, new DateTimeOffset(new DateTime(2022, 5, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, "Dell UltraSharp 27", "27-inch, 4K, IPS Panel", 1 },
                    { new Guid("223e4567-e89b-12d3-a456-426614174004"), "MON-002", new Guid("123e4567-e89b-12d3-a456-426614174007"), null, null, new DateTime(2025, 5, 21, 0, 0, 0, 0, DateTimeKind.Utc), null, null, new DateTimeOffset(new DateTime(2023, 5, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, "LG 32GN600", "32-inch, QHD, 144Hz", 1 },
                    { new Guid("223e4567-e89b-12d3-a456-426614174005"), "IPD-001", new Guid("123e4567-e89b-12d3-a456-426614174003"), null, null, new DateTime(2025, 5, 21, 0, 0, 0, 0, DateTimeKind.Utc), null, null, new DateTimeOffset(new DateTime(2023, 11, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, "iPad Pro 12.9", "M2 Chip, 256GB, Wi-Fi", 1 },
                    { new Guid("223e4567-e89b-12d3-a456-426614174006"), "IPD-002", new Guid("123e4567-e89b-12d3-a456-426614174003"), null, null, new DateTime(2025, 5, 21, 0, 0, 0, 0, DateTimeKind.Utc), null, null, new DateTimeOffset(new DateTime(2024, 11, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 3, "iPad Air 5", "M1 Chip, 64GB, Wi-Fi", 1 },
                    { new Guid("223e4567-e89b-12d3-a456-426614174007"), "IPH-001", new Guid("123e4567-e89b-12d3-a456-426614174004"), null, null, new DateTime(2025, 5, 21, 0, 0, 0, 0, DateTimeKind.Utc), null, null, new DateTimeOffset(new DateTime(2024, 5, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, "iPhone 14 Pro", "256GB, Black, 5G", 1 },
                    { new Guid("223e4567-e89b-12d3-a456-426614174008"), "MOU-001", new Guid("123e4567-e89b-12d3-a456-426614174001"), null, null, new DateTime(2025, 5, 21, 0, 0, 0, 0, DateTimeKind.Utc), null, null, new DateTimeOffset(new DateTime(2024, 5, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, "Logitech MX Master 3", "Wireless, 4000 DPI, Ergonomic", 1 },
                    { new Guid("223e4567-e89b-12d3-a456-426614174009"), "HST-001", new Guid("123e4567-e89b-12d3-a456-426614174002"), null, null, new DateTime(2025, 5, 21, 0, 0, 0, 0, DateTimeKind.Utc), null, null, new DateTimeOffset(new DateTime(2024, 9, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, "Sony WH-1000XM5", "Wireless, Noise-Cancelling, 30hr Battery", 1 },
                    { new Guid("223e4567-e89b-12d3-a456-426614174010"), "PC-001", new Guid("123e4567-e89b-12d3-a456-426614174008"), null, null, new DateTime(2025, 5, 21, 0, 0, 0, 0, DateTimeKind.Utc), null, null, new DateTimeOffset(new DateTime(2020, 5, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 3, "HP EliteDesk 800", "Intel i5, 8GB RAM, 1TB HDD", 1 },
                    { new Guid("223e4567-e89b-12d3-a456-426614174011"), "TAB-001", new Guid("123e4567-e89b-12d3-a456-426614174009"), null, null, new DateTime(2025, 5, 21, 0, 0, 0, 0, DateTimeKind.Utc), null, null, new DateTimeOffset(new DateTime(2024, 7, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, "Samsung Galaxy Tab S8", "Snapdragon 8, 128GB, 11-inch", 1 },
                    { new Guid("223e4567-e89b-12d3-a456-426614174012"), "MOB-001", new Guid("123e4567-e89b-12d3-a456-426614174006"), null, null, new DateTime(2025, 5, 21, 0, 0, 0, 0, DateTimeKind.Utc), null, null, new DateTimeOffset(new DateTime(2025, 1, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 3, "Samsung Galaxy S23", "256GB, 5G, Snapdragon 8 Gen 2", 1 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Assets_CategoryId1",
                table: "Assets",
                column: "CategoryId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Assets_Categories_CategoryId",
                table: "Assets",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Assets_Categories_CategoryId1",
                table: "Assets",
                column: "CategoryId1",
                principalTable: "Categories",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Assets_Categories_CategoryId",
                table: "Assets");

            migrationBuilder.DropForeignKey(
                name: "FK_Assets_Categories_CategoryId1",
                table: "Assets");

            migrationBuilder.DropIndex(
                name: "IX_Assets_CategoryId1",
                table: "Assets");

            migrationBuilder.DeleteData(
                table: "Assets",
                keyColumn: "Id",
                keyValue: new Guid("223e4567-e89b-12d3-a456-426614174001"));

            migrationBuilder.DeleteData(
                table: "Assets",
                keyColumn: "Id",
                keyValue: new Guid("223e4567-e89b-12d3-a456-426614174002"));

            migrationBuilder.DeleteData(
                table: "Assets",
                keyColumn: "Id",
                keyValue: new Guid("223e4567-e89b-12d3-a456-426614174003"));

            migrationBuilder.DeleteData(
                table: "Assets",
                keyColumn: "Id",
                keyValue: new Guid("223e4567-e89b-12d3-a456-426614174004"));

            migrationBuilder.DeleteData(
                table: "Assets",
                keyColumn: "Id",
                keyValue: new Guid("223e4567-e89b-12d3-a456-426614174005"));

            migrationBuilder.DeleteData(
                table: "Assets",
                keyColumn: "Id",
                keyValue: new Guid("223e4567-e89b-12d3-a456-426614174006"));

            migrationBuilder.DeleteData(
                table: "Assets",
                keyColumn: "Id",
                keyValue: new Guid("223e4567-e89b-12d3-a456-426614174007"));

            migrationBuilder.DeleteData(
                table: "Assets",
                keyColumn: "Id",
                keyValue: new Guid("223e4567-e89b-12d3-a456-426614174008"));

            migrationBuilder.DeleteData(
                table: "Assets",
                keyColumn: "Id",
                keyValue: new Guid("223e4567-e89b-12d3-a456-426614174009"));

            migrationBuilder.DeleteData(
                table: "Assets",
                keyColumn: "Id",
                keyValue: new Guid("223e4567-e89b-12d3-a456-426614174010"));

            migrationBuilder.DeleteData(
                table: "Assets",
                keyColumn: "Id",
                keyValue: new Guid("223e4567-e89b-12d3-a456-426614174011"));

            migrationBuilder.DeleteData(
                table: "Assets",
                keyColumn: "Id",
                keyValue: new Guid("223e4567-e89b-12d3-a456-426614174012"));

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("123e4567-e89b-12d3-a456-426614174001"));

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("123e4567-e89b-12d3-a456-426614174002"));

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("123e4567-e89b-12d3-a456-426614174003"));

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("123e4567-e89b-12d3-a456-426614174004"));

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("123e4567-e89b-12d3-a456-426614174005"));

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("123e4567-e89b-12d3-a456-426614174006"));

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("123e4567-e89b-12d3-a456-426614174007"));

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("123e4567-e89b-12d3-a456-426614174008"));

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("123e4567-e89b-12d3-a456-426614174009"));

            migrationBuilder.DropColumn(
                name: "CategoryId1",
                table: "Assets");

            migrationBuilder.AddForeignKey(
                name: "FK_Assets_Categories_CategoryId",
                table: "Assets",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
