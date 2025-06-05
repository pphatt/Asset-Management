using AssetManagement.Domain.Entities;
using AssetManagement.Domain.Enums;
using MockQueryable;

namespace AssetManagement.Domain.Extensions.Tests
{
    public class UserExtensionsTests
    {
        private readonly List<User> _users;
        private readonly IQueryable<User> _mockQueryable;

        public UserExtensionsTests()
        {
            _users = new List<User>
            {
                new User
                {
                    Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                    FirstName = "John",
                    LastName = "Doe",
                    Username = "john.doe",
                    Password = "Password",
                    StaffCode = "SD001",
                    Type = UserType.Staff,
                    Location = Location.HN,
                    JoinedDate = new DateTimeOffset(2024, 1, 15, 0, 0, 0, TimeSpan.Zero),
                    CreatedDate = new DateTime(2024, 1, 10),
                    LastModifiedDate = new DateTime(2024, 1, 16)
                },
                new User
                {
                    Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                    FirstName = "Jane",
                    LastName = "Smith",
                    Username = "jane.smith",
                    Password = "Password",
                    StaffCode = "SD002",
                    Type = UserType.Admin,
                    Location = Location.HCM,
                    JoinedDate = new DateTimeOffset(2024, 1, 20, 0, 0, 0, TimeSpan.Zero),
                    CreatedDate = new DateTime(2024, 1, 18),
                    LastModifiedDate = new DateTime(2024, 1, 21)
                },
                new User
                {
                    Id = Guid.Parse("33333333-3333-3333-3333-333333333333"),
                    FirstName = "Bob",
                    LastName = "Johnson",
                    Username = "bob.johnson",
                    Password = "Password",
                    StaffCode = "SD003",
                    Type = UserType.Staff,
                    Location = Location.HN,
                    JoinedDate = new DateTimeOffset(2024, 1, 25, 0, 0, 0, TimeSpan.Zero),
                    CreatedDate = new DateTime(2024, 1, 23),
                    LastModifiedDate = new DateTime(2024, 1, 26)
                }
            };
            _mockQueryable = _users.BuildMock();
        }

        [Fact]
        public void ApplySearch_WithNullSearchTerm_ReturnsOriginalQuery()
        {
            // Act
            var result = _mockQueryable.ApplySearch(null);

            // Assert
            Assert.Equal(3, result.Count());
        }

        [Fact]
        public void ApplySearch_WithEmptySearchTerm_ReturnsOriginalQuery()
        {
            // Act
            var result = _mockQueryable.ApplySearch("");

            // Assert
            Assert.Equal(3, result.Count());
        }

        [Fact]
        public void ApplySearch_WithFirstNameLastName_ReturnsMatchingResults()
        {
            // Act
            var result = _mockQueryable.ApplySearch("John Doe");

            // Assert
            Assert.Single(result);
            Assert.Equal("John", result.First().FirstName);
            Assert.Equal("Doe", result.First().LastName);
        }

        [Fact]
        public void ApplySearch_WithLastNameFirstName_ReturnsMatchingResults()
        {
            // Act
            var result = _mockQueryable.ApplySearch("Doe John");

            // Assert
            Assert.Single(result);
            Assert.Equal("John", result.First().FirstName);
            Assert.Equal("Doe", result.First().LastName);
        }

        [Fact]
        public void ApplySearch_WithStaffCode_ReturnsMatchingResults()
        {
            // Act
            var result = _mockQueryable.ApplySearch("SD001");

            // Assert
            Assert.Single(result);
            Assert.Equal("SD001", result.First().StaffCode);
        }

        [Fact]
        public void ApplySearch_WithPartialName_ReturnsMatchingResults()
        {
            // Act
            var result = _mockQueryable.ApplySearch("jane");

            // Assert
            Assert.Single(result);
            Assert.Equal("Jane", result.First().FirstName);
        }

        [Fact]
        public void ApplyFilters_WithLocation_FiltersCorrectly()
        {
            // Act
            var result = _mockQueryable.ApplyFilters(null, Location.HN);

            // Assert
            Assert.Equal(2, result.Count());
            Assert.All(result, u => Assert.Equal(Location.HN, u.Location));
        }

        [Fact]
        public void ApplyFilters_WithUserTypeStaff_FiltersCorrectly()
        {
            // Act
            var result = _mockQueryable.ApplyFilters("staff", Location.HN);

            // Assert
            Assert.Equal(2, result.Count());
            Assert.All(result, u => Assert.Equal(UserType.Staff, u.Type));
        }

        [Fact]
        public void ApplyFilters_WithUserTypeAdmin_FiltersCorrectly()
        {
            // Act
            var result = _mockQueryable.ApplyFilters("admin", Location.HCM);

            // Assert
            Assert.Single(result);
            Assert.Equal(UserType.Admin, result.First().Type);
        }

        [Fact]
        public void ApplyFilters_WithUserTypeAll_ReturnsAllFromLocation()
        {
            // Act
            var result = _mockQueryable.ApplyFilters("all", Location.HN);

            // Assert
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public void ApplyFilters_WithNullUserType_ReturnsAllFromLocation()
        {
            // Act
            var result = _mockQueryable.ApplyFilters(null, Location.HN);

            // Assert
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public void ApplySorting_WithNoSortingCriteria_ReturnsOriginalQuery()
        {
            // Act
            var result = _mockQueryable.ApplySorting([]);

            // Assert
            Assert.Equal(3, result.Count());
        }

        [Fact]
        public void ApplySorting_WithNameAscending_SortsCorrectly()
        {
            // Arrange
            var sortingCriteria = new List<(string property, string order)> { ("name", "asc") };

            // Act
            var result = _mockQueryable.ApplySorting(sortingCriteria);

            // Assert
            var resultList = result.ToList();
            Assert.Equal("Bob", resultList[0].FirstName);
            Assert.Equal("Jane", resultList[1].FirstName);
            Assert.Equal("John", resultList[2].FirstName);
        }

        [Fact]
        public void ApplySorting_WithStaffCodeDescending_SortsCorrectly()
        {
            // Arrange
            var sortingCriteria = new List<(string property, string order)> { ("code", "desc") };

            // Act
            var result = _mockQueryable.ApplySorting(sortingCriteria);

            // Assert
            var resultList = result.ToList();
            Assert.Equal("SD003", resultList[0].StaffCode);
            Assert.Equal("SD002", resultList[1].StaffCode);
            Assert.Equal("SD001", resultList[2].StaffCode);
        }

        [Fact]
        public void ApplySorting_WithTypeAscending_SortsCorrectly()
        {
            // Arrange
            var sortingCriteria = new List<(string property, string order)> { ("type", "asc") };

            // Act
            var result = _mockQueryable.ApplySorting(sortingCriteria);

            // Assert
            var resultList = result.ToList();
            Assert.Equal(3, resultList.Count);
        }

        [Fact]
        public void ApplySorting_WithJoinedDateDescending_SortsCorrectly()
        {
            // Arrange
            var sortingCriteria = new List<(string property, string order)> { ("joined", "desc") };

            // Act
            var result = _mockQueryable.ApplySorting(sortingCriteria);

            // Assert
            var resultList = result.ToList();
            Assert.True(resultList[0].JoinedDate >= resultList[1].JoinedDate);
            Assert.True(resultList[1].JoinedDate >= resultList[2].JoinedDate);
        }

        [Fact]
        public void ApplySorting_WithUnknownProperty_DefaultsToId()
        {
            // Arrange
            var sortingCriteria = new List<(string property, string order)> { ("unknown", "asc") };

            // Act
            var result = _mockQueryable.ApplySorting(sortingCriteria);

            // Assert
            var resultList = result.ToList();
            Assert.Equal(3, resultList.Count);
            Assert.Equal("11111111-1111-1111-1111-111111111111", resultList[0].Id.ToString());
            Assert.Equal("22222222-2222-2222-2222-222222222222", resultList[1].Id.ToString());
            Assert.Equal("33333333-3333-3333-3333-333333333333", resultList[2].Id.ToString());
        }

        [Fact]
        public void ApplySorting_WithMultipleCriteria_SortsCorrectly()
        {
            // Arrange
            var sortingCriteria = new List<(string property, string order)>
            {
                ("type", "asc"),
                ("name", "desc")
            };

            // Act
            var result = _mockQueryable.ApplySorting(sortingCriteria);

            // Assert
            var resultList = result.ToList();
            Assert.Equal(3, resultList.Count);
        }
    }
}