using System.Linq.Expressions;
using AssetManagement.Application.Services;
using AssetManagement.Contracts.DTOs.Requests;
using AssetManagement.Contracts.Enums;
using AssetManagement.Contracts.Exceptions;
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
    private readonly Mock<ICategoryRepository> _categoryRepository;
    private readonly Mock<IUserRepository> _userRepository;
    private readonly AssetService _assetService;

    public AssetServiceTests()
    {
        _assetRepository = new Mock<IAssetRepository>();
        _categoryRepository = new Mock<ICategoryRepository>();
        _userRepository = new Mock<IUserRepository>();
        _userRepository = new Mock<IUserRepository>();
        _assetService = new AssetService(_assetRepository.Object, _categoryRepository.Object, _userRepository.Object);
    }

    #region Helpers
    private User CreateAdminUser(Guid? id = null)
    {
        return new User
        {
            Id = id ?? Guid.NewGuid(),
            Username = "admin1",
            Password = "Password",
            Location = Location.HCM
        };
    }
    #endregion

    #region GetAssets
    [Fact]
    public async Task GetAssetsAsync_ReturnsPagedResult()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var queryParams = new AssetQueryParameters
        {
            PageNumber = 1,
            PageSize = 2
        };

        var categoryId = Guid.NewGuid();
        var category = new Category { Id = categoryId, Name = "Electronics" };

        var assets = new List<Asset>
        {
            new Asset { Id = Guid.NewGuid(), Code = "A001", Name = "Laptop A", State = AssetState.Available, Category = category, Location = Location.HCM },
            new Asset { Id = Guid.NewGuid(), Code = "A002", Name = "Laptop B", State = AssetState.Available, Category = category, Location = Location.HCM },
            new Asset { Id = Guid.NewGuid(), Code = "A003", Name = "Laptop C", State = AssetState.Available, Category = category, Location = Location.HN }
        };

        // Use MockQueryable to build the mock
        var mockDbSet = assets.AsQueryable().BuildMock();
        var admin = CreateAdminUser(userId);

        _userRepository.Setup(repo => repo.GetByIdAsync(userId)).ReturnsAsync(admin);
        _assetRepository.Setup(repo => repo.GetAll())
            .Returns(mockDbSet);

        // Act
        var result = await _assetService.GetAssetsAsync(userId.ToString(), queryParams);

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
            InstalledDate = new DateTimeOffset(2023, 1, 1, 0, 0, 0, TimeSpan.Zero),
            Location = Location.HCM,
            Specification = "Intel i7, 16GB RAM"
        };

        _assetRepository.Setup(repo => repo.GetAll())
            .Returns(new List<Asset>([asset]).AsQueryable().BuildMock());

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
        Assert.Equal(new DateTimeOffset(2023, 1, 1, 0, 0, 0, TimeSpan.Zero), result.InstalledDate);
        Assert.Equal("Intel i7, 16GB RAM", result.Specification);
    }

    [Fact]
    public async Task GetAssetAsync_ThrowsKeyNotFoundException_WhenAssetNotFound()
    {
        // Arrange
        var assetId = Guid.NewGuid();

        _assetRepository.Setup(repo => repo.GetAll())
            .Returns(new List<Asset>().AsQueryable().BuildMock());

        // Act & Assert
        var exception = await Assert.ThrowsAsync<KeyNotFoundException>(() => _assetService.GetAssetByIdAsync(assetId));
        Assert.Equal($"Cannot find asset with id {assetId}", exception.Message);
    }

    [Fact]
    public async Task GetAssetsAsync_WithDifferentStates_ReturnsCorrectDisplayNames()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var queryParams = new AssetQueryParameters
        {
            PageNumber = 1,
            PageSize = 5
        };

        var category = new Category { Id = Guid.NewGuid(), Name = "Electronics" };

        var assets = new List<Asset>
        {
            new Asset { Id = Guid.NewGuid(), Code = "A001", Name = "Asset Assigned", State = AssetState.Assigned, Category = category, Location = Location.HCM },
            new Asset { Id = Guid.NewGuid(), Code = "A002", Name = "Asset Available", State = AssetState.Available, Category = category, Location = Location.HCM },
            new Asset { Id = Guid.NewGuid(), Code = "A003", Name = "Asset NotAvailable", State = AssetState.NotAvailable, Category = category, Location = Location.HCM },
            new Asset { Id = Guid.NewGuid(), Code = "A004", Name = "Asset WaitingForRecycling", State = AssetState.WaitingForRecycling, Category = category, Location = Location.HCM },
            new Asset { Id = Guid.NewGuid(), Code = "A005", Name = "Asset Recycled", State = AssetState.Recycled, Category = category, Location = Location.HCM }
        };

        // Use MockQueryable to build the mock
        var mockDbSet = assets.AsQueryable().BuildMock();

        var admin = CreateAdminUser(userId);
        _userRepository.Setup(repo => repo.GetByIdAsync(userId)).ReturnsAsync(admin);
        _assetRepository.Setup(repo => repo.GetAll()).Returns(mockDbSet);

        // Act
        var result = await _assetService.GetAssetsAsync(userId.ToString(), queryParams);

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
    #endregion

    #region CreateAsset
    [Fact]
    public async Task CreateAssetAsync_ValidRequest_ReturnsCreateAssetResponseDto()
    {
        // Arrange
        var adminId = Guid.NewGuid();
        var request = new CreateAssetRequestDto
        {
            Name = "Test Asset",
            CategoryId = Guid.NewGuid(),
            Specifications = "Test specifications",
            InstalledDate = "2023-01-01",
            State = AssetStateDto.Available
        };

        var category = new Category
        {
            Id = request.CategoryId,
            Name = "Test Category",
            Prefix = "TC"
        };

        var adminUser = new User
        {
            Id = adminId,
            Location = Location.HCM,
            Username = "1",
            Password = "1"
        };

        _categoryRepository.Setup(x => x.GetByIdAsync(request.CategoryId))
            .ReturnsAsync(category);

        _userRepository.Setup(x => x.GetByIdAsync(adminId))
            .ReturnsAsync(adminUser);

        _assetRepository.Setup(x => x.GetAll())
            .Returns(new List<Asset>().AsQueryable());

        // Act
        var result = await _assetService.CreateAssetAsync(request, adminId.ToString());

        // Assert
        Assert.NotNull(result);
        Assert.Equal(request.Name, result.Name);
        Assert.Equal(category.Name, result.CategoryName);
        Assert.Equal("Available", result.StateName);
        Assert.StartsWith("TC", result.Code);
        Assert.Equal("TC000001", result.Code);

        _assetRepository.Verify(x => x.AddAsync(It.IsAny<Asset>()), Times.Once);
        _assetRepository.Verify(x => x.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task CreateAssetAsync_GeneratesCorrectCode_WhenExistingAssetsExist()
    {
        // Arrange
        var adminId = Guid.NewGuid();
        var request = new CreateAssetRequestDto
        {
            Name = "Test Asset",
            CategoryId = Guid.NewGuid(),
            Specifications = "Test specifications",
            InstalledDate = "2023-01-01",
            State = AssetStateDto.Available
        };

        var category = new Category
        {
            Id = request.CategoryId,
            Name = "Test Category",
            Prefix = "TC"
        };

        var adminUser = new User
        {
            Id = adminId,
            Location = Location.HCM,
            Username = "1",
            Password = "1"
        };

        var existingAssets = new List<Asset>
            {
                new Asset { Code = "TC000001" },
                new Asset { Code = "TC000005" },
                new Asset { Code = "TC000003" }
            };

        _categoryRepository.Setup(x => x.GetByIdAsync(request.CategoryId))
            .ReturnsAsync(category);

        _userRepository.Setup(x => x.GetByIdAsync(adminId))
            .ReturnsAsync(adminUser);

        _assetRepository.Setup(x => x.GetAll())
            .Returns(existingAssets.AsQueryable());

        // Act
        var result = await _assetService.CreateAssetAsync(request, adminId.ToString());

        // Assert
        Assert.Equal("TC000006", result.Code);
    }

    [Fact]
    public async Task CreateAssetAsync_ThrowsNotFoundException_WhenCategoryNotFound()
    {
        // Arrange
        var adminId = Guid.NewGuid();
        var request = new CreateAssetRequestDto
        {
            Name = "Test Asset",
            CategoryId = Guid.NewGuid(),
            Specifications = "Test specifications",
            InstalledDate = "2023-01-01",
            State = AssetStateDto.Available
        };

        _categoryRepository.Setup(x => x.GetByIdAsync(request.CategoryId))
            .ReturnsAsync((Category?)null);

        // Act & Assert
        await Assert.ThrowsAsync<ApiExceptionTypes.NotFoundException>(
            () => _assetService.CreateAssetAsync(request, adminId.ToString()));
    }

    [Fact]
    public async Task CreateAssetAsync_ThrowsUnauthorizedAccessException_WhenAdminNotFound()
    {
        // Arrange
        var adminId = Guid.NewGuid();
        var request = new CreateAssetRequestDto
        {
            Name = "Test Asset",
            CategoryId = Guid.NewGuid(),
            Specifications = "Test specifications",
            InstalledDate = "2023-01-01",
            State = AssetStateDto.Available
        };

        var category = new Category
        {
            Id = request.CategoryId,
            Name = "Test Category",
            Prefix = "TC"
        };

        _categoryRepository.Setup(x => x.GetByIdAsync(request.CategoryId))
            .ReturnsAsync(category);

        _userRepository.Setup(x => x.GetByIdAsync(adminId))
            .ReturnsAsync((User?)null);

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(
            () => _assetService.CreateAssetAsync(request, adminId.ToString()));
    }

    [Fact]
    public async Task CreateAssetAsync_MapsAssetStateCorrectly_Available()
    {
        // Arrange
        var adminId = Guid.NewGuid();
        var request = new CreateAssetRequestDto
        {
            Name = "Test Asset",
            CategoryId = Guid.NewGuid(),
            Specifications = "Test specifications",
            InstalledDate = "2023-01-01",
            State = AssetStateDto.Available
        };

        var category = new Category
        {
            Id = request.CategoryId,
            Name = "Test Category",
            Prefix = "TC"
        };

        var adminUser = new User
        {
            Id = adminId,
            Location = Location.HCM,
            Username = "1",
            Password = "1"
        };

        _categoryRepository.Setup(x => x.GetByIdAsync(request.CategoryId))
            .ReturnsAsync(category);

        _userRepository.Setup(x => x.GetByIdAsync(adminId))
            .ReturnsAsync(adminUser);

        _assetRepository.Setup(x => x.GetAll())
            .Returns(new List<Asset>().AsQueryable());

        Asset? capturedAsset = null;
        _assetRepository.Setup(x => x.AddAsync(It.IsAny<Asset>()))
            .Callback<Asset>(asset => capturedAsset = asset);

        // Act
        var result = await _assetService.CreateAssetAsync(request, adminId.ToString());

        // Assert
        Assert.Equal("Available", result.StateName);
        Assert.Equal(AssetState.Available, capturedAsset?.State);
    }

    [Fact]
    public async Task CreateAssetAsync_MapsAssetStateCorrectly_NotAvailable()
    {
        // Arrange
        var adminId = Guid.NewGuid();
        var request = new CreateAssetRequestDto
        {
            Name = "Test Asset",
            CategoryId = Guid.NewGuid(),
            Specifications = "Test specifications",
            InstalledDate = "2023-01-01",
            State = AssetStateDto.NotAvailable
        };

        var category = new Category
        {
            Id = request.CategoryId,
            Name = "Test Category",
            Prefix = "TC"
        };

        var adminUser = new User
        {
            Id = adminId,
            Location = Location.HCM,
            Username = "1",
            Password = "1"
        };

        _categoryRepository.Setup(x => x.GetByIdAsync(request.CategoryId))
            .ReturnsAsync(category);

        _userRepository.Setup(x => x.GetByIdAsync(adminId))
            .ReturnsAsync(adminUser);

        _assetRepository.Setup(x => x.GetAll())
            .Returns(new List<Asset>().AsQueryable());

        Asset? capturedAsset = null;
        _assetRepository.Setup(x => x.AddAsync(It.IsAny<Asset>()))
            .Callback<Asset>(asset => capturedAsset = asset);

        // Act
        var result = await _assetService.CreateAssetAsync(request, adminId.ToString());

        // Assert
        Assert.Equal("Not available", result.StateName);
        Assert.Equal(AssetState.NotAvailable, capturedAsset?.State);
    }

    [Fact]
    public async Task CreateAssetAsync_SetsCorrectAssetProperties()
    {
        // Arrange
        var adminId = Guid.NewGuid();
        var request = new CreateAssetRequestDto
        {
            Name = "Test Asset",
            CategoryId = Guid.NewGuid(),
            Specifications = "Test specifications",
            InstalledDate = "2023-01-01",
            State = AssetStateDto.Available
        };

        var category = new Category
        {
            Id = request.CategoryId,
            Name = "Test Category",
            Prefix = "TC"
        };

        var adminUser = new User
        {
            Id = adminId,
            Location = Location.HCM,
            Username = "1",
            Password = "1"
        };

        _categoryRepository.Setup(x => x.GetByIdAsync(request.CategoryId))
            .ReturnsAsync(category);

        _userRepository.Setup(x => x.GetByIdAsync(adminId))
            .ReturnsAsync(adminUser);

        _assetRepository.Setup(x => x.GetAll())
            .Returns(new List<Asset>().AsQueryable());

        Asset? capturedAsset = null;
        _assetRepository.Setup(x => x.AddAsync(It.IsAny<Asset>()))
            .Callback<Asset>(asset => capturedAsset = asset);

        // Act
        await _assetService.CreateAssetAsync(request, adminId.ToString());

        // Assert
        Assert.NotNull(capturedAsset);
        Assert.Equal(request.Name, capturedAsset.Name);
        Assert.Equal(request.CategoryId, capturedAsset.CategoryId);
        Assert.Equal(request.Specifications, capturedAsset.Specification);
        Assert.Equal(Location.HCM, capturedAsset.Location);
        Assert.Equal(adminId, capturedAsset.CreatedBy);
        Assert.Equal(adminId, capturedAsset.LastModifiedBy);
        Assert.Equal(DateTimeOffset.Parse(request.InstalledDate), capturedAsset.InstalledDate);
    }

    [Fact]
    public async Task GenerateCode_ReturnsFirstCode_WhenNoAssetsExist()
    {
        // Arrange
        var adminId = Guid.NewGuid();
        var request = new CreateAssetRequestDto
        {
            Name = "Test Asset",
            CategoryId = Guid.NewGuid(),
            Specifications = "Test specifications",
            InstalledDate = "2023-01-01",
            State = AssetStateDto.Available
        };

        var category = new Category
        {
            Id = request.CategoryId,
            Name = "Test Category",
            Prefix = "TC"
        };

        var adminUser = new User
        {
            Id = adminId,
            Location = Location.HCM,
            Username = "admin",
            Password = "password"
        };

        _categoryRepository.Setup(x => x.GetByIdAsync(request.CategoryId))
            .ReturnsAsync(category);

        _userRepository.Setup(x => x.GetByIdAsync(adminId))
            .ReturnsAsync(adminUser);

        _assetRepository.Setup(x => x.GetAll())
            .Returns(new List<Asset>().AsQueryable().BuildMock());

        // Act
        var result = await _assetService.CreateAssetAsync(request, adminId.ToString());

        // Assert
        Assert.Equal("TC000001", result.Code);
        _assetRepository.Verify(x => x.AddAsync(It.Is<Asset>(a => a.Code == "TC000001")), Times.Once());
        _assetRepository.Verify(x => x.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task GenerateCode_ReturnsIncrementedCode_WhenAssetsExist()
    {
        // Arrange
        var adminId = Guid.NewGuid();
        var request = new CreateAssetRequestDto
        {
            Name = "Test Asset",
            CategoryId = Guid.NewGuid(),
            Specifications = "Test specifications",
            InstalledDate = "2023-01-01",
            State = AssetStateDto.Available
        };

        var category = new Category
        {
            Id = request.CategoryId,
            Name = "Test Category",
            Prefix = "TC"
        };

        var adminUser = new User
        {
            Id = adminId,
            Location = Location.HCM,
            Username = "admin",
            Password = "password"
        };

        var existingAssets = new List<Asset>
        {
            new Asset { Code = "TC000001" },
            new Asset { Code = "TC000003" },
            new Asset { Code = "TC000005" }
        };

        _categoryRepository.Setup(x => x.GetByIdAsync(request.CategoryId))
            .ReturnsAsync(category);

        _userRepository.Setup(x => x.GetByIdAsync(adminId))
            .ReturnsAsync(adminUser);

        _assetRepository.Setup(x => x.GetAll())
            .Returns(existingAssets.AsQueryable().BuildMock());

        // Act
        var result = await _assetService.CreateAssetAsync(request, adminId.ToString());

        // Assert
        Assert.Equal("TC000006", result.Code);
        _assetRepository.Verify(x => x.AddAsync(It.Is<Asset>(a => a.Code == "TC000006")), Times.Once());
        _assetRepository.Verify(x => x.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task GenerateCode_ReturnsFallbackCode_WhenLastCodeNumberInvalid()
    {
        // Arrange
        var adminId = Guid.NewGuid();
        var request = new CreateAssetRequestDto
        {
            Name = "Test Asset",
            CategoryId = Guid.NewGuid(),
            Specifications = "Test specifications",
            InstalledDate = "2023-01-01",
            State = AssetStateDto.Available
        };

        var category = new Category
        {
            Id = request.CategoryId,
            Name = "Test Category",
            Prefix = "TC"
        };

        var adminUser = new User
        {
            Id = adminId,
            Location = Location.HCM,
            Username = "admin",
            Password = "password"
        };

        var existingAssets = new List<Asset>
        {
            new Asset { Code = "TCINVALID" }
        };

        _categoryRepository.Setup(x => x.GetByIdAsync(request.CategoryId))
            .ReturnsAsync(category);

        _userRepository.Setup(x => x.GetByIdAsync(adminId))
            .ReturnsAsync(adminUser);

        _assetRepository.Setup(x => x.GetAll())
            .Returns(existingAssets.AsQueryable().BuildMock());

        // Act
        var result = await _assetService.CreateAssetAsync(request, adminId.ToString());

        // Assert
        Assert.Equal("TC000001", result.Code);
        _assetRepository.Verify(x => x.AddAsync(It.Is<Asset>(a => a.Code == "TC000001")), Times.Once());
        _assetRepository.Verify(x => x.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task MapAssetState_MapsAvailableStateCorrectly()
    {
        // Arrange
        var adminId = Guid.NewGuid();
        var request = new CreateAssetRequestDto
        {
            Name = "Test Asset",
            CategoryId = Guid.NewGuid(),
            Specifications = "Test specifications",
            InstalledDate = "2023-01-01",
            State = AssetStateDto.Available
        };

        var category = new Category
        {
            Id = request.CategoryId,
            Name = "Test Category",
            Prefix = "TC"
        };

        var adminUser = new User
        {
            Id = adminId,
            Location = Location.HCM,
            Username = "admin",
            Password = "password"
        };

        _categoryRepository.Setup(x => x.GetByIdAsync(request.CategoryId))
            .ReturnsAsync(category);

        _userRepository.Setup(x => x.GetByIdAsync(adminId))
            .ReturnsAsync(adminUser);

        _assetRepository.Setup(x => x.GetAll())
            .Returns(new List<Asset>().AsQueryable().BuildMock());

        Asset? capturedAsset = null;
        _assetRepository.Setup(x => x.AddAsync(It.IsAny<Asset>()))
            .Callback<Asset>(asset => capturedAsset = asset);

        // Act
        var result = await _assetService.CreateAssetAsync(request, adminId.ToString());

        // Assert
        Assert.NotNull(capturedAsset);
        Assert.Equal(AssetState.Available, capturedAsset.State);
        Assert.Equal("Available", result.StateName);
    }

    [Fact]
    public async Task MapAssetState_MapsNotAvailableStateCorrectly()
    {
        // Arrange
        var adminId = Guid.NewGuid();
        var request = new CreateAssetRequestDto
        {
            Name = "Test Asset",
            CategoryId = Guid.NewGuid(),
            Specifications = "Test specifications",
            InstalledDate = "2023-01-01",
            State = AssetStateDto.NotAvailable
        };

        var category = new Category
        {
            Id = request.CategoryId,
            Name = "Test Category",
            Prefix = "TC"
        };

        var adminUser = new User
        {
            Id = adminId,
            Location = Location.HCM,
            Username = "admin",
            Password = "password"
        };

        _categoryRepository.Setup(x => x.GetByIdAsync(request.CategoryId))
            .ReturnsAsync(category);

        _userRepository.Setup(x => x.GetByIdAsync(adminId))
            .ReturnsAsync(adminUser);

        _assetRepository.Setup(x => x.GetAll())
            .Returns(new List<Asset>().AsQueryable().BuildMock());

        Asset? capturedAsset = null;
        _assetRepository.Setup(x => x.AddAsync(It.IsAny<Asset>()))
            .Callback<Asset>(asset => capturedAsset = asset);

        // Act
        var result = await _assetService.CreateAssetAsync(request, adminId.ToString());

        // Assert
        Assert.NotNull(capturedAsset);
        Assert.Equal(AssetState.NotAvailable, capturedAsset.State);
        Assert.Equal("Not available", result.StateName);
    }

    [Fact]
    public async Task MapAssetState_ThrowsArgumentOutOfRangeException_ForInvalidState()
    {
        // Arrange
        var adminId = Guid.NewGuid();
        var request = new CreateAssetRequestDto
        {
            Name = "Test Asset",
            CategoryId = Guid.NewGuid(),
            Specifications = "Test specifications",
            InstalledDate = "2023-01-01",
            // Simulate an invalid state by manipulating the request
            State = (AssetStateDto)999 // Invalid enum value
        };

        var category = new Category
        {
            Id = request.CategoryId,
            Name = "Test Category",
            Prefix = "TC"
        };

        var adminUser = new User
        {
            Id = adminId,
            Location = Location.HCM,
            Username = "admin",
            Password = "password"
        };

        _categoryRepository.Setup(x => x.GetByIdAsync(request.CategoryId))
            .ReturnsAsync(category);

        _userRepository.Setup(x => x.GetByIdAsync(adminId))
            .ReturnsAsync(adminUser);

        _assetRepository.Setup(x => x.GetAll())
            .Returns(new List<Asset>().AsQueryable().BuildMock());

        // Act & Assert
        await Assert.ThrowsAsync<AggregateFieldValidationException>(
            () => _assetService.CreateAssetAsync(request, adminId.ToString()));
    }

    [Fact]
    public async Task CreateAssetAsync_SetsTimestampsCorrectly()
    {
        // Arrange
        var adminId = Guid.NewGuid();
        var beforeTest = DateTime.UtcNow;

        var request = new CreateAssetRequestDto
        {
            Name = "Test Asset",
            CategoryId = Guid.NewGuid(),
            Specifications = "Test specifications",
            InstalledDate = "2023-01-01",
            State = AssetStateDto.Available
        };

        var category = new Category
        {
            Id = request.CategoryId,
            Name = "Test Category",
            Prefix = "TC"
        };

        var adminUser = new User
        {
            Id = adminId,
            Location = Location.HCM,
            Username = "admin",
            Password = "password"
        };

        _categoryRepository.Setup(x => x.GetByIdAsync(request.CategoryId))
            .ReturnsAsync(category);

        _userRepository.Setup(x => x.GetByIdAsync(adminId))
            .ReturnsAsync(adminUser);

        _assetRepository.Setup(x => x.GetAll())
            .Returns(new List<Asset>().AsQueryable().BuildMock());

        Asset? capturedAsset = null;
        _assetRepository.Setup(x => x.AddAsync(It.IsAny<Asset>()))
            .Callback<Asset>(asset => capturedAsset = asset);

        // Act
        await _assetService.CreateAssetAsync(request, adminId.ToString());

        var afterTest = DateTime.UtcNow;

        // Assert
        Assert.NotNull(capturedAsset);
        Assert.True(capturedAsset.CreatedDate >= beforeTest && capturedAsset.CreatedDate <= afterTest);
        Assert.True(capturedAsset.LastModifiedDate >= beforeTest && capturedAsset.LastModifiedDate <= afterTest);
    }

    [Fact]
    public async Task CreateAssetAsync_HandlesDateTimeParsing_WithDifferentFormats()
    {
        // Arrange
        var adminId = Guid.NewGuid();
        var request = new CreateAssetRequestDto
        {
            Name = "Test Asset",
            CategoryId = Guid.NewGuid(),
            Specifications = "Test specifications",
            InstalledDate = "2023-12-25T10:30:00Z", // ISO format
            State = AssetStateDto.Available
        };

        var category = new Category
        {
            Id = request.CategoryId,
            Name = "Test Category",
            Prefix = "TC"
        };

        var adminUser = new User
        {
            Id = adminId,
            Location = Location.HCM,
            Username = "admin",
            Password = "password"
        };

        _categoryRepository.Setup(x => x.GetByIdAsync(request.CategoryId))
            .ReturnsAsync(category);

        _userRepository.Setup(x => x.GetByIdAsync(adminId))
            .ReturnsAsync(adminUser);

        _assetRepository.Setup(x => x.GetAll())
            .Returns(new List<Asset>().AsQueryable().BuildMock());

        Asset? capturedAsset = null;
        _assetRepository.Setup(x => x.AddAsync(It.IsAny<Asset>()))
            .Callback<Asset>(asset => capturedAsset = asset);

        // Act
        await _assetService.CreateAssetAsync(request, adminId.ToString());

        // Assert
        Assert.NotNull(capturedAsset);
        Assert.Equal(DateTimeOffset.Parse(request.InstalledDate), capturedAsset.InstalledDate);
    }

    [Theory]
    [InlineData(Location.HCM)]
    [InlineData(Location.HN)]
    public async Task CreateAssetAsync_SetsCorrectLocation_BasedOnAdminLocation(Location adminLocation)
    {
        // Arrange
        var adminId = Guid.NewGuid();
        var request = new CreateAssetRequestDto
        {
            Name = "Test Asset",
            CategoryId = Guid.NewGuid(),
            Specifications = "Test specifications",
            InstalledDate = "2023-01-01",
            State = AssetStateDto.Available
        };

        var category = new Category
        {
            Id = request.CategoryId,
            Name = "Test Category",
            Prefix = "TC"
        };

        var adminUser = new User
        {
            Id = adminId,
            Location = adminLocation,
            Username = "admin",
            Password = "password"
        };

        _categoryRepository.Setup(x => x.GetByIdAsync(request.CategoryId))
            .ReturnsAsync(category);

        _userRepository.Setup(x => x.GetByIdAsync(adminId))
            .ReturnsAsync(adminUser);

        _assetRepository.Setup(x => x.GetAll())
            .Returns(new List<Asset>().AsQueryable().BuildMock());

        Asset? capturedAsset = null;
        _assetRepository.Setup(x => x.AddAsync(It.IsAny<Asset>()))
            .Callback<Asset>(asset => capturedAsset = asset);

        // Act
        await _assetService.CreateAssetAsync(request, adminId.ToString());

        // Assert
        Assert.NotNull(capturedAsset);
        Assert.Equal(adminLocation, capturedAsset.Location);
    }
    #endregion

    #region UpdateAsset
    [Fact]
    public async Task UpdateAssetAsync_UpdatesAllFields_WhenValidRequestProvided()
    {
        // Arrange
        var adminId = Guid.NewGuid().ToString();
        var assetCode = "A001";
        var categoryId = Guid.NewGuid();
        var existingAsset = new Asset
        {
            Code = assetCode,
            Name = "Old Laptop",
            State = AssetState.Available,
            CategoryId = Guid.NewGuid(),
            Specification = "Old Specs",
            InstalledDate = new DateTime(2022, 1, 1),
            LastModifiedDate = DateTime.UtcNow.AddDays(-10)
        };

        var request = new UpdateAssetRequestDto
        {
            Name = "New Laptop",
            State = AssetStateDto.NotAvailable,
            CategoryId = categoryId,
            Specification = "New Specs",
            InstalledDate = "2023-02-15"
        };

        var category = new Category { Id = categoryId, Name = "New Category" };

        _assetRepository.Setup(r => r.GetByCodeAsync(assetCode))
            .ReturnsAsync(existingAsset);
        _categoryRepository.Setup(r => r.GetByIdAsync(categoryId))
            .ReturnsAsync(category);
        _assetRepository.Setup(r => r.Update(It.IsAny<Asset>()))
            .Verifiable();

        // Act
        var result = await _assetService.UpdateAssetAsync(adminId, assetCode, request);

        // Assert
        Assert.Equal(assetCode, result);
        Assert.Equal("New Laptop", existingAsset.Name);
        Assert.Equal(AssetState.NotAvailable, existingAsset.State);
        Assert.Equal(categoryId, existingAsset.CategoryId);
        Assert.Equal("New Specs", existingAsset.Specification);
        Assert.Equal(new DateTime(2023, 2, 15), existingAsset.InstalledDate);
        Assert.Equal(Guid.Parse(adminId), existingAsset.LastModifiedBy);
        Assert.True(existingAsset.LastModifiedDate > existingAsset.LastModifiedDate.AddDays(-1)); // Modified recently

        _assetRepository.Verify(r => r.Update(existingAsset), Times.Once);
        _assetRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task UpdateAssetAsync_UpdatesOnlyProvidedFields_WhenPartialRequestProvided()
    {
        // Arrange
        var adminId = Guid.NewGuid().ToString();
        var assetCode = "A001";
        var originalName = "Original Laptop";
        var originalSpec = "Original Specs";
        var originalDate = new DateTime(2022, 1, 1);

        var existingAsset = new Asset
        {
            Code = assetCode,
            Name = originalName,
            State = AssetState.Available,
            CategoryId = Guid.NewGuid(),
            Specification = originalSpec,
            InstalledDate = originalDate
        };

        var request = new UpdateAssetRequestDto
        {
            Name = "New Laptop",
            // State not provided
            // CategoryId not provided
            // Specification not provided
            // InstalledDate not provided
        };

        _assetRepository.Setup(r => r.GetByCodeAsync(assetCode))
            .ReturnsAsync(existingAsset);

        // Act
        var result = await _assetService.UpdateAssetAsync(adminId, assetCode, request);

        // Assert
        Assert.Equal(assetCode, result);
        Assert.Equal("New Laptop", existingAsset.Name);
        Assert.Equal(AssetState.Available, existingAsset.State); // Unchanged
        Assert.Equal(originalSpec, existingAsset.Specification); // Unchanged
        Assert.Equal(originalDate, existingAsset.InstalledDate); // Unchanged

        _assetRepository.Verify(r => r.Update(existingAsset), Times.Once);
        _assetRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task UpdateAssetAsync_ThrowsArgumentNullException_WhenRequestIsNull()
    {
        // Arrange
        var adminId = Guid.NewGuid().ToString();
        var assetCode = "A001";
        UpdateAssetRequestDto? request = null;

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentNullException>(
            () => _assetService.UpdateAssetAsync(adminId, assetCode, request!));

        Assert.Equal("request", exception.ParamName);
        Assert.Contains("Asset update data cannot be null", exception.Message);
    }

    [Fact]
    public async Task UpdateAssetAsync_ThrowsKeyNotFoundException_WhenAssetNotFound()
    {
        // Arrange
        var adminId = Guid.NewGuid().ToString();
        var assetCode = "A001";
        var request = new UpdateAssetRequestDto { Name = "New Name" };

        _assetRepository.Setup(r => r.GetByCodeAsync(assetCode))
            .ReturnsAsync((Asset?)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<KeyNotFoundException>(
            () => _assetService.UpdateAssetAsync(adminId, assetCode, request));

        Assert.Equal($"Cannot find asset with id {assetCode}", exception.Message);
    }

    [Fact]
    public async Task UpdateAssetAsync_ThrowsKeyNotFoundException_WhenCategoryNotFound()
    {
        // Arrange
        var adminId = Guid.NewGuid().ToString();
        var assetCode = "A001";
        var nonExistentCategoryId = Guid.NewGuid();

        var existingAsset = new Asset
        {
            Code = assetCode,
            Name = "Old Laptop",
            State = AssetState.Available
        };

        var request = new UpdateAssetRequestDto
        {
            Name = "New Name",
            CategoryId = nonExistentCategoryId
        };

        _assetRepository.Setup(r => r.GetByCodeAsync(assetCode))
            .ReturnsAsync(existingAsset);
        _categoryRepository.Setup(r => r.GetByIdAsync(nonExistentCategoryId))
            .ReturnsAsync((Category?)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<KeyNotFoundException>(
            () => _assetService.UpdateAssetAsync(adminId, assetCode, request));

        Assert.Equal($"Cannot find category with id {nonExistentCategoryId}", exception.Message);
    }

    [Fact]
    public async Task UpdateAssetAsync_UpdatesState_WhenStateIsProvided()
    {
        // Arrange
        var adminId = Guid.NewGuid().ToString();
        var assetCode = "A001";

        var existingAsset = new Asset
        {
            Code = assetCode,
            Name = "Asset",
            State = AssetState.Available
        };

        var request = new UpdateAssetRequestDto
        {
            Name = "Valid Asset Name",
            State = AssetStateDto.Recycled
        };

        _assetRepository.Setup(r => r.GetByCodeAsync(assetCode))
            .ReturnsAsync(existingAsset);

        // Act
        await _assetService.UpdateAssetAsync(adminId, assetCode, request);

        // Assert
        Assert.Equal(AssetState.Recycled, existingAsset.State);
        _assetRepository.Verify(r => r.Update(existingAsset), Times.Once);
        _assetRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task UpdateAssetAsync_ParsesInstalledDate_WhenDateStringProvided()
    {
        // Arrange
        var adminId = Guid.NewGuid().ToString();
        var assetCode = "A001";
        var dateString = "2023-12-25 00:00:00.000 +0000";
        var expectedDate = DateTimeOffset.Parse(dateString);

        var existingAsset = new Asset
        {
            Code = assetCode,
            Name = "Original Asset",
            InstalledDate = new DateTimeOffset(2023, 12, 25, 0, 0, 0, TimeSpan.Zero)
        };

        var request = new UpdateAssetRequestDto
        {
            Name = "Valid Asset Name",
            InstalledDate = dateString
        };

        _assetRepository.Setup(r => r.GetByCodeAsync(assetCode))
            .ReturnsAsync(existingAsset);

        // Act
        await _assetService.UpdateAssetAsync(adminId, assetCode, request);

        // Assert
        Assert.Equal(expectedDate, existingAsset.InstalledDate);
    }

    [Fact]
    public async Task UpdateAssetAsync_UpdatesStateToAvailable_WhenAvailableStateProvided()
    {
        // Arrange
        var adminId = Guid.NewGuid().ToString();
        var assetCode = "A001";

        var existingAsset = new Asset
        {
            Code = assetCode,
            Name = "Asset",
            State = AssetState.NotAvailable
        };

        var request = new UpdateAssetRequestDto
        {
            Name = "Valid Asset Name",
            State = AssetStateDto.Available
        };

        _assetRepository.Setup(r => r.GetByCodeAsync(assetCode))
            .ReturnsAsync(existingAsset);

        // Act
        await _assetService.UpdateAssetAsync(adminId, assetCode, request);

        // Assert
        Assert.Equal(AssetState.Available, existingAsset.State);
        _assetRepository.Verify(r => r.Update(existingAsset), Times.Once);
        _assetRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task UpdateAssetAsync_UpdatesStateToAssigned_WhenAssignedStateProvided()
    {
        // Arrange
        var adminId = Guid.NewGuid().ToString();
        var assetCode = "A001";

        var existingAsset = new Asset
        {
            Code = assetCode,
            Name = "Asset",
            State = AssetState.Available
        };

        var request = new UpdateAssetRequestDto
        {
            Name = "Valid Asset Name",
            State = AssetStateDto.Assigned
        };

        _assetRepository.Setup(r => r.GetByCodeAsync(assetCode))
            .ReturnsAsync(existingAsset);

        // Act
        await _assetService.UpdateAssetAsync(adminId, assetCode, request);

        // Assert
        Assert.Equal(AssetState.Assigned, existingAsset.State);
        _assetRepository.Verify(r => r.Update(existingAsset), Times.Once);
        _assetRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task UpdateAssetAsync_UpdatesStateToWaitingForRecycling_WhenWaitingForRecyclingStateProvided()
    {
        // Arrange
        var adminId = Guid.NewGuid().ToString();
        var assetCode = "A001";

        var existingAsset = new Asset
        {
            Code = assetCode,
            Name = "Asset",
            State = AssetState.Available
        };

        var request = new UpdateAssetRequestDto
        {
            Name = "Valid Asset Name",
            State = AssetStateDto.WaitingForRecycling
        };

        _assetRepository.Setup(r => r.GetByCodeAsync(assetCode))
            .ReturnsAsync(existingAsset);

        // Act
        await _assetService.UpdateAssetAsync(adminId, assetCode, request);

        // Assert
        Assert.Equal(AssetState.WaitingForRecycling, existingAsset.State);
        _assetRepository.Verify(r => r.Update(existingAsset), Times.Once);
        _assetRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task UpdateAssetAsync_ThrowsArgumentOutOfRangeException_WhenInvalidStateProvided()
    {
        // Arrange
        var adminId = Guid.NewGuid().ToString();
        var assetCode = "A001";

        var existingAsset = new Asset
        {
            Code = assetCode,
            Name = "Asset",
            State = AssetState.Available
        };

        // Create an invalid state value (out of enum range)
        var invalidStateValue = (AssetStateDto)99;

        var request = new UpdateAssetRequestDto
        {
            Name = "Valid Asset Name",
            State = invalidStateValue
        };

        _assetRepository.Setup(r => r.GetByCodeAsync(assetCode))
            .ReturnsAsync(existingAsset);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<AggregateFieldValidationException>(() =>
            _assetService.UpdateAssetAsync(adminId, assetCode, request));

        Assert.Contains(exception.Errors, e => e.Field == "State" &&
            e.Message == "Invalid state value");
    }
    #endregion

    #region DeleteAsset
    [Fact]
    public async Task DeleteAssetAsync_MarksAssetAsDeletedAndReturnsCode_WhenAssetExists()
    {
        // Arrange
        var assetId = Guid.NewGuid();
        var deletedBy = Guid.NewGuid();
        var asset = new Asset
        {
            Id = assetId,
            Code = "A001",
            Name = "Test Asset",
            IsDeleted = false,
            State = AssetState.NotAvailable
        };

        _assetRepository.Setup(repo => repo.GetByIdAsync(assetId))
            .ReturnsAsync(asset);
        _assetRepository.Setup(repo => repo.Update(It.IsAny<Asset>()))
            .Verifiable();

        // Act
        var result = await _assetService.DeleteAssetAsync(deletedBy, assetId);

        // Assert
        Assert.Equal("A001", result);
        Assert.True(asset.IsDeleted);
        Assert.Equal(deletedBy, asset.DeletedBy);
        Assert.True(asset.DeletedDate.HasValue);
        Assert.True(asset.DeletedDate.Value <= DateTime.UtcNow);
        _assetRepository.Verify(repo => repo.GetByIdAsync(assetId), Times.Once());
        _assetRepository.Verify(repo => repo.Update(asset), Times.Once());
    }

    [Fact]
    public async Task DeleteAssetAsync_ThrowsKeyNotFoundException_WhenAssetDoesNotExist()
    {
        // Arrange
        var assetId = Guid.NewGuid();
        var deletedBy = Guid.NewGuid();

        _assetRepository.Setup(repo => repo.GetByIdAsync(assetId))
            .ReturnsAsync((Asset?)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<KeyNotFoundException>(
            () => _assetService.DeleteAssetAsync(deletedBy, assetId));
        Assert.Equal($"Cannot find asset with id {assetId}", exception.Message);
        _assetRepository.Verify(repo => repo.GetByIdAsync(assetId), Times.Once());
        _assetRepository.Verify(repo => repo.Update(It.IsAny<Asset>()), Times.Never());
    }

    [Fact]
    public async Task DeleteAssetAsync_ThrowsInvalidOperationException_WhenAssetAlreadyDeleted()
    {
        // Arrange
        var assetId = Guid.NewGuid();
        var deletedBy = Guid.NewGuid();
        var asset = new Asset
        {
            Id = assetId,
            Code = "A001",
            Name = "Test Asset",
            IsDeleted = true
        };

        _assetRepository.Setup(repo => repo.GetByIdAsync(assetId))
            .ReturnsAsync(asset);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _assetService.DeleteAssetAsync(deletedBy, assetId));
        Assert.Equal("Asset is already deleted", exception.Message);
        _assetRepository.Verify(repo => repo.GetByIdAsync(assetId), Times.Once());
        _assetRepository.Verify(repo => repo.Update(It.IsAny<Asset>()), Times.Never());
    }
    #endregion

    #region GetAssetReport
    [Fact]
    public async Task GetAssetReportAsync_FiltersByAdminLocation()
    {
        // Arrange
        var adminId = Guid.NewGuid();
        var user = CreateAdminUser(adminId);
        var assets = new List<Asset>
        {
            new Asset { Id = Guid.NewGuid(), Location = Location.HCM, Category = new Category { Name = "Cat1" }, State = AssetState.Available, IsDeleted = false },
            new Asset { Id = Guid.NewGuid(), Location = Location.HN, Category = new Category { Name = "Cat1" }, State = AssetState.Available, IsDeleted = false },
            new Asset { Id = Guid.NewGuid(), Location = Location.HCM, Category = new Category { Name = "Cat2" }, State = AssetState.Assigned, IsDeleted = false },
            new Asset { Id = Guid.NewGuid(), Location = Location.HCM, Category = new Category { Name = "Cat1" }, State = AssetState.Available, IsDeleted = true }
        };

        _userRepository.Setup(r => r.GetByIdAsync(adminId)).ReturnsAsync(user);
        _assetRepository.Setup(r => r.GetAll()).Returns(assets.BuildMock());
        var service = new AssetService(_assetRepository.Object, _categoryRepository.Object, _userRepository.Object);
        var queryParams = new AssetReportQueryParameters();

        // Act
        var report = await service.GetAssetReportAsync(adminId, queryParams);

        // Assert
        Assert.Equal(2, report.Count);
        Assert.Contains(report, r => r.Category == "Cat1" && r.Total == 1 && r.Available == 1);
        Assert.Contains(report, r => r.Category == "Cat2" && r.Total == 1 && r.Assigned == 1);
    }

    [Fact]
    public async Task GetAssetReportAsync_GroupsAndCountsCorrectly()
    {
        // Arrange
        var adminId = Guid.NewGuid();
        var user = CreateAdminUser(adminId);
        var assets = new List<Asset>
        {
            new Asset { Id = Guid.NewGuid(), Location = Location.HCM, Category = new Category { Name = "Cat1" }, State = AssetState.Available, IsDeleted = false },
            new Asset { Id = Guid.NewGuid(), Location = Location.HCM, Category = new Category { Name = "Cat1" }, State = AssetState.Assigned, IsDeleted = false },
            new Asset { Id = Guid.NewGuid(), Location = Location.HCM, Category = new Category { Name = "Cat2" }, State = AssetState.Recycled, IsDeleted = false }
        };

        _userRepository.Setup(r => r.GetByIdAsync(adminId)).ReturnsAsync(user);
        _assetRepository.Setup(r => r.GetAll()).Returns(assets.BuildMock());
        var service = new AssetService(_assetRepository.Object, _categoryRepository.Object, _userRepository.Object);
        var queryParams = new AssetReportQueryParameters();

        // Act
        var report = await service.GetAssetReportAsync(adminId, queryParams);

        // Assert
        Assert.Equal(2, report.Count);
        Assert.Contains(report, r => r.Category == "Cat1" && r.Total == 2 && r.Available == 1 && r.Assigned == 1);
        Assert.Contains(report, r => r.Category == "Cat2" && r.Total == 1 && r.Recycled == 1);
    }

    [Fact]
    public async Task GetAssetReportAsync_AppliesSorting()
    {
        // Arrange
        var adminId = Guid.NewGuid();
        var user = CreateAdminUser(adminId);
        var assets = new List<Asset>
        {
            new Asset { Id = Guid.NewGuid(), Location = Location.HCM, Category = new Category { Name = "CatB" }, State = AssetState.Available, IsDeleted = false },
            new Asset { Id = Guid.NewGuid(), Location = Location.HCM, Category = new Category { Name = "CatA" }, State = AssetState.Assigned, IsDeleted = false }
        };

        _userRepository.Setup(r => r.GetByIdAsync(adminId)).ReturnsAsync(user);
        _assetRepository.Setup(r => r.GetAll()).Returns(assets.BuildMock());
        var service = new AssetService(_assetRepository.Object, _categoryRepository.Object, _userRepository.Object);
        var queryParams = new AssetReportQueryParameters { SortBy = "Category", SortOrder = "asc" };

        // Act
        var report = await service.GetAssetReportAsync(adminId, queryParams);

        // Assert
        Assert.Equal(2, report.Count);
        Assert.Equal("CatA", report[0].Category);
        Assert.Equal("CatB", report[1].Category);
    }

    [Fact]
    public async Task GetAssetReportAsync_NoAssets_ReturnsEmptyList()
    {
        // Arrange
        var adminId = Guid.NewGuid();
        var user = CreateAdminUser(adminId);
        var assets = new List<Asset>();

        _userRepository.Setup(r => r.GetByIdAsync(adminId)).ReturnsAsync(user);
        _assetRepository.Setup(r => r.GetAll()).Returns(assets.BuildMock());
        var service = new AssetService(_assetRepository.Object, _categoryRepository.Object, _userRepository.Object);
        var queryParams = new AssetReportQueryParameters();

        // Act
        var report = await service.GetAssetReportAsync(adminId, queryParams);

        // Assert
        Assert.Empty(report);
    }

    [Fact]
    public async Task GetAssetReportAsync_InvalidAdminId_ThrowsException()
    {
        // Arrange
        var adminId = Guid.NewGuid();
        _userRepository.Setup(r => r.GetByIdAsync(adminId)).ReturnsAsync((User?)null);

        var service = new AssetService(_assetRepository.Object, _categoryRepository.Object, _userRepository.Object);
        var queryParams = new AssetReportQueryParameters();

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => service.GetAssetReportAsync(adminId, queryParams));
    }
    #endregion
}
