using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace AssetManagement.Data.Migrations
{
    /// <inheritdoc />
    [ExcludeFromCodeCoverage]
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Assets",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModificatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LastModificatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeletedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Assets", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StaffCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Username = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsPasswordUpdated = table.Column<bool>(type: "bit", nullable: false),
                    DateOfBirth = table.Column<DateTime>(type: "datetime2", nullable: true),
                    JoinedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Location = table.Column<int>(type: "int", nullable: false),
                    Gender = table.Column<int>(type: "int", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModificatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LastModificatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeletedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Assets");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
