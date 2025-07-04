﻿using AssetManagement.Domain.Entities;
using AssetManagement.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;

namespace AssetManagement.Data.Extensions
{
    [ExcludeFromCodeCoverage]
    public static class ModelBuilderExtensions
    {
        public static void SeedUsers(this ModelBuilder modelBuilder)
        {
            var fixedCreatedDate = new DateTime(2025, 5, 1, 0, 0, 0, DateTimeKind.Utc);

            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = new Guid("c87d8f20-c381-4970-b367-09f2cf95d655"),
                    StaffCode = "SD0001",
                    FirstName = "Nghia",
                    LastName = "Dinh",
                    Username = "nghiadinh",
                    Password = "Xq4XCC8U9vadB+YSKnXCeOL45zqs4xg3TbdQoCi/5uUkzqIfBeayAIxn7sxp7XSK",
                    IsPasswordUpdated = true,
                    DateOfBirth = new DateTime(1989, 1, 1),
                    JoinedDate = new DateTimeOffset(new DateTime(2016, 1, 10)),
                    Type = UserType.Admin,
                    Location = Location.HCM,
                    Gender = Gender.Male,
                    IsActive = true,
                    CreatedDate = fixedCreatedDate
                },
                new User
                {
                    Id = new Guid("1f2b0a56-3c8e-4771-b227-69e2cd5e41a3"),
                    StaffCode = "SD0002",
                    FirstName = "Minh",
                    LastName = "Nguyen",
                    Username = "minhnguyen",
                    Password = "Xq4XCC8U9vadB+YSKnXCeOL45zqs4xg3TbdQoCi/5uUkzqIfBeayAIxn7sxp7XSK",
                    IsPasswordUpdated = true,
                    DateOfBirth = new DateTime(1992, 5, 15),
                    JoinedDate = new DateTimeOffset(new DateTime(2021, 3, 20)),
                    Type = UserType.Admin,
                    Location = Location.DN,
                    Gender = Gender.Male,
                    IsActive = true,
                    CreatedDate = fixedCreatedDate
                }
            );
        }
    }
}
