using AssetManagement.Data.Helpers.Hashing;
using AssetManagement.Domain.Entities;
using AssetManagement.Domain.Enums;
using Microsoft.Extensions.Logging;
using System.Diagnostics.CodeAnalysis;

namespace AssetManagement.Data.Helpers.Seeding
{
    [ExcludeFromCodeCoverage]
    public class DataSeeder
    {
        private readonly AssetManagementDbContext _dbContext;
        private readonly IPasswordHasher _passwordHasher;
        private readonly ILogger<DataSeeder> _logger;

        public DataSeeder(AssetManagementDbContext dbContext, IPasswordHasher passwordHasher, ILogger<DataSeeder> logger)
        {
            _dbContext = dbContext;
            _passwordHasher = passwordHasher;
            _logger = logger;
        }

        public async Task SeedAsync()
        {
            try
            {
                await SeedUsersAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while seeding the database.");
                throw;
            }
        }

        private async Task SeedUsersAsync()
        {
            // Skip if users already exist
            if (_dbContext.Users.Any())
            {
                _logger.LogInformation("Users already seeded.");
                return;
            }

            _logger.LogInformation("Seeding users...");

            var fixedCreatedDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            var adminUsers = new List<User>
        {
            new User
            {
                Id = new Guid("11111111-1111-1111-1111-111111111111"),
                StaffCode = "SD0001",
                FirstName = "John",
                LastName = "Doe",
                Username = "johndoe",
                Password = _passwordHasher.HashPassword("Password@123"),
                IsPasswordUpdated = false,
                DateOfBirth = new DateTime(1990, 1, 1),
                JoinedDate = new DateTimeOffset(new DateTime(2020, 1, 10)),
                Type = UserType.Admin,
                Location = Location.HCM,
                Gender = Gender.Male,
                IsActive = true,
                CreatedDate = fixedCreatedDate
            },
            new User
            {
                Id = new Guid("22222222-2222-2222-2222-222222222222"),
                StaffCode = "SD0002",
                FirstName = "Jane",
                LastName = "Smith",
                Username = "janesmith",
                Password = _passwordHasher.HashPassword("Password@123"),
                IsPasswordUpdated = false,
                DateOfBirth = new DateTime(1992, 5, 15),
                JoinedDate = new DateTimeOffset(new DateTime(2020, 2, 15)),
                Type = UserType.Admin,
                Location = Location.HN,
                Gender = Gender.Female,
                IsActive = true,
                CreatedDate = fixedCreatedDate
            }
        };

            var staffUsers = new List<User>
        {
            // HCM Staff
            new User
            {
                Id = new Guid("33333333-3333-3333-3333-333333333333"),
                StaffCode = "SD0003",
                FirstName = "Robert",
                LastName = "Johnson",
                Username = "robertj",
                Password = _passwordHasher.HashPassword("Password@123"),
                IsPasswordUpdated = false,
                DateOfBirth = new DateTime(1995, 8, 20),
                JoinedDate = new DateTimeOffset(new DateTime(2021, 4, 10)),
                Type = UserType.Staff,
                Location = Location.HCM,
                Gender = Gender.Male,
                IsActive = true,
                CreatedDate = fixedCreatedDate
            },
            new User
            {
                Id = new Guid("44444444-4444-4444-4444-444444444444"),
                StaffCode = "SD0004",
                FirstName = "Sarah",
                LastName = "Lee",
                Username = "sarahlee",
                Password = _passwordHasher.HashPassword("Password@123"),
                IsPasswordUpdated = false,
                DateOfBirth = new DateTime(1993, 3, 15),
                JoinedDate = new DateTimeOffset(new DateTime(2021, 5, 12)),
                Type = UserType.Staff,
                Location = Location.HCM,
                Gender = Gender.Female,
                IsActive = true,
                CreatedDate = fixedCreatedDate
            },
            
            // HN Staff
            new User
            {
                Id = new Guid("55555555-5555-5555-5555-555555555555"),
                StaffCode = "SD0005",
                FirstName = "Michael",
                LastName = "Chen",
                Username = "michaelc",
                Password = _passwordHasher.HashPassword("Password@123"),
                IsPasswordUpdated = false,
                DateOfBirth = new DateTime(1991, 11, 8),
                JoinedDate = new DateTimeOffset(new DateTime(2022, 1, 15)),
                Type = UserType.Staff,
                Location = Location.HN,
                Gender = Gender.Male,
                IsActive = true,
                CreatedDate = fixedCreatedDate
            },

            // Disabled user
            new User
            {
                Id = new Guid("66666666-6666-6666-6666-666666666666"),
                StaffCode = "SD0006",
                FirstName = "David",
                LastName = "Wilson",
                Username = "davidw",
                Password = _passwordHasher.HashPassword("Password@123"),
                IsPasswordUpdated = false,
                DateOfBirth = new DateTime(1988, 7, 22),
                JoinedDate = new DateTimeOffset(new DateTime(2020, 8, 15)),
                Type = UserType.Staff,
                Location = Location.HN,
                Gender = Gender.Male,
                IsActive = false,
                CreatedDate = fixedCreatedDate
            }
        };

            // Add all users to context
            await _dbContext.Users.AddRangeAsync(adminUsers);
            await _dbContext.Users.AddRangeAsync(staffUsers);

            // Save changes
            await _dbContext.SaveChangesAsync();

            _logger.LogInformation("Successfully seeded {AdminCount} admin users and {StaffCount} staff users", adminUsers.Count, staffUsers.Count);
        }
    }
}