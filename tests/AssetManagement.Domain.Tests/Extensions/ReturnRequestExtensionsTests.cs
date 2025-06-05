using AssetManagement.Contracts.Enums;
using AssetManagement.Domain.Entities;
using AssetManagement.Domain.Enums;
using MockQueryable;

namespace AssetManagement.Domain.Extensions.Tests
{
    public class ReturnRequestExtensionsTests
    {
        private readonly List<ReturnRequest> _returnRequests;
        private readonly IQueryable<ReturnRequest> _mockQueryable;

        public ReturnRequestExtensionsTests()
        {
            _returnRequests = new List<ReturnRequest>
            {
                new ReturnRequest
                {
                    Id = Guid.NewGuid(),
                    Assignment = new Assignment
                    {
                        Asset = new Asset { Code = "AST001", Name = "Laptop Dell", Location = Location.HN },
                        AssignedDate = new DateTimeOffset(2024, 1, 15, 0, 0, 0, TimeSpan.Zero)
                    },
                    Requester = new User { Username = "john.doe", Password = "Password" },
                    Acceptor = new User { Username = "admin1", Password = "Password" },
                    ReturnedDate = new DateTimeOffset(2024, 2, 1, 0, 0, 0, TimeSpan.Zero),
                    State = ReturnRequestState.Completed,
                    CreatedDate = new DateTime(2024, 1, 20),
                    LastModifiedDate = new DateTime(2024, 2, 1)
                },
                new ReturnRequest
                {
                    Id = Guid.NewGuid(),
                    Assignment = new Assignment
                    {
                        Asset = new Asset { Code = "AST002", Name = "Monitor Samsung", Location = Location.HCM },
                        AssignedDate = new DateTimeOffset(2024, 1, 10, 0, 0, 0, TimeSpan.Zero)
                    },
                    Requester = new User { Username = "jane.smith", Password = "Password" },
                    Acceptor = new User { Username = "admin2", Password = "Password" },
                    ReturnedDate = new DateTimeOffset(2024, 2, 5, 0, 0, 0, TimeSpan.Zero),
                    State = ReturnRequestState.WaitingForReturning,
                    CreatedDate = new DateTime(2024, 1, 25),
                    LastModifiedDate = new DateTime(2024, 2, 2)
                }
            };
            _mockQueryable = _returnRequests.AsQueryable().BuildMock();
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
            Assert.Equal("AST001", result.First().Assignment.Asset.Code);
        }

        [Fact]
        public void ApplySearch_WithAssetName_ReturnsMatchingResults()
        {
            // Act
            var result = _mockQueryable.ApplySearch("laptop");

            // Assert
            Assert.Single(result);
            Assert.Contains("laptop", result.First().Assignment.Asset.Name.ToLower());
        }

        [Fact]
        public void ApplySearch_WithUsername_ReturnsMatchingResults()
        {
            // Act
            var result = _mockQueryable.ApplySearch("john");

            // Assert
            Assert.Single(result);
            Assert.Contains("john", result.First().Requester.Username);
        }

        [Fact]
        public void ApplySearch_CaseInsensitive_ReturnsMatchingResults()
        {
            // Act
            var result = _mockQueryable.ApplySearch("LAPTOP");

            // Assert
            Assert.Single(result);
        }

        [Fact]
        public void ApplyFilters_WithLocation_FiltersCorrectly()
        {
            // Act
            var result = _mockQueryable.ApplyFilters(null, null, Location.HN);

            // Assert
            Assert.Single(result);
            Assert.Equal(Location.HN, result.First().Assignment.Asset.Location);
        }

        [Fact]
        public void ApplyFilters_WithStates_FiltersCorrectly()
        {
            // Arrange
            var states = new List<ReturnRequestState> { ReturnRequestState.Completed };

            // Act
            var result = _mockQueryable.ApplyFilters(states, null, Location.HN);

            // Assert
            Assert.Single(result);
            Assert.Equal(ReturnRequestState.Completed, result.First().State);
        }

        [Fact]
        public void ApplyFilters_WithDate_FiltersCorrectly()
        {
            // Arrange
            var filterDate = new DateTimeOffset(2024, 2, 1, 0, 0, 0, TimeSpan.Zero);

            // Act
            var result = _mockQueryable.ApplyFilters(null, filterDate, Location.HN);

            // Assert
            Assert.Single(result);
            Assert.Equal(filterDate.Date, result.First().ReturnedDate.Date);
        }

        [Fact]
        public void ApplySorting_WithNoSortingCriteria_DefaultSortsByReturnedDate()
        {
            // Act
            var result = _mockQueryable.ApplySorting([]);

            // Assert
            var resultList = result.ToList();
            Assert.Equal(2, resultList.Count);
            Assert.True(resultList[0].ReturnedDate <= resultList[1].ReturnedDate);
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
            Assert.Equal("AST001", resultList[0].Assignment.Asset.Code);
            Assert.Equal("AST002", resultList[1].Assignment.Asset.Code);
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
            Assert.Equal("AST002", resultList[0].Assignment.Asset.Code);
            Assert.Equal("AST001", resultList[1].Assignment.Asset.Code);
        }

        [Fact]
        public void ApplySorting_WithMultipleCriteria_SortsCorrectly()
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
            Assert.Equal(2, resultList.Count);
        }
    }
}