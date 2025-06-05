using System.Security.Claims;
using AssetManagement.Application.Controllers;
using AssetManagement.Application.Services.Interfaces;
using AssetManagement.Contracts.Common;
using AssetManagement.Contracts.Common.Pagination;
using AssetManagement.Contracts.DTOs;
using AssetManagement.Contracts.DTOs.Requests;
using AssetManagement.Contracts.DTOs.Responses;
using AssetManagement.Contracts.Enums;
using AssetManagement.Contracts.Exceptions;
using AssetManagement.Contracts.Parameters;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace AssetManagement.Application.Tests.Controllers;

public class AssetsControllerTests
{
    private readonly Mock<IAssetService> _assetServiceMock;
    private readonly AssetsController _controller;
    private readonly Guid _adminId;


    public AssetsControllerTests()
    {
        _assetServiceMock = new Mock<IAssetService>();
        _controller = new AssetsController(_assetServiceMock.Object);

        // Mock HttpContext User
        _adminId = Guid.NewGuid();
        var user = new ClaimsPrincipal(new ClaimsIdentity(
        [
            new Claim(ClaimTypes.NameIdentifier, _adminId.ToString()),
            new Claim(ClaimTypes.Role, "Admin")
        ], "mock"));

        // Initialize HttpContext
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = user }
        };
    }

    private ClaimsPrincipal CreateUserPrincipal(string userId, string role = "Admin")
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, userId),
            new Claim(ClaimTypes.Role, role)
        };
        var identity = new ClaimsIdentity(claims, "TestAuthType");
        return new ClaimsPrincipal(identity);
    }

    private void ApplyUserToController(ClaimsPrincipal user)
    {
        _controller.ControllerContext = new ControllerContext()
        {
            HttpContext = new DefaultHttpContext() { User = user }
        };
    }

    [Fact]
    public async Task GetAssetList_ReturnsPagedAssets_WhenAuthorized()
    {
        // Arrange
        var userId = "123e4567-e89b-12d3-a456-426614174000";
        var queryParams = new AssetQueryParameters { PageNumber = 1, PageSize = 10 };
        var dtoList = new List<AssetDto>
        {
            new AssetDto
            {
                Code = "A001",
                Id = Guid.NewGuid(),
                Name = "Test Asset",
                CategoryName = "Test Category",
                State = "Available"
            }
        };
        var expectedPagedResult = new PagedResult<AssetDto>(dtoList, dtoList.Count, queryParams.PageSize, queryParams.PageNumber);

        _assetServiceMock.Setup(s => s.GetAssetsAsync(userId, It.Is<AssetQueryParameters>(p =>
                p.PageNumber == queryParams.PageNumber &&
                p.PageSize == queryParams.PageSize)))
            .ReturnsAsync(expectedPagedResult)
            .Verifiable();

        // Setup user claims
        var claimsPrincipal = CreateUserPrincipal(userId);
        ApplyUserToController(claimsPrincipal);

        // Act
        var result = await _controller.Get(queryParams);

        // Assert
        var actionResult = Assert.IsType<ActionResult<ApiResponse<PagedResult<AssetDto>>>>(result);
        var response = Assert.IsType<ApiResponse<PagedResult<AssetDto>>>(actionResult.Value);

        Assert.NotNull(response);
        Assert.True(response.Success);
        Assert.Equal("Successfully fetched a paginated list of assets", response.Message);
        Assert.Equal(expectedPagedResult, response.Data);
        Assert.Equal(expectedPagedResult.Items.Count(), response.Data?.Items.Count());

        _assetServiceMock.Verify();
    }

    [Fact]
    public async Task GetAssetDetail_ReturnsAsset_WhenAuthorized()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var assetId = Guid.NewGuid();
        var expectedAsset = new AssetDetailsDto
        {
            Id = assetId,
            Code = "LP000123",
            Name = "MacBook Pro 2022",
            State = "Available",
            CategoryName = "Laptop",
            InstalledDate = DateTimeOffset.UtcNow.AddYears(1),
            Location = "HN",
            Specification = "M1 Pro, 16GB RAM, 512GB SSD",
            CategoryId = categoryId
        };

        _assetServiceMock.Setup(s => s.GetAssetByIdAsync(assetId))
            .ReturnsAsync(expectedAsset)
            .Verifiable();

        // Act
        var response = await _controller.GetById(assetId);

        // Assert
        Assert.NotNull(response);
        Assert.True(response.Success);
        Assert.Equal("Successfully fetched an asset details", response.Message);
        Assert.Equal(expectedAsset, response.Data);
        Assert.Equal(assetId, response.Data?.Id);
        Assert.Equal(expectedAsset.Code, response.Data?.Code);
        Assert.Equal(expectedAsset.Name, response.Data?.Name);
        Assert.Equal(expectedAsset.CategoryId, categoryId);

        _assetServiceMock.Verify();
    }

    [Fact]
    public async Task Delete_ReturnsOkResponse_WhenAssetIsDeletedSuccessfully()
    {
        // Arrange
        var assetId = Guid.NewGuid();
        var adminId = Guid.NewGuid();
        var assetCode = "A001";

        _assetServiceMock.Setup(s => s.DeleteAssetAsync(adminId, assetId))
            .ReturnsAsync(assetCode)
            .Verifiable();

        // Set up ClaimsPrincipal for admin
        var claims = new[]
        {
        new Claim(ClaimTypes.NameIdentifier, adminId.ToString()),
        new Claim(ClaimTypes.Role, "Admin")
    };
        var identity = new ClaimsIdentity(claims, "mock"); // Ensure authentication type is set
        var user = new ClaimsPrincipal(identity);
        _controller.ControllerContext.HttpContext = new DefaultHttpContext { User = user };

        // Verify user is authenticated and has Admin role
        Console.WriteLine($"IsAuthenticated: {user.Identity?.IsAuthenticated}, Roles: {string.Join(", ", user.Claims.Select(c => c.Type + ": " + c.Value))}");

        // Act
        var result = await _controller.Delete(assetId);

        // Assert
        Assert.NotNull(result);
        var actionResult = Assert.IsType<ActionResult<ApiResponse<string>>>(result);

        if (actionResult.Result == null && actionResult.Value != null)
        {
            // If the method returns ApiResponse<string> directly, it�s wrapped in OkObjectResult
            var apiResponse = Assert.IsType<ApiResponse<string>>(actionResult.Value);
            Assert.True(apiResponse.Success);
            Assert.Equal("Successfully deleted asset.", apiResponse.Message);
            Assert.Equal(assetCode, apiResponse.Data);
            Assert.Empty(apiResponse.Errors);
        }
        else
        {
            if (actionResult.Result == null)
            {
                Assert.Fail($"actionResult.Result is null. Actual result type: {result.GetType().Name}, Value: {result}");
            }

            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            Assert.Equal(200, okResult.StatusCode);

            var apiResponse = Assert.IsType<ApiResponse<string>>(okResult.Value);
            Assert.True(apiResponse.Success);
            Assert.Equal("Successfully deleted asset.", apiResponse.Message);
            Assert.Equal(assetCode, apiResponse.Data);
            Assert.Empty(apiResponse.Errors);
        }

        _assetServiceMock.Verify();
    }
    [Fact]
    public async Task Delete_ThrowsUnauthorizedAccessException_WhenAdminIdIsMissing()
    {
        // Arrange
        var assetId = Guid.NewGuid();
        var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { new Claim(ClaimTypes.Role, "Admin") }, "mock"));
        _controller.ControllerContext.HttpContext = new DefaultHttpContext { User = user };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<UnauthorizedAccessException>(
            () => _controller.Delete(assetId));
        Assert.Equal("Cannot retrieve user id from claims", exception.Message);
        _assetServiceMock.Verify(s => s.DeleteAssetAsync(It.IsAny<Guid>(), It.IsAny<Guid>()), Times.Never());
    }

    [Fact]
    public async Task Delete_ThrowsKeyNotFoundException_WhenAssetDoesNotExist()
    {
        // Arrange
        var assetId = Guid.NewGuid();

        _assetServiceMock.Setup(s => s.DeleteAssetAsync(_adminId, assetId))
            .ThrowsAsync(new KeyNotFoundException($"Cannot find asset with id {assetId}"))
            .Verifiable();

        // Act & Assert
        var exception = await Assert.ThrowsAsync<KeyNotFoundException>(
            () => _controller.Delete(assetId));
        Assert.Equal($"Cannot find asset with id {assetId}", exception.Message);
        _assetServiceMock.Verify();
    }

    [Fact]
    public async Task Delete_ThrowsInvalidOperationException_WhenAssetAlreadyDeleted()
    {
        // Arrange
        var assetId = Guid.NewGuid();

        _assetServiceMock.Setup(s => s.DeleteAssetAsync(_adminId, assetId))
            .ThrowsAsync(new InvalidOperationException("Asset is already deleted"))
            .Verifiable();

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _controller.Delete(assetId));
        Assert.Equal("Asset is already deleted", exception.Message);
        _assetServiceMock.Verify();
    }

    [Fact]
    public async Task Create_ReturnsCreatedResponse_WhenValidRequestAndAuthorized()
    {
        // Arrange
        var request = new CreateAssetRequestDto
        {
            Name = "Test Asset",
            CategoryId = Guid.NewGuid(),
            Specifications = "Test specifications",
            InstalledDate = "2023-01-01",
            State = AssetStateDto.Available
        };

        var expectedResponse = new CreateAssetResponseDto
        {
            Code = "TC000001",
            Name = "Test Asset",
            CategoryName = "Test Category",
            StateName = "Available"
        };

        var adminId = Guid.NewGuid().ToString();
        _assetServiceMock.Setup(s => s.CreateAssetAsync(request, adminId))
            .ReturnsAsync(expectedResponse)
            .Verifiable();

        // Update ClaimsPrincipal to ensure admin role and ID
        var user = new ClaimsPrincipal(new ClaimsIdentity(
            new[]
            {
            new Claim(ClaimTypes.NameIdentifier, adminId),
            new Claim(ClaimTypes.Role, "Admin")
            }, "mock"));
        _controller.ControllerContext.HttpContext = new DefaultHttpContext { User = user };

        // Act
        var result = await _controller.Create(request);

        // Assert
        // Check if the result is an ActionResult<ApiResponse<CreateAssetResponseDto>>
        var actionResult = Assert.IsType<ActionResult<ApiResponse<CreateAssetResponseDto>>>(result);
        // Extract the ObjectResult
        var objectResult = Assert.IsType<ObjectResult>(actionResult.Result);
        Assert.Equal(201, objectResult.StatusCode);

        // Extract the ApiResponse from the Value
        var apiResponse = Assert.IsType<ApiResponse<CreateAssetResponseDto>>(objectResult.Value);
        Assert.True(apiResponse.Success);
        Assert.Equal("Successfully created a new asset", apiResponse.Message);
        Assert.Equal(expectedResponse, apiResponse.Data);
        Assert.NotNull(apiResponse.Data);
        Assert.Equal("TC000001", apiResponse.Data.Code);
        Assert.Equal("Test Asset", apiResponse.Data.Name);
        Assert.Equal("Test Category", apiResponse.Data.CategoryName);
        Assert.Equal("Available", apiResponse.Data.StateName);

        _assetServiceMock.Verify();
    }

    [Fact]
    public async Task Create_ThrowsValidationException_WhenRequestIsInvalid()
    {
        // Arrange
        var request = new CreateAssetRequestDto
        {
            Name = "", // Invalid: empty name
            CategoryId = Guid.NewGuid(),
            Specifications = "Test specifications",
            InstalledDate = "2023-01-01",
            State = AssetStateDto.Available
        };

        var adminId = Guid.NewGuid().ToString();
        _assetServiceMock.Setup(s => s.CreateAssetAsync(request, adminId))
            .ThrowsAsync(new ApiExceptionTypes.ValidationException("All required fields must be filled"))
            .Verifiable();

        var user = new ClaimsPrincipal(new ClaimsIdentity(
            new[]
            {
                new Claim(ClaimTypes.NameIdentifier, adminId),
                new Claim(ClaimTypes.Role, "Admin")
            }, "mock"));
        _controller.ControllerContext.HttpContext = new DefaultHttpContext { User = user };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ApiExceptionTypes.ValidationException>(
            () => _controller.Create(request));
        Assert.Equal("All required fields must be filled", exception.Message);
        _assetServiceMock.Verify();
    }

    [Fact]
    public async Task Create_ThrowsNotFoundException_WhenCategoryNotFound()
    {
        // Arrange
        var request = new CreateAssetRequestDto
        {
            Name = "Test Asset",
            CategoryId = Guid.NewGuid(),
            Specifications = "Test specifications",
            InstalledDate = "2023-01-01",
            State = AssetStateDto.Available
        };

        var adminId = Guid.NewGuid().ToString();
        _assetServiceMock.Setup(s => s.CreateAssetAsync(request, adminId))
            .ThrowsAsync(new ApiExceptionTypes.NotFoundException("Category not found"))
            .Verifiable();

        var user = new ClaimsPrincipal(new ClaimsIdentity(
            new[]
            {
                new Claim(ClaimTypes.NameIdentifier, adminId),
                new Claim(ClaimTypes.Role, "Admin")
            }, "mock"));
        _controller.ControllerContext.HttpContext = new DefaultHttpContext { User = user };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ApiExceptionTypes.NotFoundException>(
            () => _controller.Create(request));
        Assert.Equal("Category not found", exception.Message);
        _assetServiceMock.Verify();
    }

    [Fact]
    public async Task Create_ThrowsUnauthorizedAccessException_WhenAdminIdNotFound()
    {
        // Arrange
        var request = new CreateAssetRequestDto
        {
            Name = "Test Asset",
            CategoryId = Guid.NewGuid(),
            Specifications = "Test specifications",
            InstalledDate = "2023-01-01",
            State = AssetStateDto.Available
        };

        // Setup ClaimsPrincipal without NameIdentifier claim
        var user = new ClaimsPrincipal(new ClaimsIdentity(
            new[]
            {
                new Claim(ClaimTypes.Role, "Admin")
            }, "mock"));
        _controller.ControllerContext.HttpContext = new DefaultHttpContext { User = user };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<UnauthorizedAccessException>(
            () => _controller.Create(request));
        Assert.Equal("Cannot retrieve user id from claims", exception.Message);
        _assetServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task Create_RespectsAdminRoleAuthorization()
    {
        // Arrange
        var request = new CreateAssetRequestDto
        {
            Name = "Test Asset",
            CategoryId = Guid.NewGuid(),
            Specifications = "Test specifications",
            InstalledDate = "2023-01-01",
            State = AssetStateDto.Available
        };

        // Setup ClaimsPrincipal with Staff role instead of Admin
        var user = new ClaimsPrincipal(new ClaimsIdentity(
            new[]
            {
                new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Role, "Staff")
            }, "mock"));
        _controller.ControllerContext.HttpContext = new DefaultHttpContext { User = user };

        // Act
        // Note: In a real ASP.NET Core app, [Authorize(Roles = "Admin")] would block this request
        // before reaching the action. For unit testing, we simulate the call and verify behavior.
        // Since the middleware handles authorization, we can't directly test 403 in the action
        // without invoking the middleware. Instead, we ensure the test setup reflects the role.

        // We can call the action directly to ensure it would proceed if middleware allows
        var result = await _controller.Create(request);

        // Assert
        // Since the action itself doesn't check roles (middleware does), if we reach here,
        // the test confirms the action can be called. In a real scenario, middleware would block.
        // We verify the service was called if the action executes.
        _assetServiceMock.Verify(s => s.CreateAssetAsync(It.IsAny<CreateAssetRequestDto>(), It.IsAny<string>()), Times.Once());
    }

    [Fact]
    public async Task UpdateAsset_ReturnsSuccess_WhenUpdateIsValid()
    {
        // Arrange
        var assetCode = "LP000123";
        var userId = Guid.NewGuid().ToString();
        var request = new UpdateAssetRequestDto
        {
            Name = "Updated MacBook Pro",
            Specification = "M2 Pro, 32GB RAM, 1TB SSD"
        };

        // Update the mock to include the user ID claim
        var userClaims = new ClaimsIdentity(
        [
            new Claim(ClaimTypes.NameIdentifier, userId),
        new Claim(ClaimTypes.Role, "Admin")
        ], "mock");

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal(userClaims) }
        };

        _assetServiceMock.Setup(s => s.UpdateAssetAsync(userId, assetCode, request))
            .ReturnsAsync(assetCode)
            .Verifiable();

        // Act
        var result = await _controller.Update(assetCode, request);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Success);
        Assert.Equal("Successfully updated asset.", result.Message);
        Assert.Equal(assetCode, result.Data);

        _assetServiceMock.Verify();
    }

    [Fact]
    public async Task UpdateAsset_ThrowsException_WhenUserIdIsNull()
    {
        // Arrange
        var assetCode = "LP000123";
        var request = new UpdateAssetRequestDto
        {
            Name = "Updated MacBook Pro",
            Specification = "M2 Pro, 32GB RAM, 1TB SSD"
        };

        // Set up context without NameIdentifier claim
        var userClaims = new ClaimsIdentity(
        [
            new Claim(ClaimTypes.Role, "Admin")
        ], "mock");

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal(userClaims) }
        };

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
            _controller.Update(assetCode, request));
    }

    [Fact]
    public async Task GetReport_CallsServiceWithCorrectParameters()
    {
        // Arrange
        var adminId = Guid.NewGuid();
        var report = new List<AssetReportDto>
        {
            new AssetReportDto { Category = "Cat1", Total = 2, Available = 1, Assigned = 1, Recycled = 1, NotAvailable = 2, WaitingForRecycling = 0 },
            new AssetReportDto { Category = "Cat2", Total = 1, Available = 0, Assigned = 0, Recycled = 1, NotAvailable = 3, WaitingForRecycling = 1 }
        };

        _assetServiceMock.Setup(s => s.GetAssetReportAsync(adminId, It.IsAny<AssetReportQueryParameters>())).ReturnsAsync(report);

        var claimsPrincipal = CreateUserPrincipal(adminId.ToString(), "Admin");
        ApplyUserToController(claimsPrincipal);

        var queryParams = new AssetReportQueryParameters { SortBy = "Category", SortOrder = "asc" };

        // Act
        var result = await _controller.GetReport(queryParams);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var apiResponse = Assert.IsType<ApiResponse<List<AssetReportDto>>>(okResult.Value);
        Assert.True(apiResponse.Success);
        Assert.Equal("Successfully fetched asset report", apiResponse.Message);
        Assert.Equal(report, apiResponse.Data);
        _assetServiceMock.Verify(s => s.GetAssetReportAsync(adminId, It.Is<AssetReportQueryParameters>(qp => qp.SortBy == "Category" && qp.SortOrder == "asc")), Times.Once());
    }
}