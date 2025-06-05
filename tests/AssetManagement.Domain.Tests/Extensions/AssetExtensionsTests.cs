using AssetManagement.Domain.Entities;
using AssetManagement.Domain.Enums;
using MockQueryable;

namespace AssetManagement.Domain.Extensions.Tests
{
    public class AssetExtensionsTests
    {
        #region Test Data Setup
        private readonly List<Asset> _assets;
        private readonly IQueryable<Asset> _mockQueryable;

        public AssetExtensionsTests()
        {
            var category1 = new Category { Id = Guid.NewGuid(), Name = "Electronics" };
            var category2 = new Category { Id = Guid.NewGuid(), Name = "Furniture" };
            var category3 = new Category { Id = Guid.NewGuid(), Name = "Software" };

            _assets = new List<Asset>
            {
                new Asset
                {
                    Id = Guid.NewGuid(),
                    Code = "EL001",
                    Name = "Laptop Dell",
                    State = AssetState.Available,
                    Category = category1,
                    Location = Location.HCM,
                    CreatedDate = new DateTime(2023, 1, 1),
                    LastModifiedDate = new DateTime(2023, 1, 2)
                },
                new Asset
                {
                    Id = Guid.NewGuid(),
                    Code = "EL002",
                    Name = "Monitor Samsung",
                    State = AssetState.Assigned,
                    Category = category1,
                    Location = Location.HCM,
                    CreatedDate = new DateTime(2023, 1, 3),
                    LastModifiedDate = new DateTime(2023, 1, 4)
                },
                new Asset
                {
                    Id = Guid.NewGuid(),
                    Code = "FU001",
                    Name = "Office Chair",
                    State = AssetState.NotAvailable,
                    Category = category2,
                    Location = Location.HN,
                    CreatedDate = new DateTime(2023, 1, 5),
                    LastModifiedDate = new DateTime(2023, 1, 6)
                },
                new Asset
                {
                    Id = Guid.NewGuid(),
                    Code = "FU002",
                    Name = "Desk Wooden",
                    State = AssetState.WaitingForRecycling,
                    Category = category2,
                    Location = Location.HCM,
                    CreatedDate = new DateTime(2023, 1, 7),
                    LastModifiedDate = new DateTime(2023, 1, 8)
                },
                new Asset
                {
                    Id = Guid.NewGuid(),
                    Code = "SW001",
                    Name = "Microsoft Office",
                    State = AssetState.Recycled,
                    Category = category3,
                    Location = Location.HN,
                    CreatedDate = new DateTime(2023, 1, 9),
                    LastModifiedDate = new DateTime(2023, 1, 10)
                }
            };
            _mockQueryable = _assets.BuildMock();
        }
        #endregion

        #region ApplySearch Tests
        [Fact]
        public void ApplySearch_WithEmptySearchTerm_ReturnsOriginalQuery()
        {
            // Act
            var result = _mockQueryable.ApplySearch("");

            // Assert
            Assert.Equal(5, result.Count());
        }

        [Fact]
        public void ApplySearch_WithNullSearchTerm_ReturnsOriginal_mockQueryable()
        {
            // Act
            var result = _mockQueryable.ApplySearch(null);

            // Assert
            Assert.Equal(5, result.Count());
        }

        [Fact]
        public void ApplySearch_WithWhitespaceSearchTerm_ReturnsOriginal_mockQueryable()
        {
            // Act
            var result = _mockQueryable.ApplySearch("   ");

            // Assert
            Assert.Equal(5, result.Count());
        }

        [Fact]
        public void ApplySearch_WithValidCodeSearchTerm_ReturnsMatchingAssets()
        {
            // Act
            var result = _mockQueryable.ApplySearch("EL001");

            // Assert
            Assert.Single(result);
            Assert.Equal("EL001", result.First().Code);
        }

        [Fact]
        public void ApplySearch_WithValidNameSearchTerm_ReturnsMatchingAssets()
        {
            // Act
            var result = _mockQueryable.ApplySearch("Laptop");

            // Assert
            Assert.Single(result);
            Assert.Equal("Laptop Dell", result.First().Name);
        }

        [Fact]
        public void ApplySearch_WithPartialMatch_ReturnsMatchingAssets()
        {
            // Act
            var result = _mockQueryable.ApplySearch("Fu");

            // Assert
            Assert.Equal(2, result.Count());
            Assert.All(result, asset => Assert.StartsWith("FU", asset.Code));
        }

        [Fact]
        public void ApplySearch_WithCaseInsensitiveMatch_ReturnsMatchingAssets()
        {
            // Act
            var result = _mockQueryable.ApplySearch("laptop");

            // Assert
            Assert.Single(result);
            Assert.Equal("Laptop Dell", result.First().Name);
        }

        [Fact]
        public void ApplySearch_WithNoMatch_ReturnsEmptyResult()
        {
            // Act
            var result = _mockQueryable.ApplySearch("XYZ999");

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
            Assert.Equal(3, result.Count());
            Assert.All(result, asset => Assert.Equal(Location.HCM, asset.Location));
        }

        [Fact]
        public void ApplyFilters_WithNullAssetStates_DoesNotFilterByState()
        {
            // Act
            var result = _mockQueryable.ApplyFilters(null, null, Location.HCM);

            // Assert
            Assert.Equal(3, result.Count());
        }

        [Fact]
        public void ApplyFilters_WithEmptyAssetStates_DoesNotFilterByState()
        {
            // Act
            var result = _mockQueryable.ApplyFilters(new List<string>(), null, Location.HCM);

            // Assert
            Assert.Equal(3, result.Count());
        }

        [Fact]
        public void ApplyFilters_WithAllAssetStates_DoesNotFilterByState()
        {
            // Act
            var result = _mockQueryable.ApplyFilters(new List<string> { "All" }, null, Location.HCM);

            // Assert
            Assert.Equal(3, result.Count());
        }

        [Fact]
        public void ApplyFilters_WithSpecificAssetState_Assigned_FiltersCorrectly()
        {
            // Act
            var result = _mockQueryable.ApplyFilters(new List<string> { "Assigned" }, null, Location.HCM);

            // Assert
            Assert.Single(result);
            Assert.Equal(AssetState.Assigned, result.First().State);
        }

        [Fact]
        public void ApplyFilters_WithSpecificAssetState_Available_FiltersCorrectly()
        {
            // Act
            var result = _mockQueryable.ApplyFilters(new List<string> { "Available" }, null, Location.HCM);

            // Assert
            Assert.Single(result);
            Assert.Equal(AssetState.Available, result.First().State);
        }

        [Fact]
        public void ApplyFilters_WithSpecificAssetState_NotAvailable_FiltersCorrectly()
        {
            // Act
            var result = _mockQueryable.ApplyFilters(new List<string> { "NotAvailable" }, null, Location.HN);

            // Assert
            Assert.Single(result);
            Assert.Equal(AssetState.NotAvailable, result.First().State);
        }

        [Fact]
        public void ApplyFilters_WithSpecificAssetState_WaitingForRecycling_FiltersCorrectly()
        {
            // Act
            var result = _mockQueryable.ApplyFilters(new List<string> { "WaitingForRecycling" }, null, Location.HCM);

            // Assert
            Assert.Single(result);
            Assert.Equal(AssetState.WaitingForRecycling, result.First().State);
        }

        [Fact]
        public void ApplyFilters_WithSpecificAssetState_Recycled_FiltersCorrectly()
        {
            // Act
            var result = _mockQueryable.ApplyFilters(new List<string> { "Recycled" }, null, Location.HN);

            // Assert
            Assert.Single(result);
            Assert.Equal(AssetState.Recycled, result.First().State);
        }

        [Fact]
        public void ApplyFilters_WithMultipleAssetStates_FiltersCorrectly()
        {
            // Act
            var result = _mockQueryable.ApplyFilters(new List<string> { "Available", "Assigned" }, null, Location.HCM);

            // Assert
            Assert.Equal(2, result.Count());
            Assert.Contains(result, a => a.State == AssetState.Available);
            Assert.Contains(result, a => a.State == AssetState.Assigned);
        }

        [Fact]
        public void ApplyFilters_WithNullAssetCategories_DoesNotFilterByCategory()
        {
            // Act
            var result = _mockQueryable.ApplyFilters(null, null, Location.HCM);

            // Assert
            Assert.Equal(3, result.Count());
        }

        [Fact]
        public void ApplyFilters_WithEmptyAssetCategories_DoesNotFilterByCategory()
        {
            // Act
            var result = _mockQueryable.ApplyFilters(null, new List<string>(), Location.HCM);

            // Assert
            Assert.Equal(3, result.Count());
        }

        [Fact]
        public void ApplyFilters_WithSpecificCategory_FiltersCorrectly()
        {
            // Act
            var result = _mockQueryable.ApplyFilters(null, new List<string> { "Electronics" }, Location.HCM);

            // Assert
            Assert.Equal(2, result.Count());
            Assert.All(result, asset => Assert.Equal("Electronics", asset.Category.Name));
        }

        [Fact]
        public void ApplyFilters_WithMultipleCategories_FiltersCorrectly()
        {
            // Act
            var result = _mockQueryable.ApplyFilters(null, new List<string> { "Electronics", "Furniture" }, Location.HCM);

            // Assert
            Assert.Equal(3, result.Count());
            Assert.Contains(result, a => a.Category.Name == "Electronics");
            Assert.Contains(result, a => a.Category.Name == "Furniture");
        }

        [Fact]
        public void ApplyFilters_WithStateAndCategoryFilters_FiltersCorrectly()
        {
            // Act
            var result = _mockQueryable.ApplyFilters(
                new List<string> { "Available" },
                new List<string> { "Electronics" },
                Location.HCM);

            // Assert
            Assert.Single(result);
            Assert.Equal(AssetState.Available, result.First().State);
            Assert.Equal("Electronics", result.First().Category.Name);
        }
        #endregion

        #region ApplySorting Tests
        [Fact]
        public void ApplySorting_WithNullSortingCriteria_ReturnsOriginal_mockQueryable()
        {
            // Act
            var result = _mockQueryable.ApplySorting([]);

            // Assert
            Assert.Equal(5, result.Count());
            // Should maintain original order
            Assert.Equal("EL001", result.First().Code);
        }

        [Fact]
        public void ApplySorting_WithEmptySortingCriteria_ReturnsOriginal_mockQueryable()
        {
            // Act
            var result = _mockQueryable.ApplySorting(new List<(string Property, string Order)>());

            // Assert
            Assert.Equal(5, result.Count());
            Assert.Equal("EL001", result.First().Code);
        }

        [Fact]
        public void ApplySorting_ByName_Ascending_SortsCorrectly()
        {
           // Act
            var result = _mockQueryable.ApplySorting(new List<(string Property, string Order)>
            {
                ("name", "asc")
            });

            // Assert
            var sortedAssets = result.ToList();
            Assert.Equal("Desk Wooden", sortedAssets[0].Name);
            Assert.Equal("Laptop Dell", sortedAssets[1].Name);
            Assert.Equal("Microsoft Office", sortedAssets[2].Name);
            Assert.Equal("Monitor Samsung", sortedAssets[3].Name);
            Assert.Equal("Office Chair", sortedAssets[4].Name);
        }

        [Fact]
        public void ApplySorting_ByName_Descending_SortsCorrectly()
        {
            // Act
            var result = _mockQueryable.ApplySorting(new List<(string Property, string Order)>
            {
                ("name", "desc")
            });

            // Assert
            var sortedAssets = result.ToList();
            Assert.Equal("Office Chair", sortedAssets[0].Name);
            Assert.Equal("Monitor Samsung", sortedAssets[1].Name);
            Assert.Equal("Microsoft Office", sortedAssets[2].Name);
            Assert.Equal("Laptop Dell", sortedAssets[3].Name);
            Assert.Equal("Desk Wooden", sortedAssets[4].Name);
        }

        [Fact]
        public void ApplySorting_ByCode_Ascending_SortsCorrectly()
        {
            // Act
            var result = _mockQueryable.ApplySorting(new List<(string Property, string Order)>
            {
                ("code", "asc")
            });

            // Assert
            var sortedAssets = result.ToList();
            Assert.Equal("EL001", sortedAssets[0].Code);
            Assert.Equal("EL002", sortedAssets[1].Code);
            Assert.Equal("FU001", sortedAssets[2].Code);
            Assert.Equal("FU002", sortedAssets[3].Code);
            Assert.Equal("SW001", sortedAssets[4].Code);
        }

        [Fact]
        public void ApplySorting_ByCode_Descending_SortsCorrectly()
        {
            // Act
            var result = _mockQueryable.ApplySorting(new List<(string Property, string Order)>
            {
                ("code", "desc")
            });

            // Assert
            var sortedAssets = result.ToList();
            Assert.Equal("SW001", sortedAssets[0].Code);
            Assert.Equal("FU002", sortedAssets[1].Code);
            Assert.Equal("FU001", sortedAssets[2].Code);
            Assert.Equal("EL002", sortedAssets[3].Code);
            Assert.Equal("EL001", sortedAssets[4].Code);
        }

        [Fact]
        public void ApplySorting_ByState_Ascending_SortsCorrectly()
        {
            // Act
            var result = _mockQueryable.ApplySorting(new List<(string Property, string Order)>
            {
                ("state", "asc")
            });

            // Assert
            var sortedAssets = result.ToList();
            // States should be sorted by enum value
            Assert.Equal(AssetState.Assigned, sortedAssets[0].State);
            Assert.Equal(AssetState.Available, sortedAssets[1].State);
            Assert.Equal(AssetState.NotAvailable, sortedAssets[2].State);
            Assert.Equal(AssetState.WaitingForRecycling, sortedAssets[3].State);
            Assert.Equal(AssetState.Recycled, sortedAssets[4].State);
        }

        [Fact]
        public void ApplySorting_ByState_Descending_SortsCorrectly()
        {
            // Act
            var result = _mockQueryable.ApplySorting(new List<(string Property, string Order)>
            {
                ("state", "desc")
            });

            // Assert
            var sortedAssets = result.ToList();
            Assert.Equal(AssetState.Recycled, sortedAssets[0].State);
            Assert.Equal(AssetState.WaitingForRecycling, sortedAssets[1].State);
            Assert.Equal(AssetState.NotAvailable, sortedAssets[2].State);
            Assert.Equal(AssetState.Available, sortedAssets[3].State);
            Assert.Equal(AssetState.Assigned, sortedAssets[4].State);
        }

        [Fact]
        public void ApplySorting_ByCategory_Ascending_SortsCorrectly()
        {
            // Act
            var result = _mockQueryable.ApplySorting(new List<(string Property, string Order)>
            {
                ("category", "asc")
            });

            // Assert
            var sortedAssets = result.ToList();
            Assert.Equal("Electronics", sortedAssets[0].Category.Name);
            Assert.Equal("Electronics", sortedAssets[1].Category.Name);
            Assert.Equal("Furniture", sortedAssets[2].Category.Name);
            Assert.Equal("Furniture", sortedAssets[3].Category.Name);
            Assert.Equal("Software", sortedAssets[4].Category.Name);
        }

        [Fact]
        public void ApplySorting_ByCategory_Descending_SortsCorrectly()
        {
            // Act
            var result = _mockQueryable.ApplySorting(new List<(string Property, string Order)>
            {
                ("category", "desc")
            });

            // Assert
            var sortedAssets = result.ToList();
            Assert.Equal("Software", sortedAssets[0].Category.Name);
            Assert.Equal("Furniture", sortedAssets[1].Category.Name);
            Assert.Equal("Furniture", sortedAssets[2].Category.Name);
            Assert.Equal("Electronics", sortedAssets[3].Category.Name);
            Assert.Equal("Electronics", sortedAssets[4].Category.Name);
        }

        [Fact]
        public void ApplySorting_ByCreated_Ascending_SortsCorrectly()
        {
            // Act
            var result = _mockQueryable.ApplySorting(new List<(string Property, string Order)>
            {
                ("created", "asc")
            });

            // Assert
            var sortedAssets = result.ToList();
            Assert.Equal(new DateTime(2023, 1, 1), sortedAssets[0].CreatedDate);
            Assert.Equal(new DateTime(2023, 1, 3), sortedAssets[1].CreatedDate);
            Assert.Equal(new DateTime(2023, 1, 5), sortedAssets[2].CreatedDate);
            Assert.Equal(new DateTime(2023, 1, 7), sortedAssets[3].CreatedDate);
            Assert.Equal(new DateTime(2023, 1, 9), sortedAssets[4].CreatedDate);
        }

        [Fact]
        public void ApplySorting_ByCreated_Descending_SortsCorrectly()
        {
            // Act
            var result = _mockQueryable.ApplySorting(new List<(string Property, string Order)>
            {
                ("created", "desc")
            });

            // Assert
            var sortedAssets = result.ToList();
            Assert.Equal(new DateTime(2023, 1, 9), sortedAssets[0].CreatedDate);
            Assert.Equal(new DateTime(2023, 1, 7), sortedAssets[1].CreatedDate);
            Assert.Equal(new DateTime(2023, 1, 5), sortedAssets[2].CreatedDate);
            Assert.Equal(new DateTime(2023, 1, 3), sortedAssets[3].CreatedDate);
            Assert.Equal(new DateTime(2023, 1, 1), sortedAssets[4].CreatedDate);
        }

        [Fact]
        public void ApplySorting_ByUpdated_Ascending_SortsCorrectly()
        {
            // Act
            var result = _mockQueryable.ApplySorting(new List<(string Property, string Order)>
            {
                ("updated", "asc")
            });

            // Assert
            var sortedAssets = result.ToList();
            Assert.Equal(new DateTime(2023, 1, 2), sortedAssets[0].LastModifiedDate);
            Assert.Equal(new DateTime(2023, 1, 4), sortedAssets[1].LastModifiedDate);
            Assert.Equal(new DateTime(2023, 1, 6), sortedAssets[2].LastModifiedDate);
            Assert.Equal(new DateTime(2023, 1, 8), sortedAssets[3].LastModifiedDate);
            Assert.Equal(new DateTime(2023, 1, 10), sortedAssets[4].LastModifiedDate);
        }

        [Fact]
        public void ApplySorting_ByUpdated_Descending_SortsCorrectly()
        {
            // Act
            var result = _mockQueryable.ApplySorting(new List<(string Property, string Order)>
            {
                ("updated", "desc")
            });

            // Assert
            var sortedAssets = result.ToList();
            Assert.Equal(new DateTime(2023, 1, 10), sortedAssets[0].LastModifiedDate);
            Assert.Equal(new DateTime(2023, 1, 8), sortedAssets[1].LastModifiedDate);
            Assert.Equal(new DateTime(2023, 1, 6), sortedAssets[2].LastModifiedDate);
            Assert.Equal(new DateTime(2023, 1, 4), sortedAssets[3].LastModifiedDate);
            Assert.Equal(new DateTime(2023, 1, 2), sortedAssets[4].LastModifiedDate);
        }

        [Fact]
        public void ApplySorting_ByUnknownProperty_DefaultsToSortById()
        {
            // Act
            var result = _mockQueryable.ApplySorting(new List<(string Property, string Order)>
            {
                ("unknown", "asc")
            });

            // Assert
            var sortedAssets = result.ToList();
            // Should be sorted by Id (default behavior)
            Assert.Equal(5, sortedAssets.Count);
        }

        [Fact]
        public void ApplySorting_WithMultipleSortingCriteria_SortsCorrectly()
        {
            // Act - Sort by category first, then by name
            var result = _mockQueryable.ApplySorting(new List<(string Property, string Order)>
            {
                ("category", "asc"),
                ("name", "asc")
            });

            // Assert
            var sortedAssets = result.ToList();
            // Electronics category first
            Assert.Equal("Electronics", sortedAssets[0].Category.Name);
            Assert.Equal("Electronics", sortedAssets[1].Category.Name);
            // Within Electronics, sorted by name
            Assert.Equal("Laptop Dell", sortedAssets[0].Name);
            Assert.Equal("Monitor Samsung", sortedAssets[1].Name);

            // Furniture category next
            Assert.Equal("Furniture", sortedAssets[2].Category.Name);
            Assert.Equal("Furniture", sortedAssets[3].Category.Name);
            // Within Furniture, sorted by name
            Assert.Equal("Desk Wooden", sortedAssets[2].Name);
            Assert.Equal("Office Chair", sortedAssets[3].Name);

            // Software category last
            Assert.Equal("Software", sortedAssets[4].Category.Name);
            Assert.Equal("Microsoft Office", sortedAssets[4].Name);
        }

        [Fact]
        public void ApplySorting_WithMultipleSortingCriteria_ThenByDescending_SortsCorrectly()
        {
            // Act - Sort by category ascending, then by name descending
            var result = _mockQueryable.ApplySorting(new List<(string Property, string Order)>
            {
                ("category", "asc"),
                ("name", "desc")
            });

            // Assert
            var sortedAssets = result.ToList();
            // Electronics category first
            Assert.Equal("Electronics", sortedAssets[0].Category.Name);
            Assert.Equal("Electronics", sortedAssets[1].Category.Name);
            // Within Electronics, sorted by name descending
            Assert.Equal("Monitor Samsung", sortedAssets[0].Name);
            Assert.Equal("Laptop Dell", sortedAssets[1].Name);

            // Furniture category next
            Assert.Equal("Furniture", sortedAssets[2].Category.Name);
            Assert.Equal("Furniture", sortedAssets[3].Category.Name);
            // Within Furniture, sorted by name descending
            Assert.Equal("Office Chair", sortedAssets[2].Name);
            Assert.Equal("Desk Wooden", sortedAssets[3].Name);
        }

        [Fact]
        public void ApplySorting_WithMultipleSortingCriteria_UnknownSecondProperty_UsesDefault()
        {
            // Act
            var result = _mockQueryable.ApplySorting(new List<(string Property, string Order)>
            {
                ("category", "asc"),
                ("unknown", "asc")
            });

            // Assert
            var sortedAssets = result.ToList();
            // Should still be sorted by category first, then by default (Id)
            Assert.Equal("Electronics", sortedAssets[0].Category.Name);
            Assert.Equal("Electronics", sortedAssets[1].Category.Name);
            Assert.Equal("Furniture", sortedAssets[2].Category.Name);
            Assert.Equal("Furniture", sortedAssets[3].Category.Name);
            Assert.Equal("Software", sortedAssets[4].Category.Name);
        }
        #endregion
    }
}