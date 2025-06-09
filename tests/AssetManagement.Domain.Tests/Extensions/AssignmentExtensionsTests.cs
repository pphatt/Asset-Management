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
                },
                new Assignment
                {
                    Id = Guid.NewGuid(),
                    Asset = new Asset { Code = "AST003", Name = "Samsung Galaxy", Location = Location.HN },
                    Assignee = new User { Username = "mark.doe", Password = "Password" },
                    Assignor = new User { Username = "admin3", Password = "Password" },
                    AssignedDate = new DateTimeOffset(2024, 2, 15, 0, 0, 0, TimeSpan.Zero),
                    State = AssignmentState.WaitingForReturning,
                    CreatedDate = new DateTime(2023, 4, 10),
                    LastModifiedDate = new DateTime(2024, 1, 16)
                },
            };
            _mockQueryable = _assignments.AsQueryable().BuildMock();
        }

        #region ApplySearch Tests
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
        public void ApplySearch_WithWhitespaceSearchTerm_ReturnsOriginalQuery()
        {
            // Act
            var result = _mockQueryable.ApplySearch("   ");

            // Assert
            Assert.Equal(3, result.Count());
        }

        [Fact]
        public void ApplySearch_WithPartialAssetName_ReturnsMatchingResults()
        {
            // Act
            var result = _mockQueryable.ApplySearch("samsung");

            // Assert
            Assert.Equal(2, result.Count());
            Assert.All(result, assignment =>
                Assert.Contains("samsung", assignment.Asset.Name.ToLower()));
        }

        [Fact]
        public void ApplySearch_WithCaseInsensitiveSearch_ReturnsMatchingResults()
        {
            // Act
            var result = _mockQueryable.ApplySearch("LAPTOP");

            // Assert
            Assert.Single(result);
            Assert.Contains("laptop", result.First().Asset.Name.ToLower());
        }

        [Fact]
        public void ApplySearch_WithNonExistentTerm_ReturnsEmptyResults()
        {
            // Act
            var result = _mockQueryable.ApplySearch("nonexistent");

            // Assert
            Assert.Empty(result);
        }
        #endregion

        #region ApplyFilters Tests
        [Fact]
        public void ApplyFilters_WithLocation_FiltersCorrectly()
        {
            // Act
            var result = _mockQueryable.ApplyFilters(null, null, Location.HCM);

            // Assert
            Assert.Single(result);
            Assert.Equal(Location.HCM, result.First().Asset.Location);
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
        public void ApplyFilters_WithEmptyStatesList_DoesNotFilterByState()
        {
            // Arrange
            var emptyStates = new List<AssignmentState>();

            // Act
            var result = _mockQueryable.ApplyFilters(emptyStates, null, Location.HN);

            // Assert
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public void ApplyFilters_WithMultipleStates_FiltersCorrectly()
        {
            // Arrange
            var states = new List<AssignmentState>
            {
                AssignmentState.Accepted,
                AssignmentState.WaitingForAcceptance
            };

            // Act
            var result = _mockQueryable.ApplyFilters(states, null, Location.HN);

            // Assert
            Assert.Single(result); // Only one HN assignment matches these states
            Assert.Equal(AssignmentState.Accepted, result.First().State);
        }

        [Fact]
        public void ApplyFilters_WithAllParameters_CombinesFiltersCorrectly()
        {
            // Arrange
            var states = new List<AssignmentState> { AssignmentState.Accepted };
            var filterDate = new DateTimeOffset(2024, 1, 15, 0, 0, 0, TimeSpan.Zero);

            // Act
            var result = _mockQueryable.ApplyFilters(states, filterDate, Location.HN);

            // Assert
            Assert.Single(result);
            var assignment = result.First();
            Assert.Equal(AssignmentState.Accepted, assignment.State);
            Assert.Equal(filterDate.Date, assignment.AssignedDate.Date);
            Assert.Equal(Location.HN, assignment.Asset.Location);
        }

        [Fact]
        public void ApplyFilters_WithDateButNoMatches_ReturnsEmpty()
        {
            // Arrange
            var filterDate = new DateTimeOffset(2025, 1, 1, 0, 0, 0, TimeSpan.Zero);

            // Act
            var result = _mockQueryable.ApplyFilters(null, filterDate, Location.HN);

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public void ApplyFilters_WithStateButNoMatches_ReturnsEmpty()
        {
            // Arrange
            var states = new List<AssignmentState> { AssignmentState.Declined };

            // Act
            var result = _mockQueryable.ApplyFilters(states, null, Location.HN);

            // Assert
            Assert.Empty(result);
        }
        #endregion

        #region ApplySorting Tests
        [Fact]
        public void ApplySorting_WithNoSortingCriteria_DefaultSortsByAssignedDate()
        {
            // Act
            var result = _mockQueryable.ApplySorting([]);

            // Assert
            var resultList = result.ToList();
            Assert.Equal(3, resultList.Count);
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
            Assert.Equal("mark.doe", resultList[0].Assignee.Username);
            Assert.Equal("john.doe", resultList[1].Assignee.Username);
            Assert.Equal("jane.smith", resultList[2].Assignee.Username);
        }

        [Fact]
        public void ApplySorting_WithAssetNameAscending_SortsCorrectly()
        {
            // Arrange
            var sortingCriteria = new List<(string property, string order)> { ("assetname", "asc") };

            // Act
            var result = _mockQueryable.ApplySorting(sortingCriteria);

            // Assert
            var resultList = result.ToList();
            Assert.Equal("john.doe", resultList[0].Assignee.Username);
            Assert.Equal("jane.smith", resultList[1].Assignee.Username);
        }

        [Fact]
        public void ApplySorting_WithNullSortingCriteria_DefaultSortsByAssignedDate()
        {
            // Act
            var result = _mockQueryable.ApplySorting(null!);

            // Assert
            var resultList = result.ToList();
            Assert.Equal(3, resultList.Count);
            // Should be sorted by AssignedDate ascending by default
            for (int i = 0; i < resultList.Count - 1; i++)
            {
                Assert.True(resultList[i].AssignedDate <= resultList[i + 1].AssignedDate);
            }
        }

        [Fact]
        public void ApplySorting_WithAssetCodeDescending_SortsCorrectly()
        {
            // Arrange
            var sortingCriteria = new List<(string property, string order)> { ("assetcode", "desc") };

            // Act
            var result = _mockQueryable.ApplySorting(sortingCriteria);

            // Assert
            var resultList = result.ToList();
            Assert.Equal("AST003", resultList[0].Asset.Code);
            Assert.Equal("AST002", resultList[1].Asset.Code);
            Assert.Equal("AST001", resultList[2].Asset.Code);
        }

        [Fact]
        public void ApplySorting_WithAssignedByAscending_SortsCorrectly()
        {
            // Arrange
            var sortingCriteria = new List<(string property, string order)> { ("assignedby", "asc") };

            // Act
            var result = _mockQueryable.ApplySorting(sortingCriteria);

            // Assert
            var resultList = result.ToList();
            Assert.Equal("admin1", resultList[0].Assignor.Username);
            Assert.Equal("admin2", resultList[1].Assignor.Username);
            Assert.Equal("admin3", resultList[2].Assignor.Username);
        }

        [Fact]
        public void ApplySorting_WithAssignedDateDescending_SortsCorrectly()
        {
            // Arrange
            var sortingCriteria = new List<(string property, string order)> { ("assigneddate", "desc") };

            // Act
            var result = _mockQueryable.ApplySorting(sortingCriteria);

            // Assert
            var resultList = result.ToList();
            Assert.True(resultList[0].AssignedDate >= resultList[1].AssignedDate);
            Assert.True(resultList[1].AssignedDate >= resultList[2].AssignedDate);
        }

        [Fact]
        public void ApplySorting_WithStateAscending_SortsCorrectly()
        {
            // Arrange
            var sortingCriteria = new List<(string property, string order)> { ("state", "asc") };

            // Act
            var result = _mockQueryable.ApplySorting(sortingCriteria);

            // Assert
            var resultList = result.ToList();
            for (int i = 0; i < resultList.Count - 1; i++)
            {
                Assert.True((int)resultList[i].State <= (int)resultList[i + 1].State);
            }
        }

        [Fact]
        public void ApplySorting_WithNoProperty_SortsCorrectly()
        {
            // Arrange
            var sortingCriteria = new List<(string property, string order)> { ("no", "asc") };

            // Act
            var result = _mockQueryable.ApplySorting(sortingCriteria);

            // Assert
            var resultList = result.ToList();
            for (int i = 0; i < resultList.Count - 1; i++)
            {
                Assert.True(resultList[i].CreatedDate <= resultList[i + 1].CreatedDate);
            }
        }

        [Fact]
        public void ApplySorting_WithCreatedProperty_SortsCorrectly()
        {
            // Arrange
            var sortingCriteria = new List<(string property, string order)> { ("created", "desc") };

            // Act
            var result = _mockQueryable.ApplySorting(sortingCriteria);

            // Assert
            var resultList = result.ToList();
            for (int i = 0; i < resultList.Count - 1; i++)
            {
                Assert.True(resultList[i].CreatedDate >= resultList[i + 1].CreatedDate);
            }
        }

        [Fact]
        public void ApplySorting_WithUpdatedProperty_SortsCorrectly()
        {
            // Arrange
            var sortingCriteria = new List<(string property, string order)> { ("updated", "asc") };

            // Act
            var result = _mockQueryable.ApplySorting(sortingCriteria);

            // Assert
            var resultList = result.ToList();
            for (int i = 0; i < resultList.Count - 1; i++)
            {
                Assert.True(resultList[i].LastModifiedDate <= resultList[i + 1].LastModifiedDate);
            }
        }

        [Fact]
        public void ApplySorting_WithUnknownProperty_DefaultsToCreatedDate()
        {
            // Arrange
            var sortingCriteria = new List<(string property, string order)> { ("unknown", "asc") };

            // Act
            var result = _mockQueryable.ApplySorting(sortingCriteria);

            // Assert
            var resultList = result.ToList();
            for (int i = 0; i < resultList.Count - 1; i++)
            {
                Assert.True(resultList[i].CreatedDate <= resultList[i + 1].CreatedDate);
            }
        }

        [Fact]
        public void ApplySorting_WithMultipleSortingCriteria_AppliesCorrectly()
        {
            // Arrange
            var sortingCriteria = new List<(string property, string order)>
            {
                ("state", "asc"),
                ("assetcode", "desc")
            };

            // Act
            var result = _mockQueryable.ApplySorting(sortingCriteria);

            // Assert
            var resultList = result.ToList();
            Assert.Equal(3, resultList.Count);
            Assert.Equal("AST001", resultList[0].Asset.Code);
            Assert.Equal("AST002", resultList[1].Asset.Code);
            Assert.Equal("AST003", resultList[2].Asset.Code);
        }

        [Fact]
        public void ApplySorting_WithCaseInsensitiveOrder_HandlesCorrectly()
        {
            // Arrange
            var sortingCriteria = new List<(string property, string order)> { ("assetcode", "DESC") };

            // Act
            var result = _mockQueryable.ApplySorting(sortingCriteria);

            // Assert
            var resultList = result.ToList();
            Assert.Equal("AST003", resultList[0].Asset.Code);
            Assert.Equal("AST001", resultList[2].Asset.Code);
        }

        [Fact]
        public void ApplySorting_WithInvalidOrder_DefaultsToAscending()
        {
            // Arrange
            var sortingCriteria = new List<(string property, string order)> { ("assetcode", "invalid") };

            // Act
            var result = _mockQueryable.ApplySorting(sortingCriteria);

            // Assert
            var resultList = result.ToList();
            Assert.Equal("AST001", resultList[0].Asset.Code);
            Assert.Equal("AST002", resultList[1].Asset.Code);
            Assert.Equal("AST003", resultList[2].Asset.Code);
        }
        #endregion
    }
}
