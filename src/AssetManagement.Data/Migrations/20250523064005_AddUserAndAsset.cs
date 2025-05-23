using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace AssetManagement.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddUserAndAsset : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Slug = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeletedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
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
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsPasswordUpdated = table.Column<bool>(type: "bit", nullable: false),
                    DateOfBirth = table.Column<DateTime>(type: "datetime2", nullable: true),
                    JoinedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Location = table.Column<int>(type: "int", nullable: false),
                    Gender = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeletedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Assets",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AssetCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    State = table.Column<int>(type: "int", nullable: false),
                    InstallDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    Location = table.Column<int>(type: "int", nullable: false),
                    Specification = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CategoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeletedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Assets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Assets_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "CreatedBy", "CreatedDate", "DeletedBy", "DeletedDate", "IsDeleted", "LastModifiedBy", "LastModifiedDate", "Name", "Slug" },
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
                table: "Users",
                columns: new[] { "Id", "CreatedBy", "CreatedDate", "DateOfBirth", "DeletedBy", "DeletedDate", "FirstName", "Gender", "IsActive", "IsDeleted", "IsPasswordUpdated", "JoinedDate", "LastModifiedBy", "LastModifiedDate", "LastName", "Location", "Password", "StaffCode", "Type", "Username" },
                values: new object[,]
                {
                    { new Guid("00b68283-d752-4fff-be4c-a0031a787876"), null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(1993, 7, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, "Hieu", 1, true, null, true, new DateTimeOffset(new DateTime(2018, 11, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 7, 0, 0, 0)), null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Pham", 1, "UG9KspzI+vFB6XbUYNTk5THtu1iPQruEIU0jEPFNrlzrBs/kgX/Ol0E/HXyjJrcV", "SD0013", 2, "hieupham" },
                    { new Guid("024e81ce-ef22-458b-bf29-e6b3bfec2fb8"), null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(1993, 2, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, "Ha", 2, true, null, true, new DateTimeOffset(new DateTime(2020, 11, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 7, 0, 0, 0)), null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Nguyen", 2, "UG9KspzI+vFB6XbUYNTk5THtu1iPQruEIU0jEPFNrlzrBs/kgX/Ol0E/HXyjJrcV", "SD0017", 2, "hanguyen" },
                    { new Guid("110fc2ec-7d7d-4955-9505-0d962e0c1deb"), null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(1990, 8, 23, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, "Minh", 1, true, null, true, new DateTimeOffset(new DateTime(2021, 7, 3, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 7, 0, 0, 0)), null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Le", 2, "UG9KspzI+vFB6XbUYNTk5THtu1iPQruEIU0jEPFNrlzrBs/kgX/Ol0E/HXyjJrcV", "SD0018", 2, "minhle" },
                    { new Guid("18c5c4c2-e04a-4e9f-8c83-370e5a03ca1c"), null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(1995, 6, 19, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, "Lan", 2, false, null, true, new DateTimeOffset(new DateTime(2023, 3, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 7, 0, 0, 0)), null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Bui", 2, "UG9KspzI+vFB6XbUYNTk5THtu1iPQruEIU0jEPFNrlzrBs/kgX/Ol0E/HXyjJrcV", "SD0029", 2, "lanbui" },
                    { new Guid("1f2b0a56-3c8e-4771-b227-69e2cd5e41a3"), null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(1992, 5, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, "Minh", 1, true, null, true, new DateTimeOffset(new DateTime(2021, 3, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 7, 0, 0, 0)), null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Nguyen", 2, "SXiTq48SSiVAhIjU3TW8PHbHJ2K2geU8aoV10m9y643uUGO1pI/m7s0d5pciA0bd", "SD0002", 1, "minhnguyen" },
                    { new Guid("2246b919-73f8-44f2-bcc2-80149e40a796"), null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(1995, 12, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, "Trang", 2, true, null, true, new DateTimeOffset(new DateTime(2021, 6, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 7, 0, 0, 0)), null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Do", 1, "UG9KspzI+vFB6XbUYNTk5THtu1iPQruEIU0jEPFNrlzrBs/kgX/Ol0E/HXyjJrcV", "SD0010", 2, "trangdo" },
                    { new Guid("2b1dc1f3-945e-444d-b68d-52e59d0e699c"), null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(1994, 4, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, "Linh", 2, true, null, true, new DateTimeOffset(new DateTime(2022, 5, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 7, 0, 0, 0)), null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Pham", 1, "UG9KspzI+vFB6XbUYNTk5THtu1iPQruEIU0jEPFNrlzrBs/kgX/Ol0E/HXyjJrcV", "SD0008", 2, "linhpham" },
                    { new Guid("2b6d4d81-7305-4cd3-9104-760aa2d9b80a"), null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(1994, 9, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, "Linh", 2, true, null, true, new DateTimeOffset(new DateTime(2022, 2, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 7, 0, 0, 0)), null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Tran", 1, "UG9KspzI+vFB6XbUYNTk5THtu1iPQruEIU0jEPFNrlzrBs/kgX/Ol0E/HXyjJrcV", "SD0003", 2, "linhtran" },
                    { new Guid("3c57b69c-d9a0-4678-b94f-ad4533be6e81"), null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(1991, 11, 23, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, "Tuan", 1, true, null, true, new DateTimeOffset(new DateTime(2021, 7, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 7, 0, 0, 0)), null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Pham", 3, "UG9KspzI+vFB6XbUYNTk5THtu1iPQruEIU0jEPFNrlzrBs/kgX/Ol0E/HXyjJrcV", "SD0004", 2, "tuanpham" },
                    { new Guid("43ea65e7-4878-45dc-b730-47c3b2dba729"), null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(1993, 9, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, "Yen", 2, false, null, true, new DateTimeOffset(new DateTime(2022, 1, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 7, 0, 0, 0)), null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Lam", 2, "UG9KspzI+vFB6XbUYNTk5THtu1iPQruEIU0jEPFNrlzrBs/kgX/Ol0E/HXyjJrcV", "SD0027", 2, "yenlam" },
                    { new Guid("43fcafa3-be42-4100-91d2-6390b1a81780"), null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(1991, 1, 29, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, "Tram", 2, false, null, true, new DateTimeOffset(new DateTime(2020, 10, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 7, 0, 0, 0)), null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Ngo", 2, "UG9KspzI+vFB6XbUYNTk5THtu1iPQruEIU0jEPFNrlzrBs/kgX/Ol0E/HXyjJrcV", "SD0025", 2, "tramngo" },
                    { new Guid("480826b4-b3f7-4838-b830-1a673a508fdb"), null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(1994, 11, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, "Long", 1, true, null, true, new DateTimeOffset(new DateTime(2023, 2, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 7, 0, 0, 0)), null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Dang", 2, "UG9KspzI+vFB6XbUYNTk5THtu1iPQruEIU0jEPFNrlzrBs/kgX/Ol0E/HXyjJrcV", "SD0020", 2, "longdang" },
                    { new Guid("4a7ee807-4881-4b19-9fa5-4caefa6ef653"), null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(1989, 3, 14, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, "Dung", 1, true, null, true, new DateTimeOffset(new DateTime(2022, 6, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 7, 0, 0, 0)), null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Ly", 2, "UG9KspzI+vFB6XbUYNTk5THtu1iPQruEIU0jEPFNrlzrBs/kgX/Ol0E/HXyjJrcV", "SD0022", 2, "dungly" },
                    { new Guid("4f38d797-4168-4c55-8884-76f1f2d3f79b"), null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(1993, 3, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, "Hoa", 2, false, null, true, new DateTimeOffset(new DateTime(2020, 9, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 7, 0, 0, 0)), null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Le", 1, "UG9KspzI+vFB6XbUYNTk5THtu1iPQruEIU0jEPFNrlzrBs/kgX/Ol0E/HXyjJrcV", "SD0005", 2, "hoale" },
                    { new Guid("4f8327a0-c573-4e28-bdce-c05b28aaab07"), null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(1991, 6, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, "Quynh", 2, true, null, true, new DateTimeOffset(new DateTime(2020, 1, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 7, 0, 0, 0)), null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Pham", 2, "UG9KspzI+vFB6XbUYNTk5THtu1iPQruEIU0jEPFNrlzrBs/kgX/Ol0E/HXyjJrcV", "SD0021", 2, "quynhpham" },
                    { new Guid("56d73f5a-91be-4552-b356-dab3367315f1"), null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(1992, 9, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, "Thao", 2, true, null, true, new DateTimeOffset(new DateTime(2020, 5, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 7, 0, 0, 0)), null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Phan", 2, "UG9KspzI+vFB6XbUYNTk5THtu1iPQruEIU0jEPFNrlzrBs/kgX/Ol0E/HXyjJrcV", "SD0019", 2, "thaophan" },
                    { new Guid("6a527ed0-cf45-4076-b930-d0183e274cc4"), null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(1990, 4, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, "Vinh", 1, false, null, true, new DateTimeOffset(new DateTime(2018, 5, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 7, 0, 0, 0)), null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Dinh", 2, "UG9KspzI+vFB6XbUYNTk5THtu1iPQruEIU0jEPFNrlzrBs/kgX/Ol0E/HXyjJrcV", "SD0028", 2, "vinhdinh" },
                    { new Guid("6abd3850-93f5-458a-94ed-4ccd3cb903df"), null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(1990, 5, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, "An", 1, true, null, true, new DateTimeOffset(new DateTime(2021, 1, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 7, 0, 0, 0)), null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Nguyen", 1, "UG9KspzI+vFB6XbUYNTk5THtu1iPQruEIU0jEPFNrlzrBs/kgX/Ol0E/HXyjJrcV", "SD0006", 2, "annguyen" },
                    { new Guid("88108306-c04f-4ab4-be0b-2ed7a813043d"), null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(1989, 9, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, "Tuan", 1, true, null, true, new DateTimeOffset(new DateTime(2019, 12, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 7, 0, 0, 0)), null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Le", 1, "UG9KspzI+vFB6XbUYNTk5THtu1iPQruEIU0jEPFNrlzrBs/kgX/Ol0E/HXyjJrcV", "SD0009", 2, "tuanle" },
                    { new Guid("91a21848-2258-4cff-b6a8-31e75ec69ed0"), null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(1991, 11, 23, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, "Khanh", 1, true, null, true, new DateTimeOffset(new DateTime(2020, 8, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 7, 0, 0, 0)), null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Vo", 1, "UG9KspzI+vFB6XbUYNTk5THtu1iPQruEIU0jEPFNrlzrBs/kgX/Ol0E/HXyjJrcV", "SD0011", 2, "khanhvo" },
                    { new Guid("9b05c83e-410f-4ca6-aa7b-98c1bc63b6f2"), null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(1992, 5, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, "Khoi", 1, false, null, true, new DateTimeOffset(new DateTime(2019, 6, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 7, 0, 0, 0)), null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Phan", 2, "UG9KspzI+vFB6XbUYNTk5THtu1iPQruEIU0jEPFNrlzrBs/kgX/Ol0E/HXyjJrcV", "SD0026", 2, "khoiphan" },
                    { new Guid("a2786e5b-9b10-4c24-92a0-915d686d1086"), null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(1988, 12, 27, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, "Tien", 1, false, null, true, new DateTimeOffset(new DateTime(2016, 12, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 7, 0, 0, 0)), null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Vu", 2, "UG9KspzI+vFB6XbUYNTk5THtu1iPQruEIU0jEPFNrlzrBs/kgX/Ol0E/HXyjJrcV", "SD0030", 2, "tienvu" },
                    { new Guid("aae93aa5-632c-4101-ac5a-ae3bf7c1b031"), null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(1995, 7, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, "Huong", 2, true, null, true, new DateTimeOffset(new DateTime(2023, 9, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 7, 0, 0, 0)), null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Do", 2, "UG9KspzI+vFB6XbUYNTk5THtu1iPQruEIU0jEPFNrlzrBs/kgX/Ol0E/HXyjJrcV", "SD0023", 2, "huongdo" },
                    { new Guid("c87d8f20-c381-4970-b367-09f2cf95d655"), null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(1990, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, "Nghia", 1, true, null, true, new DateTimeOffset(new DateTime(2020, 1, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 7, 0, 0, 0)), null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Dinh", 1, "SXiTq48SSiVAhIjU3TW8PHbHJ2K2geU8aoV10m9y643uUGO1pI/m7s0d5pciA0bd", "SD0001", 1, "nghiadinh" },
                    { new Guid("d32543f8-1346-4c0d-af20-17edf8149d75"), null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(1990, 10, 11, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, "Phong", 1, true, null, true, new DateTimeOffset(new DateTime(2021, 8, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 7, 0, 0, 0)), null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Hoang", 2, "UG9KspzI+vFB6XbUYNTk5THtu1iPQruEIU0jEPFNrlzrBs/kgX/Ol0E/HXyjJrcV", "SD0024", 2, "phonghoang" },
                    { new Guid("d445a21d-93a2-4d4e-b1e4-421717680965"), null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(1992, 8, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, "Minh", 1, true, null, true, new DateTimeOffset(new DateTime(2020, 3, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 7, 0, 0, 0)), null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Tran", 1, "UG9KspzI+vFB6XbUYNTk5THtu1iPQruEIU0jEPFNrlzrBs/kgX/Ol0E/HXyjJrcV", "SD0007", 2, "minhtran" },
                    { new Guid("dbf2b2c5-52a7-46ea-808d-e29538529105"), null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(1996, 2, 14, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, "Thao", 2, true, null, true, new DateTimeOffset(new DateTime(2023, 2, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 7, 0, 0, 0)), null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Nguyen", 1, "UG9KspzI+vFB6XbUYNTk5THtu1iPQruEIU0jEPFNrlzrBs/kgX/Ol0E/HXyjJrcV", "SD0012", 2, "thaonguyen" },
                    { new Guid("edaa920f-f98e-40a8-88ab-56ffe9e99093"), null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(1990, 10, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, "Duy", 1, false, null, true, new DateTimeOffset(new DateTime(2016, 7, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 7, 0, 0, 0)), null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Hoang", 1, "UG9KspzI+vFB6XbUYNTk5THtu1iPQruEIU0jEPFNrlzrBs/kgX/Ol0E/HXyjJrcV", "SD0015", 2, "duyhoang" },
                    { new Guid("edf1b40d-c5e2-410b-859e-ec29a5d95292"), null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(1991, 4, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, "Binh", 1, true, null, true, new DateTimeOffset(new DateTime(2022, 3, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 7, 0, 0, 0)), null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Tran", 2, "UG9KspzI+vFB6XbUYNTk5THtu1iPQruEIU0jEPFNrlzrBs/kgX/Ol0E/HXyjJrcV", "SD0016", 2, "binhtran" },
                    { new Guid("f74f2293-213d-41f3-b146-7dd4acb5499c"), null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(1988, 6, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, "Lan", 2, false, null, true, new DateTimeOffset(new DateTime(2017, 4, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 7, 0, 0, 0)), null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Nguyen", 1, "UG9KspzI+vFB6XbUYNTk5THtu1iPQruEIU0jEPFNrlzrBs/kgX/Ol0E/HXyjJrcV", "SD0014", 2, "lannguyen" }
                });

            migrationBuilder.InsertData(
                table: "Assets",
                columns: new[] { "Id", "AssetCode", "CategoryId", "CreatedBy", "CreatedDate", "DeletedBy", "DeletedDate", "InstallDate", "IsDeleted", "LastModifiedBy", "LastModifiedDate", "Location", "Name", "Specification", "State" },
                values: new object[,]
                {
                    { new Guid("223e4567-e89b-12d3-a456-426614174001"), "LAP-001", new Guid("123e4567-e89b-12d3-a456-426614174005"), null, new DateTime(2025, 5, 21, 0, 0, 0, 0, DateTimeKind.Utc), null, null, new DateTimeOffset(new DateTime(2023, 5, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, "Dell XPS 13", "Intel i7, 16GB RAM, 512GB SSD", 1 },
                    { new Guid("223e4567-e89b-12d3-a456-426614174002"), "LAP-002", new Guid("123e4567-e89b-12d3-a456-426614174005"), null, new DateTime(2025, 5, 21, 0, 0, 0, 0, DateTimeKind.Utc), null, null, new DateTimeOffset(new DateTime(2024, 5, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, "MacBook Pro 14", "M1 Pro, 32GB RAM, 1TB SSD", 1 },
                    { new Guid("223e4567-e89b-12d3-a456-426614174003"), "MON-001", new Guid("123e4567-e89b-12d3-a456-426614174007"), null, new DateTime(2025, 5, 21, 0, 0, 0, 0, DateTimeKind.Utc), null, null, new DateTimeOffset(new DateTime(2022, 5, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, "Dell UltraSharp 27", "27-inch, 4K, IPS Panel", 1 },
                    { new Guid("223e4567-e89b-12d3-a456-426614174004"), "MON-002", new Guid("123e4567-e89b-12d3-a456-426614174007"), null, new DateTime(2025, 5, 21, 0, 0, 0, 0, DateTimeKind.Utc), null, null, new DateTimeOffset(new DateTime(2023, 5, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, "LG 32GN600", "32-inch, QHD, 144Hz", 1 },
                    { new Guid("223e4567-e89b-12d3-a456-426614174005"), "IPD-001", new Guid("123e4567-e89b-12d3-a456-426614174003"), null, new DateTime(2025, 5, 21, 0, 0, 0, 0, DateTimeKind.Utc), null, null, new DateTimeOffset(new DateTime(2023, 11, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, "iPad Pro 12.9", "M2 Chip, 256GB, Wi-Fi", 1 },
                    { new Guid("223e4567-e89b-12d3-a456-426614174006"), "IPD-002", new Guid("123e4567-e89b-12d3-a456-426614174003"), null, new DateTime(2025, 5, 21, 0, 0, 0, 0, DateTimeKind.Utc), null, null, new DateTimeOffset(new DateTime(2024, 11, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 3, "iPad Air 5", "M1 Chip, 64GB, Wi-Fi", 1 },
                    { new Guid("223e4567-e89b-12d3-a456-426614174007"), "IPH-001", new Guid("123e4567-e89b-12d3-a456-426614174004"), null, new DateTime(2025, 5, 21, 0, 0, 0, 0, DateTimeKind.Utc), null, null, new DateTimeOffset(new DateTime(2024, 5, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, "iPhone 14 Pro", "256GB, Black, 5G", 1 },
                    { new Guid("223e4567-e89b-12d3-a456-426614174008"), "MOU-001", new Guid("123e4567-e89b-12d3-a456-426614174001"), null, new DateTime(2025, 5, 21, 0, 0, 0, 0, DateTimeKind.Utc), null, null, new DateTimeOffset(new DateTime(2024, 5, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, "Logitech MX Master 3", "Wireless, 4000 DPI, Ergonomic", 1 },
                    { new Guid("223e4567-e89b-12d3-a456-426614174009"), "HST-001", new Guid("123e4567-e89b-12d3-a456-426614174002"), null, new DateTime(2025, 5, 21, 0, 0, 0, 0, DateTimeKind.Utc), null, null, new DateTimeOffset(new DateTime(2024, 9, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, "Sony WH-1000XM5", "Wireless, Noise-Cancelling, 30hr Battery", 1 },
                    { new Guid("223e4567-e89b-12d3-a456-426614174010"), "PC-001", new Guid("123e4567-e89b-12d3-a456-426614174008"), null, new DateTime(2025, 5, 21, 0, 0, 0, 0, DateTimeKind.Utc), null, null, new DateTimeOffset(new DateTime(2020, 5, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 3, "HP EliteDesk 800", "Intel i5, 8GB RAM, 1TB HDD", 1 },
                    { new Guid("223e4567-e89b-12d3-a456-426614174011"), "TAB-001", new Guid("123e4567-e89b-12d3-a456-426614174009"), null, new DateTime(2025, 5, 21, 0, 0, 0, 0, DateTimeKind.Utc), null, null, new DateTimeOffset(new DateTime(2024, 7, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, "Samsung Galaxy Tab S8", "Snapdragon 8, 128GB, 11-inch", 1 },
                    { new Guid("223e4567-e89b-12d3-a456-426614174012"), "MOB-001", new Guid("123e4567-e89b-12d3-a456-426614174006"), null, new DateTime(2025, 5, 21, 0, 0, 0, 0, DateTimeKind.Utc), null, null, new DateTimeOffset(new DateTime(2025, 1, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 3, "Samsung Galaxy S23", "256GB, 5G, Snapdragon 8 Gen 2", 1 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Assets_CategoryId",
                table: "Assets",
                column: "CategoryId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Assets");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Categories");
        }
    }
}
