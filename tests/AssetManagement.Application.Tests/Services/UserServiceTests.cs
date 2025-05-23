using AssetManagement.Contracts.DTOs.Requests;
using AssetManagement.Application.Services;
using AssetManagement.Contracts.Enums;
using AssetManagement.Contracts.Exceptions;
using AssetManagement.Contracts.Parameters;
using AssetManagement.Data.Helpers.Hashing;
using AssetManagement.Domain.Entities;
using AssetManagement.Domain.Enums;
using AssetManagement.Domain.Repositories;
using MockQueryable;
using Moq;

namespace AssetManagement.Application.Tests.Services
{
    public class UserServiceTests
    {
        private readonly Mock<IUserRepository> _mockUserRepository;
        private readonly UserService _userService;
        private readonly Mock<IPasswordHasher> _passwordHasherMock;

        public UserServiceTests()
        {
            _mockUserRepository = new Mock<IUserRepository>();
            _passwordHasherMock = new Mock<IPasswordHasher>();
            _userService = new UserService(_mockUserRepository.Object, _passwordHasherMock.Object);
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
                Password = "Pa$$w0rd",
                Type = UserTypeEnum.Admin,
                Location = LocationEnum.HCM
            };

            var users = new List<User>
            {
                new User
                {
                    Id = Guid.NewGuid(), FirstName = "John", LastName = "Doe",
                    Password = "Pa$$w0rd", StaffCode = "SD0001", Type = UserTypeEnum.Staff, Location = LocationEnum.HCM,
                    JoinedDate = DateTimeOffset.UtcNow, Username = "johndoe"
                },
                new User
                {
                    Id = Guid.NewGuid(), FirstName = "Jane", LastName = "Smith",
                    Password = "Pa$$w0rd", StaffCode = "SD0002", Type = UserTypeEnum.Admin, Location = LocationEnum.HCM,
                    JoinedDate = DateTimeOffset.UtcNow, Username = "janesmith"
                },
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
            await Assert.ThrowsAsync<KeyNotFoundException>(() =>
                _userService.GetUsersAsync(userId.ToString(), queryParams));
        }

        [Theory]
        [InlineData(UserTypeDtoEnum.Admin, UserTypeEnum.Admin)]
        [InlineData(UserTypeDtoEnum.Staff, UserTypeEnum.Staff)]
        public void MapUserType_ShouldMapCorrectly(UserTypeDtoEnum input, UserTypeEnum expected)
        {
            var result = typeof(UserService).GetMethod("MapUserType",
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)!
                .Invoke(null, new object[] { input });
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData(GenderDtoEnum.Male, GenderEnum.Male)]
        [InlineData(GenderDtoEnum.Female, GenderEnum.Female)]
        public void MapGender_ShouldMapCorrectly(GenderDtoEnum input, GenderEnum expected)
        {
            var result = typeof(UserService).GetMethod("MapGender",
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)!
                .Invoke(null, new object[] { input });
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData(LocationEnum.HCM, LocationDtoEnum.HCM)]
        [InlineData(LocationEnum.DN, LocationDtoEnum.DN)]
        [InlineData(LocationEnum.HN, LocationDtoEnum.HN)]
        public void MapLocationToDto_ShouldMapCorrectly(LocationEnum input, LocationDtoEnum expected)
        {
            var result = typeof(UserService).GetMethod("MapLocationToDto",
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)!
                .Invoke(null, new object[] { input });
            Assert.Equal(expected, result);
        }

        [Fact]
        public async Task CreateUserAsync_ShouldThrow_WhenDateOfBirthIsEmpty()
        {
            var dto = new CreateUserRequestDto
            {
                FirstName = "A",
                LastName = "B",
                DateOfBirth = "",
                JoinedDate = DateTime.UtcNow.ToString("o"),
                Type = UserTypeDtoEnum.Staff,
                Gender = GenderDtoEnum.Male
            };

            var ex = await Assert.ThrowsAsync<AggregateFieldValidationException>(() =>
                _userService.CreateUserAsync(Guid.NewGuid().ToString(), dto));

            Assert.Contains(ex.Errors, e => e.Field == "DateOfBirth");
        }

        [Fact]
        public async Task CreateUserAsync_ShouldThrow_WhenUserIsUnder18()
        {
            var dto = new CreateUserRequestDto
            {
                FirstName = "A",
                LastName = "B",
                DateOfBirth = DateTime.UtcNow.AddYears(-17).ToString("yyyy-MM-dd"),
                JoinedDate = DateTime.UtcNow.ToString("o"),
                Type = UserTypeDtoEnum.Staff,
                Gender = GenderDtoEnum.Male
            };

            var ex = await Assert.ThrowsAsync<AggregateFieldValidationException>(() =>
                _userService.CreateUserAsync(Guid.NewGuid().ToString(), dto));

            Assert.Contains(ex.Errors, e => e.Field == "DateOfBirth");
        }

        [Fact]
        public async Task CreateUserAsync_ShouldThrow_WhenJoinedDateBeforeValid()
        {
            var dto = new CreateUserRequestDto
            {
                FirstName = "A",
                LastName = "B",
                DateOfBirth = DateTime.UtcNow.AddYears(-20).ToString("yyyy-MM-dd"),
                JoinedDate = DateTime.UtcNow.AddYears(-3).ToString("o"),
                Type = UserTypeDtoEnum.Staff,
                Gender = GenderDtoEnum.Male
            };

            var ex = await Assert.ThrowsAsync<AggregateFieldValidationException>(() =>
                _userService.CreateUserAsync(Guid.NewGuid().ToString(), dto));

            Assert.Contains(ex.Errors, e => e.Field == "JoinedDate");
        }

        [Fact]
        public async Task CreateUserAsync_ShouldThrow_WhenJoinedDateIsWeekend()
        {
            var saturday = new DateTime(2025, 5, 24); // example Saturday
            var dto = new CreateUserRequestDto
            {
                FirstName = "A",
                LastName = "B",
                DateOfBirth = "2000-01-01",
                JoinedDate = saturday.ToString("o"),
                Type = UserTypeDtoEnum.Staff,
                Gender = GenderDtoEnum.Male
            };

            var ex = await Assert.ThrowsAsync<AggregateFieldValidationException>(() =>
                _userService.CreateUserAsync(Guid.NewGuid().ToString(), dto));

            Assert.Contains(ex.Errors, e => e.Field == "JoinedDate");
        }

        [Fact]
        public async Task CreateUserAsync_ShouldSucceed_WithValidInput()
        {
            // Arrange
            var adminId = Guid.NewGuid().ToString();

            // Setup admin user location
            var adminUser = new User
            {
                Id = Guid.Parse(adminId),
                Username = "admin",
                Password = "hashed",
                Location = LocationEnum.HCM
            };

            _mockUserRepository
                .Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(adminUser);

            _mockUserRepository
                .Setup(r => r.GetAllUsernamesAsync())
                .ReturnsAsync(new List<string> { "vov" });

            _mockUserRepository
                .Setup(r => r.GetStaffCodesAsync())
                .ReturnsAsync(new List<string> { "SD0009" });

            _passwordHasherMock
                .Setup(h => h.HashPassword(It.IsAny<string>()))
                .Returns("hashed_password");
            var dto = new CreateUserRequestDto
            {
                FirstName = "Vinh",
                LastName = "Vo",
                DateOfBirth = "2000-01-01",
                JoinedDate = "2023-05-22T00:00:00",
                Type = UserTypeDtoEnum.Staff,
                Gender = GenderDtoEnum.Male
            };

            // Act
            var result = await _userService.CreateUserAsync(adminId, dto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("SD0010", result.StaffCode); // từ SD0009 + 1
            Assert.Equal("vinhv", result.Username); // "Vinh" + "V" (từ "Vo")
            Assert.Equal("Vo Vinh", result.FullName);
            Assert.Equal(LocationDtoEnum.HCM, result.Location);
        }

        #region CreateUserAsync Tests - Input Validation

        [Fact]
        public async Task CreateUserAsync_NullRequest_ThrowsArgumentNullException()
        {
            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentNullException>(
                () => _userService.CreateUserAsync(Guid.NewGuid().ToString(), null));

            Assert.Equal("dto", exception.ParamName);
        }

        [Theory]
        [InlineData("", "Doe", "Invalid FirstName")]
        [InlineData("John", "", "Invalid LastName")]
        [InlineData(null, "Doe", "Null FirstName")]
        [InlineData("John", null, "Null LastName")]
        public async Task CreateUserAsync_InvalidNames_ThrowsFieldValidationException(
            string firstName, string lastName, string testName)
        {
            // Arrange
            var dto = new CreateUserRequestDto
            {
                FirstName = firstName,
                LastName = lastName,
                DateOfBirth = "2000-01-01",
                JoinedDate = "2023-05-20T00:00:00",
                Type = UserTypeDtoEnum.Staff,
                Gender = GenderDtoEnum.Male
            };

            // Act & Assert
            var exception = await Assert.ThrowsAsync<AggregateFieldValidationException>(() =>
                _userService.CreateUserAsync(Guid.NewGuid().ToString(), dto));

            var expectedField = string.IsNullOrEmpty(firstName) ? "FirstName" : "LastName";
            Assert.Contains(exception.Errors, e => e.Field == expectedField);
        }
        
        #endregion

        #region CreateUserAsync Tests - Date of Birth Validation

        [Theory]
        [InlineData("", "Empty Date of Birth")]
        [InlineData(null, "Null Date of Birth")]
        [InlineData("not-a-date", "Invalid Date Format")]
        [InlineData("2/31/2000", "Invalid Date Value")]
        public async Task CreateUserAsync_InvalidDateOfBirth_ThrowsFieldValidationException(
            string dateOfBirth, string testName)
        {
            // Arrange
            var dto = new CreateUserRequestDto
            {
                FirstName = "John",
                LastName = "Doe",
                DateOfBirth = dateOfBirth,
                JoinedDate = "2023-05-20T00:00:00",
                Type = UserTypeDtoEnum.Staff,
                Gender = GenderDtoEnum.Male
            };

            // Act & Assert
            var exception = await Assert.ThrowsAsync<AggregateFieldValidationException>(
                () => _userService.CreateUserAsync(Guid.NewGuid().ToString(), dto));

            Assert.Contains(exception.Errors, e => e.Field == "DateOfBirth");
        }

        [Theory]
        [InlineData("1800-01-01", "Too Old")]
        [InlineData("2100-01-01", "Future Date")]
        public async Task CreateUserAsync_UnreasonableDateOfBirth_ThrowsFieldValidationException(
            string dateOfBirth, string testName)
        {
            // Arrange
            var dto = new CreateUserRequestDto
            {
                FirstName = "John",
                LastName = "Doe",
                DateOfBirth = dateOfBirth,
                JoinedDate = "2023-05-20T00:00:00",
                Type = UserTypeDtoEnum.Staff,
                Gender = GenderDtoEnum.Male
            };

            // Act & Assert
            var exception = await Assert.ThrowsAsync<AggregateFieldValidationException>(
                () => _userService.CreateUserAsync(Guid.NewGuid().ToString(), dto));

            Assert.Contains(exception.Errors, e => e.Field == "DateOfBirth");
        }

        [Theory]
        [InlineData("2010-01-01", "15 Years Old")]
        [InlineData("2008-06-15", "Almost 17 Years Old")]
        public async Task CreateUserAsync_UserUnder18_ThrowsFieldValidationException(
            string dateOfBirth, string testName)
        {
            // Arrange
            var dto = new CreateUserRequestDto
            {
                FirstName = "John",
                LastName = "Doe",
                DateOfBirth = dateOfBirth,
                JoinedDate = "2023-05-20T00:00:00",
                Type = UserTypeDtoEnum.Staff,
                Gender = GenderDtoEnum.Male
            }; // Act & Assert
            var exception = await Assert.ThrowsAsync<AggregateFieldValidationException>(
                () => _userService.CreateUserAsync(Guid.NewGuid().ToString(), dto));

            Assert.Contains(exception.Errors, e => e.Field == "DateOfBirth" && e.Message.Contains("under 18"));
        }

        #endregion

        #region CreateUserAsync Tests - Joined Date Validation

        [Theory]
        [InlineData("", "Empty Joined Date")]
        [InlineData(null, "Null Joined Date")]
        [InlineData("not-a-date", "Invalid Date Format")]
        [InlineData("2/31/2023", "Invalid Date Value")]
        public async Task CreateUserAsync_InvalidJoinedDate_ThrowsFieldValidationException(
            string joinedDate, string testName)
        {
            // Arrange
            var dto = new CreateUserRequestDto
            {
                FirstName = "John",
                LastName = "Doe",
                DateOfBirth = "2000-01-01", // 18+ years old
                JoinedDate = joinedDate,
                Type = UserTypeDtoEnum.Staff,
                Gender = GenderDtoEnum.Male
            };

            // Act & Assert
            var exception = await Assert.ThrowsAsync<AggregateFieldValidationException>(
                () => _userService.CreateUserAsync(Guid.NewGuid().ToString(), dto));

            Assert.Contains(exception.Errors, e => e.Field == "JoinedDate");
        }

        [Theory]
        [InlineData("1800-01-01T00:00:00", "Too Old")]
        [InlineData("2100-01-01T00:00:00", "Too Far in Future")]
        public async Task CreateUserAsync_UnreasonableJoinedDate_ThrowsFieldValidationException(
            string joinedDate, string testName)
        {
            // Arrange
            var dto = new CreateUserRequestDto
            {
                FirstName = "John",
                LastName = "Doe",
                DateOfBirth = "2000-01-01",
                JoinedDate = joinedDate,
                Type = UserTypeDtoEnum.Staff,
                Gender = GenderDtoEnum.Male
            };

            // Act & Assert           
            var exception = await Assert.ThrowsAsync<AggregateFieldValidationException>(
                () => _userService.CreateUserAsync(Guid.NewGuid().ToString(), dto));

            Assert.Contains(exception.Errors,
                e => e.Field == "JoinedDate" && e.Message.Contains("Invalid Joined Date value"));
        }

        [Fact]
        public async Task CreateUserAsync_JoinedDateBeforeAge18_ThrowsFieldValidationException()
        {
            // Arrange
            var dto = new CreateUserRequestDto
            {
                FirstName = "John",
                LastName = "Doe",
                DateOfBirth = "2000-06-15", // June 15, 2000
                JoinedDate = "2018-06-14T00:00:00", // One day before turning 18
                Type = UserTypeDtoEnum.Staff,
                Gender = GenderDtoEnum.Male
            }; // Act & Assert
            var exception = await Assert.ThrowsAsync<AggregateFieldValidationException>(
                () => _userService.CreateUserAsync(Guid.NewGuid().ToString(), dto));

            Assert.Contains(exception.Errors,
                e => e.Field == "JoinedDate" && e.Message.Contains("under the age of 18"));
        }

        [Theory]
        [InlineData("2023-05-27T00:00:00", "Saturday")]
        [InlineData("2023-05-28T00:00:00", "Sunday")]
        public async Task CreateUserAsync_JoinedDateOnWeekend_ThrowsFieldValidationException(
            string joinedDate, string testName)
        {
            // Arrange
            var dto = new CreateUserRequestDto
            {
                FirstName = "John",
                LastName = "Doe",
                DateOfBirth = "2000-01-01",
                JoinedDate = joinedDate,
                Type = UserTypeDtoEnum.Staff,
                Gender = GenderDtoEnum.Male
            };

            // Act & Assert            
            var exception = await Assert.ThrowsAsync<AggregateFieldValidationException>(
                () => _userService.CreateUserAsync(Guid.NewGuid().ToString(), dto));

            Assert.Contains(exception.Errors, e => e.Field == "JoinedDate" && e.Message.Contains("Saturday or Sunday"));
        }

        #endregion

        #region CreateUserAsync Tests - User Type and Gender Validation

        [Theory]
        [InlineData(0, "Zero is Invalid")]
        [InlineData(99, "Out of Range")]
        public async Task CreateUserAsync_InvalidUserType_ThrowsFieldValidationException(
            int userType, string testName)
        {
            // Arrange
            var dto = new CreateUserRequestDto
            {
                FirstName = "John",
                LastName = "Doe",
                DateOfBirth = "2000-01-01",
                JoinedDate = "2023-05-22T00:00:00", // Monday
                Type = (UserTypeDtoEnum)userType,
                Gender = GenderDtoEnum.Male
            };

            // Act & Assert            
            var exception = await Assert.ThrowsAsync<AggregateFieldValidationException>(
                () => _userService.CreateUserAsync(Guid.NewGuid().ToString(), dto));

            Assert.Contains(exception.Errors, e => e.Field == "Type");
        }

        [Theory]
        [InlineData(0, "Zero is Invalid")]
        [InlineData(99, "Out of Range")]
        public async Task CreateUserAsync_InvalidGender_ThrowsFieldValidationException(
            int gender, string testName)
        {
            // Arrange
            var dto = new CreateUserRequestDto
            {
                FirstName = "John",
                LastName = "Doe",
                DateOfBirth = "2000-01-01",
                JoinedDate = "2023-05-22T00:00:00", // Monday
                Type = UserTypeDtoEnum.Staff,
                Gender = (GenderDtoEnum)gender
            };

            // Act & Assert
            var exception = await Assert.ThrowsAsync<AggregateFieldValidationException>(
                () => _userService.CreateUserAsync(Guid.NewGuid().ToString(), dto));

            Assert.Contains(exception.Errors, e => e.Field == "Gender");
        }

        #endregion

        #region CreateUserAsync Tests - Admin Location and User Generation        [Fact]

        public async Task CreateUserAsync_AdminNotFound_ThrowsUnauthorizedAccessException()
        {
            // Arrange
            _mockUserRepository.Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync((User)null);

            var dto = new CreateUserRequestDto
            {
                FirstName = "John",
                LastName = "Doe",
                DateOfBirth = "2000-01-01",
                JoinedDate = "2023-05-22T00:00:00", // Monday
                Type = UserTypeDtoEnum.Staff,
                Gender = GenderDtoEnum.Male
            };

            // Act & Assert
            var exception = await Assert.ThrowsAsync<UnauthorizedAccessException>(
                () => _userService.CreateUserAsync(Guid.NewGuid().ToString(), dto));

            Assert.Contains("Cannot find admin", exception.Message);
        }

        [Fact]
        public async Task CreateUserAsync_GeneratesProperUsername_WhenUsernameExists()
        {
            // Arrange
            var adminId = Guid.NewGuid().ToString();

            var adminUser = new User
            {
                Id = Guid.Parse(adminId),
                Username = "admin1",
                Password = "hashed_password",
                Location = LocationEnum.HCM
            };

            _mockUserRepository.Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(adminUser);

            // Existing username with same pattern
            _mockUserRepository.Setup(r => r.GetAllUsernamesAsync())
                .ReturnsAsync(new List<string> { "johnv" });

            _mockUserRepository.Setup(r => r.GetStaffCodesAsync())
                .ReturnsAsync(new List<string> { "SD0001" });

            _passwordHasherMock.Setup(h => h.HashPassword(It.IsAny<string>()))
                .Returns("hashed_password");

            var dto = new CreateUserRequestDto
            {
                FirstName = "John",
                LastName = "Vu",
                DateOfBirth = "2000-01-01",
                JoinedDate = "2023-05-22T00:00:00", // Monday
                Type = UserTypeDtoEnum.Staff,
                Gender = GenderDtoEnum.Male
            };

            // Act
            var result = await _userService.CreateUserAsync(adminId, dto);

            // Assert
            Assert.Equal("johnv1", result.Username); // Should increment with 1
        }

        [Fact]
        public async Task CreateUserAsync_GeneratesProperStaffCode_WhenCodesExist()
        {
            // Arrange
            var adminId = Guid.NewGuid().ToString();

            var adminUser = new User
            {
                Id = Guid.Parse(adminId),
                Username = "admin2",
                Password = "hashed_password",
                Location = LocationEnum.HCM
            };

            _mockUserRepository.Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(adminUser);
            
            _mockUserRepository.Setup(r => r.GetAllUsernamesAsync())
                .ReturnsAsync(new List<string>());

            // High existing staff code
            _mockUserRepository.Setup(r => r.GetStaffCodesAsync())
                .ReturnsAsync(new List<string> { "SD1234" });

            _passwordHasherMock.Setup(h => h.HashPassword(It.IsAny<string>()))
                .Returns("hashed_password");

            var dto = new CreateUserRequestDto
            {
                FirstName = "John",
                LastName = "Doe",
                DateOfBirth = "2000-01-01",
                JoinedDate = "2023-05-22T00:00:00", // Monday
                Type = UserTypeDtoEnum.Staff,
                Gender = GenderDtoEnum.Male
            };

            // Act
            var result = await _userService.CreateUserAsync(adminId, dto);

            // Assert
            Assert.Equal("SD1235", result.StaffCode); // Should increment by 1
        }

        [Fact]
        public async Task CreateUserAsync_SetsLocationFromAdmin()
        {
            // Arrange
            var adminUserId = Guid.NewGuid();
            var adminUser = new User
            {
                Id = adminUserId,
                Username = "admin",
                Password = "Pa$$w0rd",
                Type = UserTypeEnum.Admin,
                Location = LocationEnum.DN
            };

            _mockUserRepository.Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(adminUser);

            _mockUserRepository.Setup(r => r.GetAllUsernamesAsync())
                .ReturnsAsync(new List<string>());

            _mockUserRepository.Setup(r => r.GetStaffCodesAsync())
                .ReturnsAsync(new List<string> { "SD0001" });

            _passwordHasherMock.Setup(h => h.HashPassword(It.IsAny<string>()))
                .Returns("hashed_password");

            var dto = new CreateUserRequestDto
            {
                FirstName = "John",
                LastName = "Doe",
                DateOfBirth = "2000-01-01",
                JoinedDate = "2023-05-22T00:00:00", // Monday
                Type = UserTypeDtoEnum.Staff,
                Gender = GenderDtoEnum.Male
            };

            // Act
            var result = await _userService.CreateUserAsync(adminUserId.ToString(), dto);

            // Assert
            Assert.Equal(LocationDtoEnum.DN, result.Location); // Should be set from admin
        }

        #endregion

        #region CreateUserAsync Tests - Success Cases

        [Theory]
        [InlineData("2000-01-01", "2023-05-22T00:00:00", "Standard Case")]
        [InlineData("2007-05-22", "2025-05-22T00:00:00", "Just Turned 18")] // User turns 18 on joining date
        [InlineData("1990-01-01", "2023-05-22T00:00:00", "Older User")]
        public async Task CreateUserAsync_ValidInput_Success(
            string dateOfBirth, string joinedDate, string testName)
        {
            // Arrange
            var adminUserId = Guid.NewGuid();
            var adminUser = new User
            {
                Id = adminUserId,
                Username = "admin",
                Password = "Pa$$w0rd",
                Type = UserTypeEnum.Admin,
                Location = LocationEnum.HN
            };

            _mockUserRepository.Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(adminUser);

            _mockUserRepository.Setup(r => r.GetAllUsernamesAsync())
                .ReturnsAsync(new List<string>());

            _mockUserRepository.Setup(r => r.GetStaffCodesAsync())
                .ReturnsAsync(new List<string> { "SD0001" });

            _passwordHasherMock.Setup(h => h.HashPassword(It.IsAny<string>()))
                .Returns("hashed_password");

            var dto = new CreateUserRequestDto
            {
                FirstName = "John",
                LastName = "Doe",
                DateOfBirth = dateOfBirth,
                JoinedDate = joinedDate,
                Type = UserTypeDtoEnum.Admin, // Test with Admin role
                Gender = GenderDtoEnum.Male
            };

            // Act
            var result = await _userService.CreateUserAsync(adminUserId.ToString(), dto);

            // Assert
            Assert.Equal("SD0002", result.StaffCode);
            Assert.Equal("johnd", result.Username);
            Assert.Equal("Doe John", result.FullName);
            Assert.Equal(LocationDtoEnum.HN, result.Location);

            _mockUserRepository.Verify(r => r.Add(It.IsAny<User>()), Times.Once);
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

            var service = new UserService(mockRepo.Object, _passwordHasherMock.Object);

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
            var service = new UserService(mockRepo.Object, _passwordHasherMock.Object);

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
            var service = new UserService(mockRepo.Object, _passwordHasherMock.Object);

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
            var service = new UserService(mockRepo.Object, _passwordHasherMock.Object);

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
            var service = new UserService(mockRepo.Object, _passwordHasherMock.Object);

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

            var service = new UserService(mockRepo.Object, _passwordHasherMock.Object);

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

            var service = new UserService(mockRepo.Object, _passwordHasherMock.Object);

            // Act
            var result = await service.DeleteUser(deletedBy, user.StaffCode);

            // Assert
            Assert.Equal(user.StaffCode, result);
            Assert.False(user.IsActive);
            Assert.Equal(deletedBy, user.DeletedBy);
            Assert.NotNull(user.DeletedDate);

            mockRepo.Verify(r => r.Update(It.IsAny<User>()), Times.Once);
        }


        #endregion

        [Fact]
        public async Task GetByStaffCodeAsync_ValidStaffCode_ReturnsUserDetails()
        {
            // Arrange
            var staffCode = "SD0001";
            var user = new User
            {
                StaffCode = staffCode,
                FirstName = "John",
                LastName = "Doe",
                Username = "johndoe",
                Password = "Pa$$w0rd",
                DateOfBirth = new DateTime(1990, 1, 1),
                JoinedDate = DateTimeOffset.Parse("2023-01-15"),
                Type = UserTypeEnum.Staff,
                Location = LocationEnum.HCM
            };

            _mockUserRepository.Setup(r => r.GetByStaffCodeAsync(staffCode)).ReturnsAsync(user);

            // Act
            var result = await _userService.GetByStaffCodeAsync(staffCode);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(staffCode, result.StaffCode);
            Assert.Equal("John", result.FirstName);
            Assert.Equal("Doe", result.LastName);
            Assert.Equal("johndoe", result.Username);
            Assert.Equal("01/01/1990", result.DateOfBirth);
            Assert.Equal("15/01/2023", result.JoinedDate);
            Assert.Equal((int)UserTypeEnum.Staff, result.Type);
            Assert.Equal((int)LocationEnum.HCM, result.Location);
        }

        [Fact]
        public async Task GetByStaffCodeAsync_InvalidStaffCode_ThrowsKeyNotFoundException()
        {
            // Arrange
            var staffCode = "InvalidCode";

            _mockUserRepository.Setup(r => r.GetByStaffCodeAsync(staffCode))
                .ReturnsAsync((User?)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<KeyNotFoundException>(
                () => _userService.GetByStaffCodeAsync(staffCode));

            Assert.Contains(staffCode, exception.Message);
        }

        [Fact]
        public async Task GetByStaffCodeAsync_EmptyStaffCode_ThrowsArgumentException()
        {
            // Arrange
            string emptyStaffCode = string.Empty;

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(
                () => _userService.GetByStaffCodeAsync(emptyStaffCode));
            
            Assert.Contains("Staff code cannot be empty", exception.Message);
            Assert.Equal("staffCode", exception.ParamName);
        }
        
        [Fact]
        public async Task GetByStaffCodeAsync_NullStaffCode_ThrowsArgumentException()
        {
            // Arrange
            string? nullStaffCode = null;

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(
                () => _userService.GetByStaffCodeAsync(nullStaffCode!));
            
            Assert.Contains("Staff code cannot be empty", exception.Message);
            Assert.Equal("staffCode", exception.ParamName);
        }
        
        [Fact]
        public async Task GetByStaffCodeAsync_UserWithNullDateOfBirth_ReturnsUserDetailsWithNullDateOfBirth()
        {
            // Arrange
            var staffCode = "SD0001";
            var user = new User
            {
                StaffCode = staffCode,
                FirstName = "John",
                LastName = "Doe",
                Username = "johndoe",
                Password = "Pa$$w0rd",
                DateOfBirth = null, // Null date of birth
                JoinedDate = DateTimeOffset.Parse("2023-01-15"),
                Type = UserTypeEnum.Staff,
                Location = LocationEnum.HCM
            };

            _mockUserRepository.Setup(r => r.GetByStaffCodeAsync(staffCode)).ReturnsAsync(user);

            // Act
            var result = await _userService.GetByStaffCodeAsync(staffCode);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(staffCode, result.StaffCode);
            Assert.Null(result.DateOfBirth); // Date of birth should be null
        }
        
        [Fact]
        public async Task GetByStaffCodeAsync_UserWithEmptyName_ReturnsUserDetailsWithEmptyName()
        {
            // Arrange
            var staffCode = "SD0001";
            var user = new User
            {
                StaffCode = staffCode,
                FirstName = "", // Empty first name
                LastName = "", // Empty last name
                Username = "johndoe",
                Password = "Pa$$w0rd",
                DateOfBirth = new DateTime(1990, 1, 1),
                JoinedDate = DateTimeOffset.Parse("2023-01-15"),
                Type = UserTypeEnum.Staff,
                Location = LocationEnum.HCM
            };

            _mockUserRepository.Setup(r => r.GetByStaffCodeAsync(staffCode)).ReturnsAsync(user);

            // Act
            var result = await _userService.GetByStaffCodeAsync(staffCode);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(staffCode, result.StaffCode);
            Assert.Equal("", result.FirstName);
            Assert.Equal("", result.LastName);
        }
    }
}