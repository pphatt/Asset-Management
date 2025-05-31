using AssetManagement.Application.Services;
using AssetManagement.Contracts.DTOs.Requests;
using AssetManagement.Contracts.Exceptions;
using AssetManagement.Domain.Entities;
using AssetManagement.Domain.Interfaces.Repositories;
using MockQueryable;
using Moq;
using static AssetManagement.Contracts.Exceptions.ApiExceptionTypes;

namespace AssetManagement.Application.Tests.Services
{
    public class CategoryServiceTests
    {
        private readonly Mock<ICategoryRepository> _mockCategoryRepository;
        private readonly Mock<IUserRepository> _mockUserRepository;
        private readonly CategoryService _categoryService;

        public CategoryServiceTests()
        {
            _mockCategoryRepository = new Mock<ICategoryRepository>();
            _mockUserRepository = new Mock<IUserRepository>();
            _categoryService = new CategoryService(_mockCategoryRepository.Object,
                _mockUserRepository.Object);
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

        #region Create category test cases

        [Fact]
        public async Task CreateCategory_SuccessfullyCreatesCategory_WhenValidRequest()
        {
            // Arrange
            var userId = Guid.NewGuid().ToString();
            var user = new User { Id = Guid.Parse(userId), Username = "testuser", Password = "pass" };
            var request = new CreateCategoryRequestDto { Name = "Stationery", Prefix = "ST" };

            var mockUserQueryable = new List<User> { user }.AsQueryable().BuildMock();
            var mockCategoryQueryable = new List<Category>().AsQueryable().BuildMock();

            var mockUserRepo = new Mock<IUserRepository>();
            mockUserRepo.Setup(x => x.GetAll()).Returns(mockUserQueryable);

            _mockCategoryRepository.Setup(x => x.GetAll()).Returns(mockCategoryQueryable);

            var service = new CategoryService(_mockCategoryRepository.Object, mockUserRepo.Object);

            // Act
            var result = await service.CreateCategoryAsync(request, userId);

            // Assert
            Assert.Equal(request.Name, result.Name);
            Assert.Equal(request.Prefix.ToUpper(), result.Prefix);
            _mockCategoryRepository.Verify(x => x.AddAsync(It.Is<Category>(c => c.Name == request.Name 
                && c.Prefix == request.Prefix.ToUpper() 
                && c.CreatedBy == user.Id)), 
                Times.Once);
            _mockCategoryRepository.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task CreateCategory_ThrowsKeyNotFoundException_WhenUserNotFound()
        {
            // Arrange
            var userId = Guid.NewGuid().ToString();
            var request = new CreateCategoryRequestDto { Name = "Stationery", Prefix = "ST" };
            var mockUserQueryable = new List<User>().AsQueryable().BuildMock();

            var mockUserRepo = new Mock<IUserRepository>();
            mockUserRepo.Setup(x => x.GetAll()).Returns(mockUserQueryable);

            var service = new CategoryService(_mockCategoryRepository.Object, mockUserRepo.Object);

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => service.CreateCategoryAsync(request, userId));
        }

        [Theory]
        [InlineData("S")]
        [InlineData("   ")]
        [InlineData("ABC")]
        public async Task CreateCategory_ThrowsValidationException_WhenPrefixIsNotTwoCharacters(string prefix)
        {
            // Arrange
            var userId = Guid.NewGuid().ToString();
            var user = new User { Id = Guid.Parse(userId), Username = "testuser", Password = "pass" };
            var request = new CreateCategoryRequestDto { Name = "Stationery", Prefix = prefix };

            var mockUserQueryable = new List<User> { user }.AsQueryable().BuildMock();
            var mockCategoryQueryable = new List<Category>().AsQueryable().BuildMock();

            var mockUserRepo = new Mock<IUserRepository>();
            mockUserRepo.Setup(x => x.GetAll()).Returns(mockUserQueryable);

            _mockCategoryRepository.Setup(x => x.GetAll()).Returns(mockCategoryQueryable);

            var service = new CategoryService(_mockCategoryRepository.Object, mockUserRepo.Object);

            // Act & Assert
            await Assert.ThrowsAsync<AggregateFieldValidationException>(() 
                => service.CreateCategoryAsync(request, userId));
        }

        [Fact]
        public async Task CreateCategory_ThrowsDuplicateResourceException_WhenCategoryNameExists()
        {
            // Arrange
            var userId = Guid.NewGuid().ToString();
            var user = new User { Id = Guid.Parse(userId), Username = "testuser", Password = "pass" };
            var request = new CreateCategoryRequestDto { Name = "Stationery", Prefix = "ST" };

            var existingCategory = new Category { Name = "Stationery", Prefix = "XX" };

            var mockUserQueryable = new List<User> { user }.AsQueryable().BuildMock();
            var mockCategoryQueryable = new List<Category> { existingCategory }.AsQueryable().BuildMock();

            var mockUserRepo = new Mock<IUserRepository>();
            mockUserRepo.Setup(x => x.GetAll()).Returns(mockUserQueryable);

            _mockCategoryRepository.SetupSequence(x => x.GetAll())
                .Returns(mockCategoryQueryable) // For name check
                .Returns(new List<Category>().AsQueryable().BuildMock()); // For prefix check

            var service = new CategoryService(_mockCategoryRepository.Object, mockUserRepo.Object);

            // Act & Assert
            await Assert.ThrowsAsync<DuplicateResourceException>(() => service.CreateCategoryAsync(request, userId));
        }

        [Fact]
        public async Task CreateCategory_ThrowsDuplicateResourceException_WhenPrefixExists()
        {
            // Arrange
            var userId = Guid.NewGuid().ToString();
            var user = new User { Id = Guid.Parse(userId), Username = "testuser", Password = "pass" };
            var request = new CreateCategoryRequestDto { Name = "NewCategory", Prefix = "ST" };

            var existingCategory = new Category { Name = "Other", Prefix = "ST" };

            var mockUserQueryable = new List<User> { user }.AsQueryable().BuildMock();
            var mockCategoryQueryableForName = new List<Category>().AsQueryable().BuildMock();
            var mockCategoryQueryableForPrefix = new List<Category> { existingCategory }.AsQueryable().BuildMock();

            var mockUserRepo = new Mock<IUserRepository>();
            mockUserRepo.Setup(x => x.GetAll()).Returns(mockUserQueryable);

            _mockCategoryRepository.SetupSequence(x => x.GetAll())
                .Returns(mockCategoryQueryableForName) // For name check
                .Returns(mockCategoryQueryableForPrefix); // For prefix check

            var service = new CategoryService(_mockCategoryRepository.Object, mockUserRepo.Object);

            // Act & Assert
            await Assert.ThrowsAsync<DuplicateResourceException>(() => service.CreateCategoryAsync(request, userId));
        }

        [Theory]
        [InlineData("A1")]
        [InlineData("A-")]
        [InlineData("!@")]
        public async Task CreateCategory_ThrowsValidationException_WhenPrefixIsNotAlphabetic(string prefix)
        {
            // Arrange
            var userId = Guid.NewGuid().ToString();
            var user = new User { Id = Guid.Parse(userId), Username = "testuser", Password = "pass" };
            var request = new CreateCategoryRequestDto { Name = "Stationery", Prefix = prefix };

            var mockUserQueryable = new List<User> { user }.AsQueryable().BuildMock();
            var mockCategoryQueryable = new List<Category>().AsQueryable().BuildMock();

            var mockUserRepo = new Mock<IUserRepository>();
            mockUserRepo.Setup(x => x.GetAll()).Returns(mockUserQueryable);

            _mockCategoryRepository.Setup(x => x.GetAll()).Returns(mockCategoryQueryable);

            var service = new CategoryService(_mockCategoryRepository.Object, mockUserRepo.Object);

            // Act & Assert
            await Assert.ThrowsAsync<AggregateFieldValidationException>(() => 
                service.CreateCategoryAsync(request, userId));
        }

        #endregion
    }
}