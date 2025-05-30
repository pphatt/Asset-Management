using AssetManagement.Data.Helpers.Hashing;
using AssetManagement.Domain.Entities;
using AssetManagement.Domain.Enums;
using Microsoft.EntityFrameworkCore;
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
        private readonly DateTime fixedCreatedDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);

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
                await SeedCategoriesAsync();
                await SeedAssetsAsync();
                await SeedAssignmentsAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while seeding the database.");
                throw;
            }
        }

        // Seed Users
        private async Task SeedUsersAsync()
        {
            // Skip if users already exist
            if (_dbContext.Users.Any())
            {
                _logger.LogInformation("Users already seeded.");
                return;
            }

            _logger.LogInformation("Seeding users...");

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

        // Seed Categories
        private async Task SeedCategoriesAsync()
        {
            if (_dbContext.Categories.Any())
            {
                _logger.LogInformation("Categories already seeded.");
                return;
            }

            _logger.LogInformation("Seeding categories...");

            var categories = new List<Category>
            {
                new Category
                {
                    Id = Guid.NewGuid(),
                    Name = "Laptop",
                    Prefix = "LT",
                    CreatedDate = fixedCreatedDate
                },
                new Category
                {
                    Id = Guid.NewGuid(),
                    Name = "Monitor",
                    Prefix = "MN",
                    CreatedDate = fixedCreatedDate
                }
            };

            await _dbContext.Categories.AddRangeAsync(categories);
            await _dbContext.SaveChangesAsync();

            _logger.LogInformation("Seeded {Count} categories", categories.Count);
        }

        // Seed Assets
        private async Task SeedAssetsAsync()
        {
            if (_dbContext.Assets.Any())
            {
                _logger.LogInformation("Assets already seeded.");
                return;
            }

            _logger.LogInformation("Seeding assets...");

            var categories = await _dbContext.Categories.ToListAsync();
            if (!categories.Any())
            {
                throw new Exception("Categories must be seeded before assets.");
            }

            var laptopCategory = categories.First(c => c.Name == "Laptop");
            var monitorCategory = categories.First(c => c.Name == "Monitor");

            var assets = new List<Asset>
            {
                // HCM Assets
                new Asset
                {
                    Id = Guid.NewGuid(),
                    Code = "LAP001",
                    Name = "Laptop 1",
                    State = AssetState.Available,
                    InstalledDate = new DateTimeOffset(new DateTime(2023, 1, 1)),
                    Location = Location.HCM,
                    Specification = "Spec for Laptop 1",
                    CategoryId = laptopCategory.Id,
                    CreatedDate = fixedCreatedDate
                },
                new Asset
                {
                    Id = Guid.NewGuid(),
                    Code = "LAP002",
                    Name = "Laptop 2",
                    State = AssetState.Available,
                    InstalledDate = new DateTimeOffset(new DateTime(2023, 2, 1)),
                    Location = Location.HCM,
                    Specification = "Spec for Laptop 2",
                    CategoryId = laptopCategory.Id,
                    CreatedDate = fixedCreatedDate
                },
                new Asset
                {
                    Id = Guid.NewGuid(),
                    Code = "MON001",
                    Name = "Monitor 1",
                    State = AssetState.Available,
                    InstalledDate = new DateTimeOffset(new DateTime(2023, 3, 1)),
                    Location = Location.HCM,
                    Specification = "Spec for Monitor 1",
                    CategoryId = monitorCategory.Id,
                    CreatedDate = fixedCreatedDate
                },
                // HN Assets
                new Asset
                {
                    Id = Guid.NewGuid(),
                    Code = "LAP003",
                    Name = "Laptop 3",
                    State = AssetState.Available,
                    InstalledDate = new DateTimeOffset(new DateTime(2023, 4, 1)),
                    Location = Location.HN,
                    Specification = "Spec for Laptop 3",
                    CategoryId = laptopCategory.Id,
                    CreatedDate = fixedCreatedDate
                },
                new Asset
                {
                    Id = Guid.NewGuid(),
                    Code = "MON002",
                    Name = "Monitor 2",
                    State = AssetState.Available,
                    InstalledDate = new DateTimeOffset(new DateTime(2023, 5, 1)),
                    Location = Location.HN,
                    Specification = "Spec for Monitor 2",
                    CategoryId = monitorCategory.Id,
                    CreatedDate = fixedCreatedDate
                }
            };

            await _dbContext.Assets.AddRangeAsync(assets);
            await _dbContext.SaveChangesAsync();

            _logger.LogInformation("Seeded {Count} assets", assets.Count);
        }

        // Seed Assignments
        private async Task SeedAssignmentsAsync()
        {
            if (_dbContext.Assignments.Any())
            {
                _logger.LogInformation("Assignments already seeded.");
                return;
            }

            _logger.LogInformation("Seeding assignments...");

            var users = await _dbContext.Users.ToListAsync();
            var assets = await _dbContext.Assets.ToListAsync();

            if (!users.Any() || !assets.Any())
            {
                throw new Exception("Users and assets must be seeded before assignments.");
            }

            // Get admins
            var adminHCM = users.First(u => u.Username == "johndoe");
            var adminHN = users.First(u => u.Username == "janesmith");

            // Get staff
            var staffHCM1 = users.First(u => u.Username == "robertj");
            var staffHCM2 = users.First(u => u.Username == "sarahlee");
            var staffHN1 = users.First(u => u.Username == "michaelc");

            // Get assets
            var assetLAP001 = assets.First(a => a.Code == "LAP001");
            var assetLAP002 = assets.First(a => a.Code == "LAP002");
            var assetMON001 = assets.First(a => a.Code == "MON001");
            var assetLAP003 = assets.First(a => a.Code == "LAP003");
            var assetMON002 = assets.First(a => a.Code == "MON002");

            var assignments = new List<Assignment>
            {
                // HCM Assignments
                new Assignment
                {
                    Id = Guid.NewGuid(),
                    AssetId = assetLAP001.Id,
                    AssignorId = adminHCM.Id,
                    AssigneeId = staffHCM1.Id,
                    AssignedDate = new DateTimeOffset(new DateTime(2024, 10, 1)),
                    Note = "Assignment 1",
                    State = AssignmentState.WaitingForAcceptance,
                    CreatedDate = fixedCreatedDate
                },
                new Assignment
                {
                    Id = Guid.NewGuid(),
                    AssetId = assetLAP002.Id,
                    AssignorId = adminHCM.Id,
                    AssigneeId = staffHCM2.Id,
                    AssignedDate = new DateTimeOffset(new DateTime(2024, 10, 5)),
                    Note = "Assignment 2",
                    State = AssignmentState.Accepted,
                    CreatedDate = fixedCreatedDate
                },
                new Assignment
                {
                    Id = Guid.NewGuid(),
                    AssetId = assetMON001.Id,
                    AssignorId = adminHCM.Id,
                    AssigneeId = staffHCM1.Id,
                    AssignedDate = new DateTimeOffset(new DateTime(2024, 10, 10)),
                    Note = "Assignment 3",
                    State = AssignmentState.Declined,
                    CreatedDate = fixedCreatedDate
                },
                // HN Assignments
                new Assignment
                {
                    Id = Guid.NewGuid(),
                    AssetId = assetLAP003.Id,
                    AssignorId = adminHN.Id,
                    AssigneeId = staffHN1.Id,
                    AssignedDate = new DateTimeOffset(new DateTime(2024, 10, 2)),
                    Note = "Assignment 4",
                    State = AssignmentState.WaitingForAcceptance,
                    CreatedDate = fixedCreatedDate
                },
                new Assignment
                {
                    Id = Guid.NewGuid(),
                    AssetId = assetMON002.Id,
                    AssignorId = adminHN.Id,
                    AssigneeId = staffHN1.Id,
                    AssignedDate = new DateTimeOffset(new DateTime(2024, 10, 6)),
                    Note = "Assignment 5",
                    State = AssignmentState.Accepted,
                    CreatedDate = fixedCreatedDate
                }
            };

            await _dbContext.Assignments.AddRangeAsync(assignments);
            await _dbContext.SaveChangesAsync();

            _logger.LogInformation("Seeded {Count} assignments", assignments.Count);
        }
    }
}