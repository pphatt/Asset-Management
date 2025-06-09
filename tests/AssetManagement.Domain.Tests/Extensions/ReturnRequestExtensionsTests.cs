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
                },
                new ReturnRequest
                {
                    Id = Guid.NewGuid(),
                    Assignment = new Assignment
                    {
                        Asset = new Asset { Code = "AST003", Name = "Keyboard Logitech", Location = Location.HN },
                        AssignedDate = new DateTimeOffset(2024, 1, 5, 0, 0, 0, TimeSpan.Zero)
                    },
                    Requester = new User { Username = "mark.wilson", Password = "Password" },
                    Acceptor = null,
                    ReturnedDate = null,
                    State = ReturnRequestState.WaitingForReturning,
                    CreatedDate = new DateTime(2024, 1, 30),
                    LastModifiedDate = new DateTime(2024, 1, 31)
                }
            };
            _mockQueryable = _returnRequests.AsQueryable().BuildMock();
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
            Assert.Single(result);
            Assert.Contains("samsung", result.First().Assignment.Asset.Name.ToLower());
        }

        [Fact]
        public void ApplySearch_WithNonExistentTerm_ReturnsEmptyResults()
        {
            // Act
            var result = _mockQueryable.ApplySearch("nonexistent");

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public void ApplySearch_WithPartialUsername_ReturnsMatchingResults()
        {
            // Act
            var result = _mockQueryable.ApplySearch("mark");

            // Assert
            Assert.Single(result);
            Assert.Contains("mark", result.First().Requester.Username);
        }

        [Fact]
        public void ApplySearch_WithMultipleMatches_ReturnsAllMatching()
        {
            // Act
            var result = _mockQueryable.ApplySearch("AST");

            // Assert
            Assert.Equal(3, result.Count());
        }
        #endregion

        #region ApplyFilters Tests
        [Fact]
        public void ApplyFilters_WithLocation_FiltersCorrectly()
        {
            // Act
            var result = _mockQueryable.ApplyFilters(null, null, Location.HN);

            // Assert
            Assert.Equal(2, result.Count());
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
            Assert.Equal(filterDate.Date, result.First().ReturnedDate?.Date);
        }

        [Fact]
        public void ApplyFilters_WithNullStates_DoesNotFilterByState()
        {
            // Act
            var result = _mockQueryable.ApplyFilters(null, null, Location.HN);

            // Assert
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public void ApplyFilters_WithEmptyStatesList_DoesNotFilterByState()
        {
            // Arrange
            var emptyStates = new List<ReturnRequestState>();

            // Act
            var result = _mockQueryable.ApplyFilters(emptyStates, null, Location.HN);

            // Assert
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public void ApplyFilters_WithMultipleStates_FiltersCorrectly()
        {
            // Arrange
            var states = new List<ReturnRequestState>
            {
                ReturnRequestState.Completed,
                ReturnRequestState.WaitingForReturning,
            };

            // Act
            var result = _mockQueryable.ApplyFilters(states, null, Location.HN);

            // Assert
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public void ApplyFilters_WithDateButNoReturnedDate_ReturnsEmpty()
        {
            // Arrange
            var filterDate = new DateTimeOffset(2024, 2, 1, 0, 0, 0, TimeSpan.Zero);

            // Act
            var result = _mockQueryable.ApplyFilters(null, filterDate, Location.HN);

            // Assert
            Assert.Single(result);
            Assert.Equal(filterDate.Date, result.First().ReturnedDate?.Date);
        }

        [Fact]
        public void ApplyFilters_WithNonExistentDate_ReturnsEmpty()
        {
            // Arrange
            var filterDate = new DateTimeOffset(2025, 1, 1, 0, 0, 0, TimeSpan.Zero);

            // Act
            var result = _mockQueryable.ApplyFilters(null, filterDate, Location.HN);

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public void ApplyFilters_WithAllParameters_CombinesFiltersCorrectly()
        {
            // Arrange
            var states = new List<ReturnRequestState> { ReturnRequestState.Completed };
            var filterDate = new DateTimeOffset(2024, 2, 1, 0, 0, 0, TimeSpan.Zero);

            // Act
            var result = _mockQueryable.ApplyFilters(states, filterDate, Location.HN);

            // Assert
            Assert.Single(result);
            var returnRequest = result.First();
            Assert.Equal(ReturnRequestState.Completed, returnRequest.State);
            Assert.Equal(filterDate.Date, returnRequest.ReturnedDate?.Date);
            Assert.Equal(Location.HN, returnRequest.Assignment.Asset.Location);
        }
        #endregion

        #region ApplySorting Tests
        [Fact]
        public void ApplySorting_WithNoSortingCriteria_DefaultSortsByReturnedDate()
        {
            // Act
            var result = _mockQueryable.ApplySorting([]);

            // Assert
            var resultList = result.ToList();
            Assert.Equal(3, resultList.Count);
            Assert.Null(resultList[0].ReturnedDate);
            Assert.True(resultList[1].ReturnedDate <= resultList[2].ReturnedDate);
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
            Assert.Equal(3, resultList.Count);
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
            Assert.Equal("AST003", resultList[0].Assignment.Asset.Code);
            Assert.Equal("AST002", resultList[1].Assignment.Asset.Code);
            Assert.Equal("AST001", resultList[2].Assignment.Asset.Code);
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
            Assert.Equal(3, resultList.Count);
        }

        [Fact]
        public void ApplySorting_WithNullSortingCriteria_DefaultSortsByReturnedDate()
        {
            // Act
            var result = _mockQueryable.ApplySorting(null!);

            // Assert
            var resultList = result.ToList();
            Assert.Equal(3, resultList.Count);
            // Items with null ReturnedDate should come first (default DateTime value)
            for (int i = 0; i < resultList.Count - 1; i++)
            {
                var current = resultList[i].ReturnedDate ?? default;
                var next = resultList[i + 1].ReturnedDate ?? default;
                Assert.True(current <= next);
            }
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
            for (int i = 0; i < resultList.Count - 1; i++)
            {
                Assert.True(string.Compare(resultList[i].Assignment.Asset.Name,
                                         resultList[i + 1].Assignment.Asset.Name,
                                         StringComparison.OrdinalIgnoreCase) <= 0);
            }
        }

        [Fact]
        public void ApplySorting_WithRequestedByDescending_SortsCorrectly()
        {
            // Arrange
            var sortingCriteria = new List<(string property, string order)> { ("requestedby", "desc") };

            // Act
            var result = _mockQueryable.ApplySorting(sortingCriteria);

            // Assert
            var resultList = result.ToList();
            Assert.Equal("mark.wilson", resultList[0].Requester.Username);
            Assert.Equal("john.doe", resultList[1].Requester.Username);
            Assert.Equal("jane.smith", resultList[2].Requester.Username);
        }

        [Fact]
        public void ApplySorting_WithAssignedDateAscending_SortsCorrectly()
        {
            // Arrange
            var sortingCriteria = new List<(string property, string order)> { ("assigneddate", "asc") };

            // Act
            var result = _mockQueryable.ApplySorting(sortingCriteria);

            // Assert
            var resultList = result.ToList();
            for (int i = 0; i < resultList.Count - 1; i++)
            {
                Assert.True(resultList[i].Assignment.AssignedDate <= resultList[i + 1].Assignment.AssignedDate);
            }
        }

        [Fact]
        public void ApplySorting_WithAcceptedByAscending_HandlesNullAcceptor()
        {
            // Arrange
            var sortingCriteria = new List<(string property, string order)> { ("acceptedby", "asc") };

            // Act
            var result = _mockQueryable.ApplySorting(sortingCriteria);

            // Assert
            var resultList = result.ToList();
            Assert.Equal(3, resultList.Count);
            // The item with null acceptor should sort as empty string and come first
            Assert.Null(resultList[0].Acceptor);
        }

        [Fact]
        public void ApplySorting_WithAcceptedByDescending_HandlesNullAcceptor()
        {
            // Arrange
            var sortingCriteria = new List<(string property, string order)> { ("acceptedby", "desc") };

            // Act
            var result = _mockQueryable.ApplySorting(sortingCriteria);

            // Assert
            var resultList = result.ToList();
            Assert.Equal(3, resultList.Count);
            // Items with actual acceptor usernames should come first when sorting descending
            Assert.NotNull(resultList[0].Acceptor);
            Assert.NotNull(resultList[1].Acceptor);
        }

        [Fact]
        public void ApplySorting_WithReturnedDateAscending_HandlesNullDates()
        {
            // Arrange
            var sortingCriteria = new List<(string property, string order)> { ("returneddate", "asc") };

            // Act
            var result = _mockQueryable.ApplySorting(sortingCriteria);

            // Assert
            var resultList = result.ToList();
            Assert.Equal(3, resultList.Count);
            // Null dates should be treated as default and come first
            Assert.Null(resultList[0].ReturnedDate);
        }

        [Fact]
        public void ApplySorting_WithReturnedDateDescending_HandlesNullDates()
        {
            // Arrange
            var sortingCriteria = new List<(string property, string order)> { ("returneddate", "desc") };

            // Act
            var result = _mockQueryable.ApplySorting(sortingCriteria);

            // Assert
            var resultList = result.ToList();
            Assert.Equal(3, resultList.Count);
            // Items with actual dates should come first when sorting descending
            Assert.NotNull(resultList[0].ReturnedDate);
            Assert.NotNull(resultList[1].ReturnedDate);
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
        public void ApplySorting_WithCreatedAscending_SortsCorrectly()
        {
            // Arrange
            var sortingCriteria = new List<(string property, string order)> { ("created", "asc") };

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
        public void ApplySorting_WithUpdatedDescending_SortsCorrectly()
        {
            // Arrange
            var sortingCriteria = new List<(string property, string order)> { ("updated", "desc") };

            // Act
            var result = _mockQueryable.ApplySorting(sortingCriteria);

            // Assert
            var resultList = result.ToList();
            for (int i = 0; i < resultList.Count - 1; i++)
            {
                Assert.True(resultList[i].LastModifiedDate >= resultList[i + 1].LastModifiedDate);
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
        public void ApplySorting_WithCaseInsensitiveOrder_HandlesCorrectly()
        {
            // Arrange
            var sortingCriteria = new List<(string property, string order)> { ("assetcode", "DESC") };

            // Act
            var result = _mockQueryable.ApplySorting(sortingCriteria);

            // Assert
            var resultList = result.ToList();
            Assert.Equal("AST003", resultList[0].Assignment.Asset.Code);
            Assert.Equal("AST001", resultList[2].Assignment.Asset.Code);
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
            Assert.Equal("AST001", resultList[0].Assignment.Asset.Code);
            Assert.Equal("AST002", resultList[1].Assignment.Asset.Code);
            Assert.Equal("AST003", resultList[2].Assignment.Asset.Code);
        }

        [Fact]
        public void ApplySorting_WithMultipleCriteriaComplex_SortsCorrectly()
        {
            // Arrange
            var sortingCriteria = new List<(string property, string order)>
            {
                ("state", "desc"),
                ("assetcode", "asc"),
                ("created", "desc")
            };

            // Act
            var result = _mockQueryable.ApplySorting(sortingCriteria);

            // Assert
            var resultList = result.ToList();
            Assert.Equal(3, resultList.Count);
            // Should be sorted by state desc, then assetcode asc, then created desc
            Assert.Equal("AST002", resultList[0].Assignment.Asset.Code);
            Assert.Equal("AST003", resultList[1].Assignment.Asset.Code);
            Assert.Equal("AST001", resultList[2].Assignment.Asset.Code);
        }

        #endregion
    }
}