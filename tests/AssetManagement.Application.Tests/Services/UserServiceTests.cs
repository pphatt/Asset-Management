using AssetManagement.Contracts.Parameters;
using AssetManagement.Domain.Entities;
using AssetManagement.Domain.Enums;
using AssetManagement.Domain.Repositories;
using MockQueryable;
using Moq;

namespace AssetManagement.Application.Services.Tests
{
    public class UserServiceTests
    {
        private readonly Mock<IUserRepository> _mockUserRepository;
        private readonly UserService _userService;

        public UserServiceTests()
        {
            _mockUserRepository = new Mock<IUserRepository>();
            _userService = new UserService(_mockUserRepository.Object);
        }

        [Fact]
        public async Task GetUsersAsync_ValidRequest_ReturnsFilteredUsers()
        {
            // Arrange
            var adminUserId = Guid.NewGuid();
            var adminUser = new User
            {
                Id = adminUserId,
                Username = "admin",
                Email = "admin@gmail.com",
                Password = "Pa$$w0rd",
                Type = UserTypeEnum.Admin,
                Location = LocationEnum.HCM
            };

            var users = new List<User>
            {
                new User { Id = Guid.NewGuid(), FirstName = "John", LastName = "Doe", Email = "johndoe@gmail.com", Password = "Pa$$w0rd", StaffCode = "SD0001", Type = UserTypeEnum.Staff, Location = LocationEnum.HCM, JoinedDate = DateTimeOffset.UtcNow, Username = "johndoe" },
                new User { Id = Guid.NewGuid(), FirstName = "Jane", LastName = "Smith", Email = "janesmith@gmail.com", Password = "Pa$$w0rd", StaffCode = "SD0002", Type = UserTypeEnum.Admin, Location = LocationEnum.HCM, JoinedDate = DateTimeOffset.UtcNow, Username = "janesmith" },
            }.BuildMock();

            _mockUserRepository.Setup(r => r.GetByIdAsync(adminUserId)).ReturnsAsync(adminUser);
            _mockUserRepository.Setup(r => r.GetAll()).Returns(users);

            var queryParams = new UserQueryParameters
            {
                PageNumber = 1,
                PageSize = 10,
                SearchTerm = "John",
                UserType = "all",
                SortBy = "name:asc",
            };

            // Act
            var result = await _userService.GetUsersAsync(adminUserId.ToString(), queryParams);

            // Assert
            Assert.Single(result.Items);
            Assert.Equal("SD0001", result.Items.First().StaffCode);
            Assert.Equal(1, result.PaginationMetadata.TotalItems);
        }

        [Fact]
        public async Task GetUsersAsync_UserNotFound_ThrowsKeyNotFoundException()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var queryParams = new UserQueryParameters { PageNumber = 1, PageSize = 10 };

            _mockUserRepository.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync((User?)null);

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => _userService.GetUsersAsync(userId.ToString(), queryParams));
        }
    }
}
