using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace AssetManagement.Data.Migrations
{
    /// <inheritdoc />
    [ExcludeFromCodeCoverage]
    public partial class UpdateUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("44444444-4444-4444-4444-444444444444"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("55555555-5555-5555-5555-555555555555"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("66666666-6666-6666-6666-666666666666"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777777"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("88888888-8888-8888-8888-888888888888"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("99999999-9999-9999-9999-999999999999"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("cccccccc-cccc-cccc-cccc-cccccccccccc"));

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Users",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CreatedBy", "CreatedDate", "DateOfBirth", "DeletedBy", "DeletedDate", "Email", "FirstName", "Gender", "IsActive", "IsDeleted", "IsPasswordUpdated", "JoinedDate", "LastModificatedBy", "LastModificatedDate", "LastName", "Location", "Password", "StaffCode", "Type", "Username" },
                values: new object[,]
                {
                    { new Guid("1f2b0a56-3c8e-4771-b227-69e2cd5e41a3"), null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(1992, 5, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, "minhnguyen@example.com", "Minh", 1, true, null, false, new DateTimeOffset(new DateTime(2021, 3, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 7, 0, 0, 0)), null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Nguyen", 1, "g7/Bvucy8BU+fYIA9ibE0gACIvZ6mPvm7Jf8EBXicQUgjeTEUvSyt1KrLXUPZmGW", "SD0002", 2, "minhnguyen" },
                    { new Guid("2b6d4d81-7305-4cd3-9104-760aa2d9b80a"), null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(1994, 9, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, "linhtran@example.com", "Linh", 2, true, null, false, new DateTimeOffset(new DateTime(2022, 2, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 7, 0, 0, 0)), null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Tran", 1, "g7/Bvucy8BU+fYIA9ibE0gACIvZ6mPvm7Jf8EBXicQUgjeTEUvSyt1KrLXUPZmGW", "SD0003", 2, "linhtran" },
                    { new Guid("3c57b69c-d9a0-4678-b94f-ad4533be6e81"), null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(1991, 11, 23, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, "tuanpham@example.com", "Tuan", 1, true, null, false, new DateTimeOffset(new DateTime(2021, 7, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 7, 0, 0, 0)), null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Pham", 3, "g7/Bvucy8BU+fYIA9ibE0gACIvZ6mPvm7Jf8EBXicQUgjeTEUvSyt1KrLXUPZmGW", "SD0004", 2, "tuanpham" },
                    { new Guid("4f38d797-4168-4c55-8884-76f1f2d3f79b"), null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(1993, 3, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, "hoale@example.com", "Hoa", 2, false, null, false, new DateTimeOffset(new DateTime(2020, 9, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 7, 0, 0, 0)), null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Le", 1, "g7/Bvucy8BU+fYIA9ibE0gACIvZ6mPvm7Jf8EBXicQUgjeTEUvSyt1KrLXUPZmGW", "SD0005", 2, "hoale" },
                    { new Guid("c87d8f20-c381-4970-b367-09f2cf95d655"), null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(1990, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, "nghiadinh@example.com", "Nghia", 1, true, null, false, new DateTimeOffset(new DateTime(2020, 1, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 7, 0, 0, 0)), null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Dinh", 1, "g7/Bvucy8BU+fYIA9ibE0gACIvZ6mPvm7Jf8EBXicQUgjeTEUvSyt1KrLXUPZmGW", "SD0001", 1, "nghiadinh" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("1f2b0a56-3c8e-4771-b227-69e2cd5e41a3"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("2b6d4d81-7305-4cd3-9104-760aa2d9b80a"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("3c57b69c-d9a0-4678-b94f-ad4533be6e81"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("4f38d797-4168-4c55-8884-76f1f2d3f79b"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("c87d8f20-c381-4970-b367-09f2cf95d655"));

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Users");

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CreatedBy", "CreatedDate", "DateOfBirth", "DeletedBy", "DeletedDate", "Email", "FirstName", "Gender", "IsDeleted", "IsPasswordUpdated", "JoinedDate", "LastModificatedBy", "LastModificatedDate", "LastName", "Location", "Password", "StaffCode", "Type", "Username" },
                values: new object[,]
                {
                    { new Guid("11111111-1111-1111-1111-111111111111"), null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(1990, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, "johndoe@example.com", "John", 1, null, false, new DateTimeOffset(new DateTime(2020, 1, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 7, 0, 0, 0)), null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Doe", 1, "Password@123", "SD0001", 1, "johndoe" },
                    { new Guid("22222222-2222-2222-2222-222222222222"), null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(1992, 2, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, "janesmith@example.com", "Jane", 2, null, false, new DateTimeOffset(new DateTime(2020, 2, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 7, 0, 0, 0)), null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Smith", 2, "Password@123", "SD0002", 2, "janesmith" },
                    { new Guid("33333333-3333-3333-3333-333333333333"), null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(1993, 3, 3, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, "alicenguyen@example.com", "Alice", 2, null, false, new DateTimeOffset(new DateTime(2020, 3, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 7, 0, 0, 0)), null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Nguyen", 3, "Password@123", "SD0003", 2, "alicenguyen" },
                    { new Guid("44444444-4444-4444-4444-444444444444"), null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(1988, 4, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, "bobtran@example.com", "Bob", 1, null, false, new DateTimeOffset(new DateTime(2020, 4, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 7, 0, 0, 0)), null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Tran", 1, "Password@123", "SD0004", 2, "bobtran" },
                    { new Guid("55555555-5555-5555-5555-555555555555"), null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(1991, 5, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, "carolle@example.com", "Carol", 2, null, false, new DateTimeOffset(new DateTime(2020, 5, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 7, 0, 0, 0)), null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Le", 2, "Password@123", "SD0005", 2, "carolle" },
                    { new Guid("66666666-6666-6666-6666-666666666666"), null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(1987, 6, 6, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, "davidpham@example.com", "David", 1, null, false, new DateTimeOffset(new DateTime(2020, 6, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 7, 0, 0, 0)), null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Pham", 3, "Password@123", "SD0006", 2, "davidpham" },
                    { new Guid("77777777-7777-7777-7777-777777777777"), null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(1994, 7, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, "emilyvo@example.com", "Emily", 2, null, false, new DateTimeOffset(new DateTime(2020, 7, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 7, 0, 0, 0)), null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Vo", 1, "Password@123", "SD0007", 2, "emilyvo" },
                    { new Guid("88888888-8888-8888-8888-888888888888"), null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(1989, 8, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, "franknguyen@example.com", "Frank", 1, null, false, new DateTimeOffset(new DateTime(2020, 8, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 7, 0, 0, 0)), null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Nguyen", 2, "Password@123", "SD0008", 2, "franknguyen" },
                    { new Guid("99999999-9999-9999-9999-999999999999"), null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(1995, 9, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, "gracepham@example.com", "Grace", 2, null, false, new DateTimeOffset(new DateTime(2020, 9, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 7, 0, 0, 0)), null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Pham", 3, "Password@123", "SD0009", 2, "gracepham" },
                    { new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(1990, 10, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, "henryle@example.com", "Henry", 1, null, false, new DateTimeOffset(new DateTime(2020, 10, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 7, 0, 0, 0)), null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Le", 1, "Password@123", "SD0010", 2, "henryle" },
                    { new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(1996, 11, 11, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, "ivytran@example.com", "Ivy", 2, null, false, new DateTimeOffset(new DateTime(2020, 11, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 7, 0, 0, 0)), null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Tran", 2, "Password@123", "SD0011", 2, "ivytran" },
                    { new Guid("cccccccc-cccc-cccc-cccc-cccccccccccc"), null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(1986, 12, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, "jackvo@example.com", "Jack", 1, null, false, new DateTimeOffset(new DateTime(2020, 12, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 7, 0, 0, 0)), null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Vo", 3, "Password@123", "SD0012", 2, "jackvo" }
                });
        }
    }
}
