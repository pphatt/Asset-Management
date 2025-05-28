using AssetManagement.Application.Services;
using AssetManagement.Domain.Entities;
using AssetManagement.Domain.Interfaces.Repositories;
using MockQueryable;
using Moq;

namespace AssetManagement.Application.Tests.Services
{
    public class CategoryServiceTests
    {
        private readonly Mock<ICategoryRepository> _mockCategoryRepository;
        private readonly CategoryService _categoryService;

        public CategoryServiceTests()
        {
            _mockCategoryRepository = new Mock<ICategoryRepository>();
            _categoryService = new CategoryService(_mockCategoryRepository.Object);
        }

        [Fact]
        public async Task GetCategoriesAsync_ReturnsCorrectDtos_WhenCategoriesExist()
        {
            // Arrange
            var categories = new List<Category>
            {
                new Category { Id = Guid.NewGuid(), Name = "Electronics", Prefix = "ELE" },
                new Category { Id = Guid.NewGuid(), Name = "Furniture", Prefix = "FUR" }
            };

            var mockQueryable = categories.AsQueryable().BuildMock();

            _mockCategoryRepository
                .Setup(x => x.GetAll())
                .Returns(mockQueryable);

            // Act
            var result = await _categoryService.GetCategoriesAsync();

            // Assert
            var resultList = result.ToList();
            Assert.Equal(2, resultList.Count);

            Assert.Equal("Electronics", resultList[0].Name);
            Assert.Equal("ELE", resultList[0].Prefix);

            Assert.Equal("Furniture", resultList[1].Name);
            Assert.Equal("FUR", resultList[1].Prefix);

            _mockCategoryRepository.Verify(x => x.GetAll(), Times.Once);
        }

        [Fact]
        public async Task GetCategoriesAsync_ReturnsEmptyList_WhenNoCategoriesExist()
        {
            // Arrange
            var emptyCategories = new List<Category>();
            var mockQueryable = emptyCategories.AsQueryable().BuildMock();

            _mockCategoryRepository
                .Setup(x => x.GetAll())
                .Returns(mockQueryable);

            // Act
            var result = await _categoryService.GetCategoriesAsync();

            // Assert
            Assert.Empty(result);
        }
    }
}