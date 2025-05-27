using System.Linq.Expressions;
using AssetManagement.Application.Services;
using AssetManagement.Contracts.Parameters;
using AssetManagement.Domain.Entities;
using AssetManagement.Domain.Enums;
using AssetManagement.Domain.Interfaces.Repositories;
using MockQueryable;
using Moq;

namespace AssetManagement.Application.Tests.Services;

public class AssetServiceTests
{
    private readonly Mock<IAssetRepository> _assetRepository;
    private readonly AssetService _assetService;
    
    public AssetServiceTests()
    {
        _assetRepository = new Mock<IAssetRepository>();
        _assetService = new AssetService(_assetRepository.Object);
    }
    
    [Fact]
    public async Task GetAssetsAsync_ReturnsPagedResult()
    {
        // Arrange
        var queryParams = new AssetQueryParameters
        {
            PageNumber = 1,
            PageSize = 2
        };

        var categoryId = Guid.NewGuid();
        var category = new Category { Id = categoryId, Name = "Electronics" };

        var assets = new List<Asset>
        {
            new Asset { Id = Guid.NewGuid(), Code = "A001", Name = "Laptop A", State = AssetState.Available, Category = category },
            new Asset { Id = Guid.NewGuid(), Code = "A002", Name = "Laptop B", State = AssetState.Available, Category = category }
        };

        // Use MockQueryable to build the mock
        var mockDbSet = assets.AsQueryable().BuildMock();
    
        _assetRepository.Setup(repo => repo.GetAll())
            .Returns(mockDbSet);

        // Act
        var result = await _assetService.GetAssetsAsync(queryParams);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Items.Count());
        Assert.Equal(2, result.PaginationMetadata.TotalItems);
        Assert.Equal(1, result.PaginationMetadata.CurrentPage);
        Assert.Equal(2, result.PaginationMetadata.PageSize);
        Assert.Equal("Electronics", result.Items.First().CategoryName);
        Assert.Equal("Available", result.Items.First().State);
    }

    [Fact]
    public async Task GetAssetAsync_ReturnsAssetDetails_WhenAssetExists()
    {
        // Arrange
        var assetId = Guid.NewGuid();
        var categoryId = Guid.NewGuid();
        
        var asset = new Asset
        {
            Id = assetId,
            Code = "A001",
            Name = "Laptop A",
            State = AssetState.Available,
            Category = new Category { Id = categoryId, Name = "Electronics" },
            InstalledDate = new DateTime(2023, 1, 1),
            Location = Location.HCM,
            Specification = "Intel i7, 16GB RAM"
        };

        _assetRepository.Setup(repo => repo.GetSingleAsync(
                It.IsAny<Expression<Func<Asset, bool>>>(),
                It.IsAny<CancellationToken>(),
                false,
                It.IsAny<Expression<Func<Asset, object>>[]>()))
            .ReturnsAsync(asset);

        // Act
        var result = await _assetService.GetAssetByIdAsync(assetId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(assetId, result.Id);
        Assert.Equal("A001", result.Code);
        Assert.Equal("Laptop A", result.Name);
        Assert.Equal("Available", result.State);
        Assert.Equal("Electronics", result.CategoryName);
        Assert.Equal("HCM", result.Location);
        Assert.Equal(new DateTime(2023, 1, 1), result.InstalledDate);
        Assert.Equal("Intel i7, 16GB RAM", result.Specification);
    }

    [Fact]
    public async Task GetAssetAsync_ThrowsKeyNotFoundException_WhenAssetNotFound()
    {
        // Arrange
        var assetId = Guid.NewGuid();

        _assetRepository.Setup(repo => repo.GetSingleAsync(
                It.IsAny<Expression<Func<Asset, bool>>>(),
                It.IsAny<CancellationToken>(),
                false,
                It.IsAny<Expression<Func<Asset, object>>[]>()))
            .ReturnsAsync((Asset?)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<KeyNotFoundException>(() => _assetService.GetAssetByIdAsync(assetId));
        Assert.Equal($"Cannot find asset with id {assetId}", exception.Message);
    }
    
    [Fact]
    public async Task GetAssetsAsync_WithDifferentStates_ReturnsCorrectDisplayNames()
    {
        // Arrange
        var queryParams = new AssetQueryParameters
        {
            PageNumber = 1,
            PageSize = 5
        };

        var category = new Category { Id = Guid.NewGuid(), Name = "Electronics" };
        
        var assets = new List<Asset>
        {
            new Asset { Id = Guid.NewGuid(), Code = "A001", Name = "Asset Assigned", State = AssetState.Assigned, Category = category },
            new Asset { Id = Guid.NewGuid(), Code = "A002", Name = "Asset Available", State = AssetState.Available, Category = category },
            new Asset { Id = Guid.NewGuid(), Code = "A003", Name = "Asset NotAvailable", State = AssetState.NotAvailable, Category = category },
            new Asset { Id = Guid.NewGuid(), Code = "A004", Name = "Asset WaitingForRecycling", State = AssetState.WaitingForRecycling, Category = category },
            new Asset { Id = Guid.NewGuid(), Code = "A005", Name = "Asset Recycled", State = AssetState.Recycled, Category = category }
        };
        
        // Use MockQueryable to build the mock
        var mockDbSet = assets.AsQueryable().BuildMock();

        _assetRepository.Setup(repo => repo.GetAll())
            .Returns(mockDbSet);

        // Act
        var result = await _assetService.GetAssetsAsync(queryParams);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(5, result.Items.Count());
        Assert.Equal(5, result.PaginationMetadata.TotalItems);
        
        var assignedAsset = result.Items.FirstOrDefault(a => a.Name == "Asset Assigned");
        var availableAsset = result.Items.FirstOrDefault(a => a.Name == "Asset Available");
        var notAvailableAsset = result.Items.FirstOrDefault(a => a.Name == "Asset NotAvailable");
        var waitingForRecyclingAsset = result.Items.FirstOrDefault(a => a.Name == "Asset WaitingForRecycling");
        var recycledAsset = result.Items.FirstOrDefault(a => a.Name == "Asset Recycled");
        
        Assert.NotNull(assignedAsset);
        Assert.NotNull(availableAsset);
        Assert.NotNull(notAvailableAsset);
        Assert.NotNull(waitingForRecyclingAsset);
        Assert.NotNull(recycledAsset);
        
        Assert.Equal("Assigned", assignedAsset.State);
        Assert.Equal("Available", availableAsset.State);
        Assert.Equal("Not available", notAvailableAsset.State);
        Assert.Equal("Waiting for recycling", waitingForRecyclingAsset.State);
        Assert.Equal("Recycled", recycledAsset.State);
    }
}
