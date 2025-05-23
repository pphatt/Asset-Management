using AssetManagement.Domain.Entities;
using AssetManagement.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AssetManagement.Data.Configuration;

public class UserConfiguration: IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        //var fixedCreatedDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        //builder.HasData(
        //    new User
        //    {
        //        Id = new Guid("c87d8f20-c381-4970-b367-09f2cf95d655"),
        //        StaffCode = "SD0001",
        //        FirstName = "Nghia",
        //        LastName = "Dinh",
        //        Username = "nghiadinh",
        //        Password = "g7/Bvucy8BU+fYIA9ibE0gACIvZ6mPvm7Jf8EBXicQUgjeTEUvSyt1KrLXUPZmGW",
        //        IsPasswordUpdated = false,
        //        DateOfBirth = new DateTime(1990, 1, 1),
        //        JoinedDate = new DateTimeOffset(new DateTime(2020, 1, 10)),
        //        Type = UserTypeEnum.Admin,
        //        Location = LocationEnum.HCM,
        //        Gender = GenderEnum.Male,
        //        IsActive = true,
        //        CreatedDate = fixedCreatedDate
        //    },
        //    new User
        //    {
        //        Id = new Guid("1f2b0a56-3c8e-4771-b227-69e2cd5e41a3"),
        //        StaffCode = "SD0002",
        //        FirstName = "Minh",
        //        LastName = "Nguyen",
        //        Username = "minhnguyen",
        //        Password = "g7/Bvucy8BU+fYIA9ibE0gACIvZ6mPvm7Jf8EBXicQUgjeTEUvSyt1KrLXUPZmGW",
        //        IsPasswordUpdated = false,
        //        DateOfBirth = new DateTime(1992, 5, 15),
        //        JoinedDate = new DateTimeOffset(new DateTime(2021, 3, 20)),
        //        Type = UserTypeEnum.Staff,
        //        Location = LocationEnum.HCM,
        //        Gender = GenderEnum.Male,
        //        IsActive = true,
        //        CreatedDate = fixedCreatedDate
        //    },
        //    new User
        //    {
        //        Id = new Guid("2b6d4d81-7305-4cd3-9104-760aa2d9b80a"),
        //        StaffCode = "SD0003",
        //        FirstName = "Linh",
        //        LastName = "Tran",
        //        Username = "linhtran",
        //        Password = "g7/Bvucy8BU+fYIA9ibE0gACIvZ6mPvm7Jf8EBXicQUgjeTEUvSyt1KrLXUPZmGW",
        //        IsPasswordUpdated = false,
        //        DateOfBirth = new DateTime(1994, 9, 8),
        //        JoinedDate = new DateTimeOffset(new DateTime(2022, 2, 15)),
        //        Type = UserTypeEnum.Staff,
        //        Location = LocationEnum.HCM,
        //        Gender = GenderEnum.Female,
        //        IsActive = true,
        //        CreatedDate = fixedCreatedDate
        //    },
        //     new User
        //     {
        //         Id = new Guid("3c57b69c-d9a0-4678-b94f-ad4533be6e81"),
        //         StaffCode = "SD0004",
        //         FirstName = "Tuan",
        //         LastName = "Pham",
        //         Username = "tuanpham",
        //         Password = "g7/Bvucy8BU+fYIA9ibE0gACIvZ6mPvm7Jf8EBXicQUgjeTEUvSyt1KrLXUPZmGW",
        //         IsPasswordUpdated = false,
        //         DateOfBirth = new DateTime(1991, 11, 23),
        //         JoinedDate = new DateTimeOffset(new DateTime(2021, 7, 5)),
        //         Type = UserTypeEnum.Staff,
        //         Location = LocationEnum.HN,
        //         Gender = GenderEnum.Male,
        //         IsActive = true,
        //         CreatedDate = fixedCreatedDate
        //     },
        //     new User
        //     {
        //         Id = new Guid("4f38d797-4168-4c55-8884-76f1f2d3f79b"),
        //         StaffCode = "SD0005",
        //         FirstName = "Hoa",
        //         LastName = "Le",
        //         Username = "hoale",
        //         Password = "g7/Bvucy8BU+fYIA9ibE0gACIvZ6mPvm7Jf8EBXicQUgjeTEUvSyt1KrLXUPZmGW",
        //         IsPasswordUpdated = false,
        //         DateOfBirth = new DateTime(1993, 3, 18),
        //         JoinedDate = new DateTimeOffset(new DateTime(2020, 9, 12)),
        //         Type = UserTypeEnum.Staff,
        //         Location = LocationEnum.HCM,
        //         Gender = GenderEnum.Female,
        //         IsActive = false,
        //         CreatedDate = fixedCreatedDate
        //     }
        //);
    }
}