using AssetManagement.Domain.Entities;
using AssetManagement.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;

namespace AssetManagement.Data.Data
{
    [ExcludeFromCodeCoverage]
    public static class UserSeeder
    {
        public static void SeedUsers(this ModelBuilder modelBuilder)
        {
            var fixedCreatedDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = new Guid("11111111-1111-1111-1111-111111111111"),
                    StaffCode = "SD0001",
                    FirstName = "John",
                    LastName = "Doe",
                    Username = "johndoe",
                    Password = "Password@123",
                    IsPasswordUpdated = false,
                    DateOfBirth = new DateTime(1990, 1, 1),
                    JoinedDate = new DateTimeOffset(new DateTime(2020, 1, 10)),
                    Type = UserTypeEnum.Admin,
                    Location = LocationEnum.HCM,
                    Gender = GenderEnum.Male,
                    CreatedDate = fixedCreatedDate
                },
                new User
                {
                    Id = new Guid("22222222-2222-2222-2222-222222222222"),
                    StaffCode = "SD0002",
                    FirstName = "Jane",
                    LastName = "Smith",
                    Username = "janesmith",
                    Password = "Password@123",
                    IsPasswordUpdated = false,
                    DateOfBirth = new DateTime(1992, 2, 2),
                    JoinedDate = new DateTimeOffset(new DateTime(2020, 2, 15)),
                    Type = UserTypeEnum.Staff,
                    Location = LocationEnum.DN,
                    Gender = GenderEnum.Female,
                    CreatedDate = fixedCreatedDate
                },
                new User
                {
                    Id = new Guid("33333333-3333-3333-3333-333333333333"),
                    StaffCode = "SD0003",
                    FirstName = "Alice",
                    LastName = "Nguyen",
                    Username = "alicenguyen",
                    Password = "Password@123",
                    IsPasswordUpdated = false,
                    DateOfBirth = new DateTime(1993, 3, 3),
                    JoinedDate = new DateTimeOffset(new DateTime(2020, 3, 20)),
                    Type = UserTypeEnum.Staff,
                    Location = LocationEnum.HN,
                    Gender = GenderEnum.Female,
                    CreatedDate = fixedCreatedDate
                },
                new User
                {
                    Id = new Guid("44444444-4444-4444-4444-444444444444"),
                    StaffCode = "SD0004",
                    FirstName = "Bob",
                    LastName = "Tran",
                    Username = "bobtran",
                    Password = "Password@123",
                    IsPasswordUpdated = false,
                    DateOfBirth = new DateTime(1988, 4, 4),
                    JoinedDate = new DateTimeOffset(new DateTime(2020, 4, 25)),
                    Type = UserTypeEnum.Staff,
                    Location = LocationEnum.HCM,
                    Gender = GenderEnum.Male,
                    CreatedDate = fixedCreatedDate
                },
                new User
                {
                    Id = new Guid("55555555-5555-5555-5555-555555555555"),
                    StaffCode = "SD0005",
                    FirstName = "Carol",
                    LastName = "Le",
                    Username = "carolle",
                    Password = "Password@123",
                    IsPasswordUpdated = false,
                    DateOfBirth = new DateTime(1991, 5, 5),
                    JoinedDate = new DateTimeOffset(new DateTime(2020, 5, 30)),
                    Type = UserTypeEnum.Staff,
                    Location = LocationEnum.DN,
                    Gender = GenderEnum.Female,
                    CreatedDate = fixedCreatedDate
                },
                new User
                {
                    Id = new Guid("66666666-6666-6666-6666-666666666666"),
                    StaffCode = "SD0006",
                    FirstName = "David",
                    LastName = "Pham",
                    Username = "davidpham",
                    Password = "Password@123",
                    IsPasswordUpdated = false,
                    DateOfBirth = new DateTime(1987, 6, 6),
                    JoinedDate = new DateTimeOffset(new DateTime(2020, 6, 5)),
                    Type = UserTypeEnum.Staff,
                    Location = LocationEnum.HN,
                    Gender = GenderEnum.Male,
                    CreatedDate = fixedCreatedDate
                },
                new User
                {
                    Id = new Guid("77777777-7777-7777-7777-777777777777"),
                    StaffCode = "SD0007",
                    FirstName = "Emily",
                    LastName = "Vo",
                    Username = "emilyvo",
                    Password = "Password@123",
                    IsPasswordUpdated = false,
                    DateOfBirth = new DateTime(1994, 7, 7),
                    JoinedDate = new DateTimeOffset(new DateTime(2020, 7, 10)),
                    Type = UserTypeEnum.Staff,
                    Location = LocationEnum.HCM,
                    Gender = GenderEnum.Female,
                    CreatedDate = fixedCreatedDate
                },
                new User
                {
                    Id = new Guid("88888888-8888-8888-8888-888888888888"),
                    StaffCode = "SD0008",
                    FirstName = "Frank",
                    LastName = "Nguyen",
                    Username = "franknguyen",
                    Password = "Password@123",
                    IsPasswordUpdated = false,
                    DateOfBirth = new DateTime(1989, 8, 8),
                    JoinedDate = new DateTimeOffset(new DateTime(2020, 8, 15)),
                    Type = UserTypeEnum.Staff,
                    Location = LocationEnum.DN,
                    Gender = GenderEnum.Male,
                    CreatedDate = fixedCreatedDate
                },
                new User
                {
                    Id = new Guid("99999999-9999-9999-9999-999999999999"),
                    StaffCode = "SD0009",
                    FirstName = "Grace",
                    LastName = "Pham",
                    Username = "gracepham",
                    Password = "Password@123",
                    IsPasswordUpdated = false,
                    DateOfBirth = new DateTime(1995, 9, 9),
                    JoinedDate = new DateTimeOffset(new DateTime(2020, 9, 20)),
                    Type = UserTypeEnum.Staff,
                    Location = LocationEnum.HN,
                    Gender = GenderEnum.Female,
                    CreatedDate = fixedCreatedDate
                },
                new User
                {
                    Id = new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                    StaffCode = "SD0010",
                    FirstName = "Henry",
                    LastName = "Le",
                    Username = "henryle",
                    Password = "Password@123",
                    IsPasswordUpdated = false,
                    DateOfBirth = new DateTime(1990, 10, 10),
                    JoinedDate = new DateTimeOffset(new DateTime(2020, 10, 25)),
                    Type = UserTypeEnum.Staff,
                    Location = LocationEnum.HCM,
                    Gender = GenderEnum.Male,
                    CreatedDate = fixedCreatedDate
                },
                new User
                {
                    Id = new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
                    StaffCode = "SD0011",
                    FirstName = "Ivy",
                    LastName = "Tran",
                    Username = "ivytran",
                    Password = "Password@123",
                    IsPasswordUpdated = false,
                    DateOfBirth = new DateTime(1996, 11, 11),
                    JoinedDate = new DateTimeOffset(new DateTime(2020, 11, 30)),
                    Type = UserTypeEnum.Staff,
                    Location = LocationEnum.DN,
                    Gender = GenderEnum.Female,
                    CreatedDate = fixedCreatedDate
                },
                new User
                {
                    Id = new Guid("cccccccc-cccc-cccc-cccc-cccccccccccc"),
                    StaffCode = "SD0012",
                    FirstName = "Jack",
                    LastName = "Vo",
                    Username = "jackvo",
                    Password = "Password@123",
                    IsPasswordUpdated = false,
                    DateOfBirth = new DateTime(1986, 12, 12),
                    JoinedDate = new DateTimeOffset(new DateTime(2020, 12, 5)),
                    Type = UserTypeEnum.Staff,
                    Location = LocationEnum.HN,
                    Gender = GenderEnum.Male,
                    CreatedDate = fixedCreatedDate
                }
            );

        }
    }
}
