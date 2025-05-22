using AssetManagement.Contracts.DTOs.Resquest;
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

        private User CreateSampleUser(Guid userId)
        {
            return new User
            {
                Id = userId,
                DateOfBirth = new DateTime(2000, 1, 1),
                JoinedDate = new DateTime(2020, 1, 1),
                Type = UserTypeEnum.Admin,
                Gender = GenderEnum.Male,
                IsActive = true,
                Email = "test@gmail.com",
                Password = "Pa$$w0rd",
                Username = "testuser",
            };
        }

        [Fact]
        public async Task UpdateUserAsync_ShouldThrow_KeyNotFoundException_WhenUserNotFound()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var mockRepo = new Mock<IUserRepository>();
            mockRepo.Setup(x => x.GetByIdAsync(userId)).ReturnsAsync((User)null!);

            var service = new UserService(mockRepo.Object);

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => service.UpdateUserAsync(userId, new UpdateUserRequest()));
        }


        [Fact]
        public async Task UpdateUserAsync_ShouldThrow_ArgumentException_WhenUserUnder18()
        {
            var userId = Guid.NewGuid();
            var mockRepo = new Mock<IUserRepository>();
            var user = CreateSampleUser(userId);

            mockRepo.Setup(x => x.GetByIdAsync(userId)).ReturnsAsync(user);
            var service = new UserService(mockRepo.Object);

            var request = new UpdateUserRequest
            {
                DateOfBirth = DateTime.UtcNow.AddYears(-17) // under 18
            };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => service.UpdateUserAsync(userId, request));
        }

        [Fact]
        public async Task UpdateUserAsync_ShouldThrow_WhenJoinedDateBefore18()
        {
            var userId = Guid.NewGuid();
            var mockRepo = new Mock<IUserRepository>();
            var user = CreateSampleUser(userId);

            mockRepo.Setup(x => x.GetByIdAsync(userId)).ReturnsAsync(user);
            var service = new UserService(mockRepo.Object);

            var request = new UpdateUserRequest
            {
                DateOfBirth = new DateTime(2010, 1, 1),
                JoinedDate = new DateTime(2025, 1, 1) // before 18th birthday
            };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => service.UpdateUserAsync(userId, request));
        }

        [Fact]
        public async Task UpdateUserAsync_ShouldThrow_WhenJoinedDateIsWeekend()
        {
            var userId = Guid.NewGuid();
            var mockRepo = new Mock<IUserRepository>();
            var user = CreateSampleUser(userId);

            mockRepo.Setup(x => x.GetByIdAsync(userId)).ReturnsAsync(user);
            var service = new UserService(mockRepo.Object);

            var request = new UpdateUserRequest
            {
                JoinedDate = new DateTime(2025, 5, 17) // Saturday
            };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => service.UpdateUserAsync(userId, request));
        }

        [Fact]
        public async Task UpdateUserAsync_ShouldUpdateSuccessfully()
        {
            var userId = Guid.NewGuid();
            var user = CreateSampleUser(userId);

            var mockRepo = new Mock<IUserRepository>();
            mockRepo.Setup(x => x.GetByIdAsync(userId)).ReturnsAsync(user);
            var service = new UserService(mockRepo.Object);

            var request = new UpdateUserRequest
            {
                DateOfBirth = new DateTime(2000, 1, 1),
                JoinedDate = new DateTime(2021, 1, 4), // Monday
                Type = "Admin",
                Gender = "Male"
            };

            var result = await service.UpdateUserAsync(userId, request);

            Assert.Equal(userId, result);
            mockRepo.Verify(r => r.Update(It.IsAny<User>()), Times.Once);
        }

        [Fact]
        public async Task DeleteUser_ShouldThrow_WhenUserNotFound()
        {
            // Arrange
            var staffCode = "NonExistentCode";

            // Create an empty mock queryable that supports async operations
            var emptyUsersMock = new List<User>().AsQueryable().BuildMock();

            var mockRepo = new Mock<IUserRepository>();
            mockRepo.Setup(x => x.GetAll()).Returns(emptyUsersMock);

            var service = new UserService(mockRepo.Object);

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() =>
                service.DeleteUser(Guid.NewGuid(), staffCode));
        }

        [Fact]
        public async Task DeleteUser_ShouldSetIsActiveFalse_AndCallUpdate()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var deletedBy = Guid.NewGuid();
            var user = CreateSampleUser(userId);
            user.StaffCode = "SD0001"; // Ensure staff code is set

            // Create a mock queryable that supports async operations
            var usersMock = new List<User> { user }.AsQueryable().BuildMock();

            var mockRepo = new Mock<IUserRepository>();
            // Set up GetAll to return the mock queryable that supports async
            mockRepo.Setup(x => x.GetAll()).Returns(usersMock);
            mockRepo.Setup(x => x.Update(It.IsAny<User>()));

            var service = new UserService(mockRepo.Object);

            // Act
            var result = await service.DeleteUser(deletedBy, user.StaffCode);

            // Assert
            Assert.Equal(user.StaffCode, result);
            Assert.False(user.IsActive);
            Assert.Equal(deletedBy, user.DeletedBy);
            Assert.NotNull(user.DeletedDate);

            mockRepo.Verify(r => r.Update(It.IsAny<User>()), Times.Once);
        }

    }
}
