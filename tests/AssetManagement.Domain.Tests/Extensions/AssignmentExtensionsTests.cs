using AssetManagement.Domain.Entities;
using AssetManagement.Domain.Enums;
using MockQueryable;

namespace AssetManagement.Domain.Extensions.Tests
{
    public class AssignmentExtensionsTests
    {
        private readonly List<Assignment> _assignments;
        private readonly IQueryable<Assignment> _mockQueryable;

        public AssignmentExtensionsTests()
        {
            _assignments = new List<Assignment>
            {
                new Assignment
                {
                    Id = Guid.NewGuid(),
                    Asset = new Asset { Code = "AST001", Name = "Laptop Dell", Location = Location.HN },
                    Assignee = new User { Username = "john.doe", Password = "Password" },
                    Assignor = new User { Username = "admin1", Password = "Password" },
                    AssignedDate = new DateTimeOffset(2024, 1, 15, 0, 0, 0, TimeSpan.Zero),
                    State = AssignmentState.Accepted,
                    CreatedDate = new DateTime(2024, 1, 10),
                    LastModifiedDate = new DateTime(2024, 1, 16)
                },
                new Assignment
                {
                    Id = Guid.NewGuid(),
                    Asset = new Asset { Code = "AST002", Name = "Monitor Samsung", Location = Location.HCM },
                    Assignee = new User { Username = "jane.smith", Password = "Password" },
                    Assignor = new User { Username = "admin2", Password = "Password" },
                    AssignedDate = new DateTimeOffset(2024, 1, 20, 0, 0, 0, TimeSpan.Zero),
                    State = AssignmentState.WaitingForAcceptance,
                    CreatedDate = new DateTime(2024, 1, 18),
                    LastModifiedDate = new DateTime(2024, 1, 21)
                }
            };
            _mockQueryable = _assignments.AsQueryable().BuildMock();
        }

        [Fact]
        public void ApplySearch_WithNullSearchTerm_ReturnsOriginalQuery()
        {
            // Act
            var result = _mockQueryable.ApplySearch(null);

            // Assert
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public void ApplySearch_WithEmptySearchTerm_ReturnsOriginalQuery()
        {
            // Act
            var result = _mockQueryable.ApplySearch("");

            // Assert
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public void ApplySearch_WithAssetCode_ReturnsMatchingResults()
        {
            // Act
            var result = _mockQueryable.ApplySearch("AST001");

            // Assert
            Assert.Single(result);
            Assert.Equal("AST001", result.First().Asset.Code);
        }

        [Fact]
        public void ApplySearch_WithAssetName_ReturnsMatchingResults()
        {
            // Act
            var result = _mockQueryable.ApplySearch("laptop");

            // Assert
            Assert.Single(result);
            Assert.Contains("laptop", result.First().Asset.Name.ToLower());
        }

        [Fact]
        public void ApplySearch_WithAssigneeUsername_ReturnsMatchingResults()
        {
            // Act
            var result = _mockQueryable.ApplySearch("john");

            // Assert
            Assert.Single(result);
            Assert.Contains("john", result.First().Assignee.Username);
        }

        [Fact]
        public void ApplyFilters_WithLocation_FiltersCorrectly()
        {
            // Act
            var result = _mockQueryable.ApplyFilters(null, null, Location.HN);

            // Assert
            Assert.Single(result);
            Assert.Equal(Location.HN, result.First().Asset.Location);
        }

        [Fact]
        public void ApplyFilters_WithStates_FiltersCorrectly()
        {
            // Arrange
            var states = new List<AssignmentState> { AssignmentState.Accepted };

            // Act
            var result = _mockQueryable.ApplyFilters(states, null, Location.HN);

            // Assert
            Assert.Single(result);
            Assert.Equal(AssignmentState.Accepted, result.First().State);
        }

        [Fact]
        public void ApplyFilters_WithDate_FiltersCorrectly()
        {
            // Arrange
            var filterDate = new DateTimeOffset(2024, 1, 15, 0, 0, 0, TimeSpan.Zero);

            // Act
            var result = _mockQueryable.ApplyFilters(null, filterDate, Location.HN);

            // Assert
            Assert.Single(result);
            Assert.Equal(filterDate.Date, result.First().AssignedDate.Date);
        }

        [Fact]
        public void ApplySorting_WithNoSortingCriteria_DefaultSortsByAssignedDate()
        {
            // Act
            var result = _mockQueryable.ApplySorting([]);

            // Assert
            var resultList = result.ToList();
            Assert.Equal(2, resultList.Count);
            Assert.True(resultList[0].AssignedDate <= resultList[1].AssignedDate);
        }

        [Fact]
        public void ApplySorting_WithAssetCodeAscending_SortsCorrectly()
        {
            // Arrange
            var sortingCriteria = new List<(string property, string order)> { ("assetcode", "asc") };

            // Act
            var result = _mockQueryable.ApplySorting(sortingCriteria);

            // Assert
            var resultList = result.ToList();
            Assert.Equal("AST001", resultList[0].Asset.Code);
            Assert.Equal("AST002", resultList[1].Asset.Code);
        }

        [Fact]
        public void ApplySorting_WithAssignedToDescending_SortsCorrectly()
        {
            // Arrange
            var sortingCriteria = new List<(string property, string order)> { ("assignedto", "desc") };

            // Act
            var result = _mockQueryable.ApplySorting(sortingCriteria);

            // Assert
            var resultList = result.ToList();
            Assert.Equal("john.doe", resultList[0].Assignee.Username);
            Assert.Equal("jane.smith", resultList[1].Assignee.Username);
        }
    }
}
