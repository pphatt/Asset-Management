using AssetManagement.Application.Controllers;
using AssetManagement.Application.Services.Interfaces;
using AssetManagement.Contracts.Common;
using AssetManagement.Contracts.DTOs;
using AssetManagement.Contracts.DTOs.Requests;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security.Claims;

namespace AssetManagement.Application.Tests.Controllers
{
    public class CategoriesControllerTests
    {
        private readonly Mock<ICategoryService> _mockCategoryService;
        private readonly CategoriesController _controller;

        public CategoriesControllerTests()
        {
            _mockCategoryService = new Mock<ICategoryService>();
            _controller = new CategoriesController(_mockCategoryService.Object);
        }

        [Fact]
        public async Task Get_ReturnsSuccessResponse_WhenCategoriesExist()
        {
            // Arrange
            var expectedCategories = new List<CategoryDto>
            {
                new CategoryDto { Name = "Electronics", Prefix = "ELE" },
                new CategoryDto { Name = "Furniture", Prefix = "FUR" }
            };

            _mockCategoryService
                .Setup(x => x.GetCategoriesAsync())
                .ReturnsAsync(expectedCategories);

            // Act
            var result = await _controller.Get();

            // Assert
            var actionResult = Assert.IsType<ActionResult<ApiResponse<IEnumerable<CategoryDto>>>>(result);
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var apiResponse = Assert.IsType<ApiResponse<IEnumerable<CategoryDto>>>(okResult.Value);

            Assert.True(apiResponse.Success);
            Assert.Equal("Successfully fetched all categories", apiResponse.Message);
            Assert.Equal(expectedCategories, apiResponse.Data);
            Assert.Empty(apiResponse.Errors);
        }

        [Fact]
        public async Task Get_ReturnsEmptyList_WhenNoCategoriesExist()
        {
            // Arrange
            var emptyCategories = new List<CategoryDto>();

            _mockCategoryService
                .Setup(x => x.GetCategoriesAsync())
                .ReturnsAsync(emptyCategories);

            // Act
            var result = await _controller.Get();

            // Assert
            var actionResult = Assert.IsType<ActionResult<ApiResponse<IEnumerable<CategoryDto>>>>(result);
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var apiResponse = Assert.IsType<ApiResponse<IEnumerable<CategoryDto>>>(okResult.Value);

            Assert.True(apiResponse.Success);
            Assert.NotNull(apiResponse.Data);
            Assert.Empty(apiResponse.Data);
        }

        #region Create category endpoint tests

        [Fact]
        public async Task Create_ReturnsSuccessResponse_WhenCategoryIsCreated()
        {
            // Arrange
            var userId = Guid.NewGuid().ToString();
            var categoryRequest = new CreateCategoryRequestDto { Name = "Stationery", Prefix = "ST" };
            var createdCategory = new CategoryDto { Id = Guid.NewGuid(), Name = "Stationery", Prefix = "ST" };

            _mockCategoryService
                .Setup(x => x.CreateCategoryAsync(categoryRequest, userId))
                .ReturnsAsync(createdCategory);

            var controller = new CategoriesController(_mockCategoryService.Object);

            // Mock user claims
            var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, userId) };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var claimsPrincipal = new ClaimsPrincipal(identity);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = claimsPrincipal }
            };

            // Act
            var result = await controller.Create(categoryRequest);

            // Assert
            var actionResult = Assert.IsType<ActionResult<ApiResponse<CategoryDto>>>(result);
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var apiResponse = Assert.IsType<ApiResponse<CategoryDto>>(okResult.Value);

            Assert.True(apiResponse.Success);
            Assert.Equal("Category created successfully", apiResponse.Message);
            Assert.Equal(createdCategory, apiResponse.Data);
            Assert.Empty(apiResponse.Errors);
        }

        [Fact]
        public async Task Create_ThrowsUnauthorizedAccessException_WhenUserIdClaimMissing()
        {
            // Arrange
            var categoryRequest = new CreateCategoryRequestDto { Name = "Stationery", Prefix = "ST" };
            var controller = new CategoriesController(_mockCategoryService.Object);

            // No NameIdentifier claim
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal() }
            };

            // Act & Assert
            await Assert.ThrowsAsync<UnauthorizedAccessException>(() => controller.Create(categoryRequest));
        }

        [Fact]
        public async Task Create_ReturnsErrorResponse_WhenServiceThrowsException()
        {
            // Arrange
            var userId = Guid.NewGuid().ToString();
            var categoryRequest = new CreateCategoryRequestDto { Name = "Stationery", Prefix = "ST" };

            _mockCategoryService
                .Setup(x => x.CreateCategoryAsync(categoryRequest, userId))
                .ThrowsAsync(new Exception("Some error"));

            var controller = new CategoriesController(_mockCategoryService.Object);

            // Mock user claims
            var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, userId) };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var claimsPrincipal = new ClaimsPrincipal(identity);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = claimsPrincipal }
            };

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => controller.Create(categoryRequest));
        }

        #endregion
    }
}