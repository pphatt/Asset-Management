using AssetManagement.Domain.Entities;
using AssetManagement.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace AssetManagement.Data.Data
{
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
                    Email = "johndoe@example.com",
                    Password = "Password@123",
                    IsPasswordUpdated = false,
                    DateOfBirth = new DateTime(1990, 1, 1),
                    JoinedDate = new DateTimeOffset(new DateTime(2020, 1, 10)),
                    Type = UserTypeEnum.Admin,
                    Location = LocationEnum.HCM,
                    Gender = GenderEnum.Male,
                    CreatedDate = fixedCreatedDate
                }
            );
        }
    }
}
