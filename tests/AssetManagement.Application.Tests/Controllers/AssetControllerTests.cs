using System.Security.Claims;
using AssetManagement.Application.Controllers;
using AssetManagement.Application.Services.Interfaces;
using AssetManagement.Contracts.Common.Pagination;
using AssetManagement.Contracts.DTOs;
using AssetManagement.Contracts.Parameters;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace AssetManagement.Application.Tests.Controllers;

public class AssetControllerTests
{
    private readonly Mock<IAssetService> _assetServiceMock;
    private readonly AssetController _controller;

    public AssetControllerTests()
    {
        _assetServiceMock = new Mock<IAssetService>();
        _controller = new AssetController(_assetServiceMock.Object);
        
        // Mock HttpContext User
        var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
        {
            new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.Role, "Admin")
        }, "mock"));

        // Initialize HttpContext
        _controller.ControllerContext = new ControllerContext
        {
            
            HttpContext = new DefaultHttpContext { User = user }
        };
    }
    
    [Fact]
    public async Task GetAssetList_ReturnsPagedAssets_WhenAuthorized()
    {
        // Arrange
        var queryParams = new AssetQueryParameters { PageNumber = 1, PageSize = 10 };
        var dtoList = new List<AssetDto> 
        { 
            new AssetDto 
            { 
                AssetCode = "A001", 
                Id = Guid.NewGuid(), 
                Name = "Test Asset", 
                CategoryName = "Test Category", 
                State = "Available" 
            } 
        };
        var expectedPagedResult = new PagedResult<AssetDto>(dtoList, dtoList.Count, queryParams.PageSize, queryParams.PageNumber);
    
        _assetServiceMock.Setup(s => s.GetAssetsAsync(It.Is<AssetQueryParameters>(p => 
                p.PageNumber == queryParams.PageNumber && 
                p.PageSize == queryParams.PageSize)))
            .ReturnsAsync(expectedPagedResult)
            .Verifiable();

        // Act
        var result = await _controller.GetAssetList(queryParams);

        // Assert
        var actionResult = Assert.IsType<ActionResult<ApiResponse<PagedResult<AssetDto>>>>(result);
        var response = Assert.IsType<ApiResponse<PagedResult<AssetDto>>>(actionResult.Value);
    
        Assert.NotNull(response);
        Assert.True(response.Success);
        Assert.Equal("Successfully fetched a paginated list of assets", response.Message);
        Assert.Equal(expectedPagedResult, response.Data);
        Assert.Equal(expectedPagedResult.Items.Count(), response.Data.Items.Count());
    
        _assetServiceMock.Verify();
    }

    [Fact]
    public async Task GetAssetDetail_ReturnsAsset_WhenAuthorized()
    {
        // Arrange
        var assetId = Guid.NewGuid();
        var expectedAsset = new AssetDetailsDto(
            assetId,
            "LP000123",
            "MacBook Pro 2022",
            "Available",
            "Laptop",
            DateTimeOffset.UtcNow.AddYears(1),
            "HN",
            "M1 Pro, 16GB RAM, 512GB SSD"
            );

        _assetServiceMock.Setup(s => s.GetAssetAsync(assetId))
            .ReturnsAsync(expectedAsset)
            .Verifiable();

        // Act
        var response = await _controller.GetAssetById(assetId);

        // Assert
        Assert.NotNull(response);
        Assert.True(response.Success);
        Assert.Equal("Successfully fetched an asset details", response.Message);
        Assert.Equal(expectedAsset, response.Data);
        Assert.Equal(assetId, response.Data.Id);
        Assert.Equal(expectedAsset.AssetCode, response.Data.AssetCode);
        Assert.Equal(expectedAsset.Name, response.Data.Name);

        _assetServiceMock.Verify();
    }
}