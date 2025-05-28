using AssetManagement.Application.Controllers;
using AssetManagement.Application.Services.Interfaces;
using AssetManagement.Contracts.Common;
using AssetManagement.Contracts.DTOs;
using Microsoft.AspNetCore.Mvc;
using Moq;

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
    }
}