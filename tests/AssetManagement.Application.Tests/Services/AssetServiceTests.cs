using System.Linq.Expressions;
using AssetManagement.Application.Services;
using AssetManagement.Contracts.Parameters;
using AssetManagement.Domain.Entities;
using AssetManagement.Domain.Enums;
using AssetManagement.Domain.Repositories;
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
            new Asset { Id = Guid.NewGuid(), AssetCode = "A001", Name = "Laptop A", State = AssetStateEnum.Available, Category = category },
            new Asset { Id = Guid.NewGuid(), AssetCode = "A002", Name = "Laptop B", State = AssetStateEnum.Available, Category = category }
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
            AssetCode = "A001",
            Name = "Laptop A",
            State = AssetStateEnum.Available,
            Category = new Category { Id = categoryId, Name = "Electronics" },
            InstallDate = new DateTime(2023, 1, 1),
            Location = LocationEnum.HCM,
            Specification = "Intel i7, 16GB RAM"
        };

        _assetRepository.Setup(repo => repo.GetSingleAsync(
                It.IsAny<Expression<Func<Asset, bool>>>(),
                It.IsAny<CancellationToken>(),
                false,
                It.IsAny<Expression<Func<Asset, object>>[]>()))
            .ReturnsAsync(asset);

        // Act
        var result = await _assetService.GetAssetAsync(assetId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(assetId, result.Id);
        Assert.Equal("A001", result.AssetCode);
        Assert.Equal("Laptop A", result.Name);
        Assert.Equal("Available", result.State);
        Assert.Equal("Electronics", result.CategoryName);
        Assert.Equal("HCM", result.Location);
        Assert.Equal(new DateTime(2023, 1, 1), result.InstallDate);
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
        var exception = await Assert.ThrowsAsync<KeyNotFoundException>(() => _assetService.GetAssetAsync(assetId));
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
            new Asset { Id = Guid.NewGuid(), AssetCode = "A001", Name = "Asset Assigned", State = AssetStateEnum.Assigned, Category = category },
            new Asset { Id = Guid.NewGuid(), AssetCode = "A002", Name = "Asset Available", State = AssetStateEnum.Available, Category = category },
            new Asset { Id = Guid.NewGuid(), AssetCode = "A003", Name = "Asset NotAvailable", State = AssetStateEnum.NotAvailable, Category = category },
            new Asset { Id = Guid.NewGuid(), AssetCode = "A004", Name = "Asset WaitingForRecycling", State = AssetStateEnum.WaitingForRecycling, Category = category },
            new Asset { Id = Guid.NewGuid(), AssetCode = "A005", Name = "Asset Recycled", State = AssetStateEnum.Recycled, Category = category }
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
        Assert.Equal("NotAvailable", notAvailableAsset.State);
        Assert.Equal("WaitingForRecycling", waitingForRecyclingAsset.State);
        Assert.Equal("Recycled", recycledAsset.State);
    }
}
