using AssetManagement.Contracts.DTOs.Requests;
using AssetManagement.Application.Services;
using AssetManagement.Contracts.Enums;
using AssetManagement.Contracts.Exceptions;
using AssetManagement.Contracts.Parameters;
using AssetManagement.Data.Helpers.Hashing;
using AssetManagement.Domain.Entities;
using AssetManagement.Domain.Enums;
using MockQueryable;
using Moq;
using static AssetManagement.Contracts.Exceptions.ApiExceptionTypes;
using AssetManagement.Domain.Interfaces.Repositories;

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
                Type = UserType.Admin,
                Location = Location.HCM
            };

            var users = new List<User>
            {
                new User
                {
                    Id = Guid.NewGuid(), FirstName = "John", LastName = "Doe",
                    Password = "Pa$$w0rd", StaffCode = "SD0001", Type = UserType.Staff, Location = Location.HCM,
                    JoinedDate = DateTimeOffset.UtcNow, Username = "johndoe"
                },
                new User
                {
                    Id = Guid.NewGuid(), FirstName = "Jane", LastName = "Smith",
                    Password = "Pa$$w0rd", StaffCode = "SD0002", Type = UserType.Admin, Location = Location.HCM,
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
        [InlineData(UserTypeDto.Admin, UserType.Admin)]
        [InlineData(UserTypeDto.Staff, UserType.Staff)]
        public void MapUserType_ShouldMapCorrectly(UserTypeDto input, UserType expected)
        {
            var result = typeof(UserService).GetMethod("MapUserType",
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)!
                .Invoke(null, new object[] { input });
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData(GenderDto.Male, Gender.Male)]
        [InlineData(GenderDto.Female, Gender.Female)]
        public void MapGender_ShouldMapCorrectly(GenderDto input, Gender expected)
        {
            var result = typeof(UserService).GetMethod("MapGender",
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)!
                .Invoke(null, new object[] { input });
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData(Location.HCM, LocationDto.HCM)]
        [InlineData(Location.DN, LocationDto.DN)]
        [InlineData(Location.HN, LocationDto.HN)]
        public void MapLocationToDto_ShouldMapCorrectly(Location input, LocationDto expected)
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
                Type = UserTypeDto.Staff,
                Gender = GenderDto.Male
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
                Type = UserTypeDto.Staff,
                Gender = GenderDto.Male
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
                Type = UserTypeDto.Staff,
                Gender = GenderDto.Male
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
                Type = UserTypeDto.Staff,
                Gender = GenderDto.Male
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
                Location = Location.HCM
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
                Type = UserTypeDto.Staff,
                Gender = GenderDto.Male
            };

            // Act
            var result = await _userService.CreateUserAsync(adminId, dto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("SD0010", result.StaffCode); // từ SD0009 + 1
            Assert.Equal("vinhv", result.Username); // "Vinh" + "V" (từ "Vo")
            Assert.Equal("Vo Vinh", result.FullName);
            Assert.Equal(LocationDto.HCM, result.Location);
        }

        #region CreateUserAsync Tests - Input Validation

        [Fact]
        public async Task CreateUserAsync_NullRequest_ThrowsArgumentNullException()
        {
            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentNullException>(
                () => _userService.CreateUserAsync(Guid.NewGuid().ToString(), null!));

            Assert.Equal("dto", exception.ParamName);
        }

        [Theory]
        [InlineData("", "Doe", "Invalid FirstName")]
        [InlineData("John", "", "Invalid LastName")]
        [InlineData(null, "Doe", "Null FirstName")]
        [InlineData("John", null, "Null LastName")]
        public async Task CreateUserAsync_InvalidNames_ThrowsFieldValidationException(
            string? firstName, string? lastName, string _)
        {
            // Arrange
            var dto = new CreateUserRequestDto
            {
                FirstName = firstName!,
                LastName = lastName!,
                DateOfBirth = "2000-01-01",
                JoinedDate = "2023-05-20T00:00:00",
                Type = UserTypeDto.Staff,
                Gender = GenderDto.Male
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
            string? dateOfBirth, string _)
        {
            // Arrange
            var dto = new CreateUserRequestDto
            {
                FirstName = "John",
                LastName = "Doe",
                DateOfBirth = dateOfBirth!,
                JoinedDate = "2023-05-20T00:00:00",
                Type = UserTypeDto.Staff,
                Gender = GenderDto.Male
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
            string dateOfBirth, string _)
        {
            // Arrange
            var dto = new CreateUserRequestDto
            {
                FirstName = "John",
                LastName = "Doe",
                DateOfBirth = dateOfBirth,
                JoinedDate = "2023-05-20T00:00:00",
                Type = UserTypeDto.Staff,
                Gender = GenderDto.Male
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
            string dateOfBirth, string _)
        {
            // Arrange
            var dto = new CreateUserRequestDto
            {
                FirstName = "John",
                LastName = "Doe",
                DateOfBirth = dateOfBirth,
                JoinedDate = "2023-05-20T00:00:00",
                Type = UserTypeDto.Staff,
                Gender = GenderDto.Male
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
            string? joinedDate, string _)
        {
            // Arrange
            var dto = new CreateUserRequestDto
            {
                FirstName = "John",
                LastName = "Doe",
                DateOfBirth = "2000-01-01", // 18+ years old
                JoinedDate = joinedDate!,
                Type = UserTypeDto.Staff,
                Gender = GenderDto.Male
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
            string joinedDate, string _)
        {
            // Arrange
            var dto = new CreateUserRequestDto
            {
                FirstName = "John",
                LastName = "Doe",
                DateOfBirth = "2000-01-01",
                JoinedDate = joinedDate,
                Type = UserTypeDto.Staff,
                Gender = GenderDto.Male
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
                Type = UserTypeDto.Staff,
                Gender = GenderDto.Male
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
            string joinedDate, string _)
        {
            // Arrange
            var dto = new CreateUserRequestDto
            {
                FirstName = "John",
                LastName = "Doe",
                DateOfBirth = "2000-01-01",
                JoinedDate = joinedDate,
                Type = UserTypeDto.Staff,
                Gender = GenderDto.Male
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
            int userType, string _)
        {
            // Arrange
            var dto = new CreateUserRequestDto
            {
                FirstName = "John",
                LastName = "Doe",
                DateOfBirth = "2000-01-01",
                JoinedDate = "2023-05-22T00:00:00", // Monday
                Type = (UserTypeDto)userType,
                Gender = GenderDto.Male
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
            int gender, string _)
        {
            // Arrange
            var dto = new CreateUserRequestDto
            {
                FirstName = "John",
                LastName = "Doe",
                DateOfBirth = "2000-01-01",
                JoinedDate = "2023-05-22T00:00:00", // Monday
                Type = UserTypeDto.Staff,
                Gender = (GenderDto)gender
            };

            // Act & Assert
            var exception = await Assert.ThrowsAsync<AggregateFieldValidationException>(
                () => _userService.CreateUserAsync(Guid.NewGuid().ToString(), dto));

            Assert.Contains(exception.Errors, e => e.Field == "Gender");
        }

        #endregion

        #region CreateUserAsync Tests - Admin Location and User Generation

        [Fact]
        public async Task CreateUserAsync_AdminNotFound_ThrowsUnauthorizedAccessException()
        {
            // Arrange
            _mockUserRepository.Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync((User?)null);

            var dto = new CreateUserRequestDto
            {
                FirstName = "John",
                LastName = "Doe",
                DateOfBirth = "2000-01-01",
                JoinedDate = "2023-05-22T00:00:00", // Monday
                Type = UserTypeDto.Staff,
                Gender = GenderDto.Male
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
                Location = Location.HCM
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
                Type = UserTypeDto.Staff,
                Gender = GenderDto.Male
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
                Location = Location.HCM
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
                Type = UserTypeDto.Staff,
                Gender = GenderDto.Male
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
                Type = UserType.Admin,
                Location = Location.DN
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
                Type = UserTypeDto.Staff,
                Gender = GenderDto.Male
            };

            // Act
            var result = await _userService.CreateUserAsync(adminUserId.ToString(), dto);

            // Assert
            Assert.Equal(LocationDto.DN, result.Location); // Should be set from admin
        }

        #endregion

        #region CreateUserAsync Tests - Success Cases

        [Theory]
        [InlineData("2000-01-01", "2023-05-22T00:00:00", "Standard Case")]
        [InlineData("2007-05-22", "2025-05-22T00:00:00", "Just Turned 18")] // User turns 18 on joining date
        [InlineData("1990-01-01", "2023-05-22T00:00:00", "Older User")]
        public async Task CreateUserAsync_ValidInput_Success(
            string dateOfBirth, string joinedDate, string _)
        {
            // Arrange
            var adminUserId = Guid.NewGuid();
            var adminUser = new User
            {
                Id = adminUserId,
                Username = "admin",
                Password = "Pa$$w0rd",
                Type = UserType.Admin,
                Location = Location.HN
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
                Type = UserTypeDto.Admin, // Test with Admin role
                Gender = GenderDto.Male
            };

            // Act
            var result = await _userService.CreateUserAsync(adminUserId.ToString(), dto);

            // Assert
            Assert.Equal("SD0002", result.StaffCode);
            Assert.Equal("johnd", result.Username);
            Assert.Equal("Doe John", result.FullName);
            Assert.Equal(LocationDto.HN, result.Location);

            _mockUserRepository.Verify(r => r.AddAsync(It.IsAny<User>()), Times.Once);
        }

        private User CreateSampleUser(string staffCode = "SD0001")
        {
            return new User
            {
                Id = Guid.NewGuid(),
                StaffCode = staffCode,
                FirstName = "John",
                LastName = "Doe",
                Username = "testuser",
                Password = "hashedPassword",
                DateOfBirth = new DateTime(2000, 1, 1),
                JoinedDate = new DateTimeOffset(2020, 1, 6, 0, 0, 0, TimeSpan.Zero), // Monday
                Type = UserType.Admin,
                Gender = Gender.Male,
                Location = Location.HCM,
                IsActive = true,
                CreatedBy = Guid.NewGuid(),
                CreatedDate = DateTime.UtcNow.AddDays(-30)
            };
        }

        [Fact]
        public async Task UpdateUserAsync_ShouldThrow_NotFoundException_WhenUserNotFound()
        {
            // Arrange
            var adminUserId = Guid.NewGuid().ToString();
            var staffCode = "SD9999";
            var mockRepo = new Mock<IUserRepository>();
            mockRepo.Setup(x => x.GetByStaffCodeAsync(staffCode)).ReturnsAsync((User)null!);

            var service = new UserService(mockRepo.Object, _passwordHasherMock.Object);

            var dto = new UpdateUserRequestDto
            {
                DateOfBirth = "2000-01-01"
            };

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => service.UpdateUserAsync(adminUserId, staffCode, dto));
        }

        [Fact]
        public async Task UpdateUserAsync_ShouldThrow_InvalidOperationException_WhenUserInactive()
        {
            // Arrange
            var adminUserId = Guid.NewGuid().ToString();
            var staffCode = "SD0001";
            var user = CreateSampleUser(staffCode);
            user.IsActive = false;

            var mockRepo = new Mock<IUserRepository>();
            mockRepo.Setup(x => x.GetByStaffCodeAsync(staffCode)).ReturnsAsync(user);

            var service = new UserService(mockRepo.Object, _passwordHasherMock.Object);

            var dto = new UpdateUserRequestDto
            {
                DateOfBirth = "2000-01-01"
            };

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => service.UpdateUserAsync(adminUserId, staffCode, dto));
        }

        [Fact]
        public async Task UpdateUserAsync_ShouldThrow_AggregateFieldValidationException_WhenUserUnder18()
        {
            // Arrange
            var adminUserId = Guid.NewGuid().ToString();
            var staffCode = "SD0001";
            var user = CreateSampleUser(staffCode);

            var mockRepo = new Mock<IUserRepository>();
            mockRepo.Setup(x => x.GetByStaffCodeAsync(staffCode)).ReturnsAsync(user);

            var service = new UserService(mockRepo.Object, _passwordHasherMock.Object);

            var dto = new UpdateUserRequestDto
            {
                DateOfBirth = DateTime.UtcNow.AddYears(-17).ToString("yyyy-MM-dd") // under 18
            };

            // Act & Assert
            var exception = await Assert.ThrowsAsync<AggregateFieldValidationException>(() => service.UpdateUserAsync(adminUserId, staffCode, dto));

            Assert.Contains(exception.Errors, e => e.Field == "DateOfBirth" &&
                e.Message == "User is under 18. Please select a different date");
        }

        [Fact]
        public async Task UpdateUserAsync_ShouldThrow_AggregateFieldValidationException_WhenJoinedDateBefore18()
        {
            // Arrange
            var adminUserId = Guid.NewGuid().ToString();
            var staffCode = "SD0001";
            var user = CreateSampleUser(staffCode);
            user.DateOfBirth = new DateTime(2010, 1, 1);

            var mockRepo = new Mock<IUserRepository>();
            mockRepo.Setup(x => x.GetByStaffCodeAsync(staffCode)).ReturnsAsync(user);

            var service = new UserService(mockRepo.Object, _passwordHasherMock.Object);

            var dto = new UpdateUserRequestDto
            {
                JoinedDate = "2025-01-01" // before 18th birthday (person born in 2010)
            };

            // Act & Assert
            var exception = await Assert.ThrowsAsync<AggregateFieldValidationException>(() => service.UpdateUserAsync(adminUserId, staffCode, dto));

            Assert.Contains(exception.Errors, e => e.Field == "JoinedDate" &&
                e.Message == "User under the age of 18 may not join company. Please select a different date");
        }

        [Fact]
        public async Task UpdateUserAsync_ShouldThrow_AggregateFieldValidationException_WhenJoinedDateIsWeekend()
        {
            // Arrange
            var adminUserId = Guid.NewGuid().ToString();
            var staffCode = "SD0001";
            var user = CreateSampleUser(staffCode);

            var mockRepo = new Mock<IUserRepository>();
            mockRepo.Setup(x => x.GetByStaffCodeAsync(staffCode)).ReturnsAsync(user);

            var service = new UserService(mockRepo.Object, _passwordHasherMock.Object);

            var dto = new UpdateUserRequestDto
            {
                JoinedDate = "2025-05-17" // Saturday
            };

            // Act & Assert
            var exception = await Assert.ThrowsAsync<AggregateFieldValidationException>(() => service.UpdateUserAsync(adminUserId, staffCode, dto));

            Assert.Contains(exception.Errors, e => e.Field == "JoinedDate" &&
                e.Message == "Joined date is Saturday or Sunday. Please select a different date");
        }

        [Fact]
        public async Task UpdateUserAsync_ShouldThrow_AggregateFieldValidationException_WhenJoinedDateProvidedButDOBIsNull()
        {
            // Arrange
            var adminUserId = Guid.NewGuid().ToString();
            var staffCode = "SD0001";
            var user = CreateSampleUser(staffCode);
            user.DateOfBirth = null; // No DOB

            var mockRepo = new Mock<IUserRepository>();
            mockRepo.Setup(x => x.GetByStaffCodeAsync(staffCode)).ReturnsAsync(user);

            var service = new UserService(mockRepo.Object, _passwordHasherMock.Object);

            var dto = new UpdateUserRequestDto
            {
                JoinedDate = "2025-01-06" // Monday, valid day but DOB is null
            };

            // Act & Assert
            var exception = await Assert.ThrowsAsync<AggregateFieldValidationException>(() => service.UpdateUserAsync(adminUserId, staffCode, dto));

            Assert.Contains(exception.Errors, e => e.Field == "JoinedDate" &&
                e.Message == "Please Select Date of Birth");
        }

        [Fact]
        public async Task UpdateUserAsync_ShouldThrow_AggregateFieldValidationException_WhenInvalidDateFormat()
        {
            // Arrange
            var adminUserId = Guid.NewGuid().ToString();
            var staffCode = "SD0001";
            var user = CreateSampleUser(staffCode);

            var mockRepo = new Mock<IUserRepository>();
            mockRepo.Setup(x => x.GetByStaffCodeAsync(staffCode)).ReturnsAsync(user);

            var service = new UserService(mockRepo.Object, _passwordHasherMock.Object);

            var dto = new UpdateUserRequestDto
            {
                DateOfBirth = "invalid-date",
                JoinedDate = "also-invalid"
            };

            // Act & Assert
            var exception = await Assert.ThrowsAsync<AggregateFieldValidationException>(() => service.UpdateUserAsync(adminUserId, staffCode, dto));

            Assert.Contains(exception.Errors, e => e.Field == "DateOfBirth" &&
                e.Message == "Invalid Date of Birth format");
            Assert.Contains(exception.Errors, e => e.Field == "JoinedDate" &&
                e.Message == "Invalid Joined Date format");
        }

        [Fact]
        public async Task UpdateUserAsync_ShouldThrow_AggregateFieldValidationException_WhenInvalidEnumValues()
        {
            // Arrange
            var adminUserId = Guid.NewGuid().ToString();
            var staffCode = "SD0001";
            var user = CreateSampleUser(staffCode);

            var mockRepo = new Mock<IUserRepository>();
            mockRepo.Setup(x => x.GetByStaffCodeAsync(staffCode)).ReturnsAsync(user);

            var service = new UserService(mockRepo.Object, _passwordHasherMock.Object);

            var dto = new UpdateUserRequestDto
            {
                Type = (UserTypeDto)999, // Invalid enum value
                Gender = (GenderDto)999 // Invalid enum value
            };

            // Act & Assert
            var exception = await Assert.ThrowsAsync<AggregateFieldValidationException>(() => service.UpdateUserAsync(adminUserId, staffCode, dto));

            Assert.Contains(exception.Errors, e => e.Field == "Type" &&
                e.Message == "Invalid user type value");
            Assert.Contains(exception.Errors, e => e.Field == "Gender" &&
                e.Message == "Invalid gender value");
        }

        [Fact]
        public async Task UpdateUserAsync_ShouldUpdateDateOfBirthSuccessfully()
        {
            // Arrange
            var adminUserId = Guid.NewGuid().ToString();
            var staffCode = "SD0001";
            var user = CreateSampleUser(staffCode);
            var originalJoinedDate = user.JoinedDate;

            var mockRepo = new Mock<IUserRepository>();
            mockRepo.Setup(x => x.GetByStaffCodeAsync(staffCode)).ReturnsAsync(user);

            var service = new UserService(mockRepo.Object, _passwordHasherMock.Object);

            var newDateOfBirth = new DateTime(1995, 6, 15);
            var dto = new UpdateUserRequestDto
            {
                DateOfBirth = newDateOfBirth.ToString("yyyy-MM-dd")
            };

            // Act
            var result = await service.UpdateUserAsync(adminUserId, staffCode, dto);

            // Assert
            Assert.Equal(staffCode, result);
            Assert.Equal(newDateOfBirth, user.DateOfBirth);
            Assert.Equal(originalJoinedDate, user.JoinedDate); // Should remain unchanged
            Assert.Equal(Guid.Parse(adminUserId), user.LastModifiedBy);
            Assert.True((DateTime.UtcNow - user.LastModifiedDate).TotalSeconds < 5); // Recently updated

            mockRepo.Verify(r => r.Update(user), Times.Once);
        }

        [Fact]
        public async Task UpdateUserAsync_ShouldUpdateJoinedDateSuccessfully()
        {
            // Arrange
            var adminUserId = Guid.NewGuid().ToString();
            var staffCode = "SD0001";
            var user = CreateSampleUser(staffCode);
            var originalDateOfBirth = user.DateOfBirth;

            var mockRepo = new Mock<IUserRepository>();
            mockRepo.Setup(x => x.GetByStaffCodeAsync(staffCode)).ReturnsAsync(user);

            var service = new UserService(mockRepo.Object, _passwordHasherMock.Object);

            var newJoinedDate = new DateTimeOffset(2022, 3, 7, 0, 0, 0, TimeSpan.Zero); // Monday
            var dto = new UpdateUserRequestDto
            {
                JoinedDate = newJoinedDate.ToString("yyyy-MM-dd")
            };

            // Act
            var result = await service.UpdateUserAsync(adminUserId, staffCode, dto);

            // Assert
            Assert.Equal(staffCode, result);
            Assert.Equal(newJoinedDate.Date, user.JoinedDate.Date);
            Assert.Equal(originalDateOfBirth, user.DateOfBirth); // Should remain unchanged

            mockRepo.Verify(r => r.Update(user), Times.Once);
        }

        [Fact]
        public async Task UpdateUserAsync_ShouldUpdateTypeAndGenderSuccessfully()
        {
            // Arrange
            var adminUserId = Guid.NewGuid().ToString();
            var staffCode = "SD0001";
            var user = CreateSampleUser(staffCode);
            user.Type = UserType.Staff;
            user.Gender = Gender.Female;

            var mockRepo = new Mock<IUserRepository>();
            mockRepo.Setup(x => x.GetByStaffCodeAsync(staffCode)).ReturnsAsync(user);

            var service = new UserService(mockRepo.Object, _passwordHasherMock.Object);

            var dto = new UpdateUserRequestDto
            {
                Type = UserTypeDto.Admin,
                Gender = GenderDto.Male
            };

            // Act
            var result = await service.UpdateUserAsync(adminUserId, staffCode, dto);

            // Assert
            Assert.Equal(staffCode, result);
            Assert.Equal(UserType.Admin, user.Type);
            Assert.Equal(Gender.Male, user.Gender);

            mockRepo.Verify(r => r.Update(user), Times.Once);
        }

        [Fact]
        public async Task UpdateUserAsync_ShouldUpdateMultipleFieldsSuccessfully()
        {
            // Arrange
            var adminUserId = Guid.NewGuid().ToString();
            var staffCode = "SD0001";
            var user = CreateSampleUser(staffCode);

            var mockRepo = new Mock<IUserRepository>();
            mockRepo.Setup(x => x.GetByStaffCodeAsync(staffCode)).ReturnsAsync(user);

            var service = new UserService(mockRepo.Object, _passwordHasherMock.Object);

            var newDateOfBirth = new DateTime(1998, 12, 25);
            var newJoinedDate = new DateTimeOffset(2021, 5, 10, 0, 0, 0, TimeSpan.Zero); // Monday
            var dto = new UpdateUserRequestDto
            {
                DateOfBirth = newDateOfBirth.ToString("yyyy-MM-dd"),
                JoinedDate = newJoinedDate.ToString("yyyy-MM-dd"),
                Type = UserTypeDto.Staff,
                Gender = GenderDto.Female
            };

            // Act
            var result = await service.UpdateUserAsync(adminUserId, staffCode, dto);

            // Assert
            Assert.Equal(staffCode, result);
            Assert.Equal(newDateOfBirth, user.DateOfBirth);
            Assert.Equal(newJoinedDate.Date, user.JoinedDate.Date);
            Assert.Equal(UserType.Staff, user.Type);
            Assert.Equal(Gender.Female, user.Gender);

            mockRepo.Verify(r => r.Update(user), Times.Once);
        }

        [Fact]
        public async Task UpdateUserAsync_ShouldNotUpdateFieldsWhenNotProvided()
        {
            // Arrange
            var adminUserId = Guid.NewGuid().ToString();
            var staffCode = "SD0001";
            var user = CreateSampleUser(staffCode);
            var originalDateOfBirth = user.DateOfBirth;
            var originalJoinedDate = user.JoinedDate;
            var originalType = user.Type;
            var originalGender = user.Gender;

            var mockRepo = new Mock<IUserRepository>();
            mockRepo.Setup(x => x.GetByStaffCodeAsync(staffCode)).ReturnsAsync(user);

            var service = new UserService(mockRepo.Object, _passwordHasherMock.Object);

            var dto = new UpdateUserRequestDto(); // Empty DTO

            // Act
            var result = await service.UpdateUserAsync(adminUserId, staffCode, dto);

            // Assert
            Assert.Equal(staffCode, result);
            Assert.Equal(originalDateOfBirth, user.DateOfBirth);
            Assert.Equal(originalJoinedDate, user.JoinedDate);
            Assert.Equal(originalType, user.Type);
            Assert.Equal(originalGender, user.Gender);

            // Should still update audit fields
            Assert.Equal(Guid.Parse(adminUserId), user.LastModifiedBy);
            Assert.True((DateTime.UtcNow - user.LastModifiedDate).TotalSeconds < 5);

            mockRepo.Verify(r => r.Update(user), Times.Once);
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
                service.DeleteUserAsync(Guid.NewGuid(), staffCode));
        }

        [Fact]
        public async Task DeleteUser_ShouldSetIsActiveFalse_AndCallUpdate()
        {
            // Arrange
            var deletedBy = Guid.NewGuid();
            var staffCode = "SD0001";
            var user = CreateSampleUser(staffCode);

            // Create a mock queryable that supports async operations
            var usersMock = new List<User> { user }.AsQueryable().BuildMock();

            var mockRepo = new Mock<IUserRepository>();
            // Set up GetAll to return the mock queryable that supports async
            mockRepo.Setup(x => x.GetAll()).Returns(usersMock);
            mockRepo.Setup(x => x.Update(It.IsAny<User>()));

            var service = new UserService(mockRepo.Object, _passwordHasherMock.Object);

            // Act
            var result = await service.DeleteUserAsync(deletedBy, user.StaffCode);

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
                Type = UserType.Staff,
                Location = Location.HCM
            };

            _mockUserRepository.Setup(r => r.GetByStaffCodeAsync(staffCode)).ReturnsAsync(user);

            // Act
            var result = await _userService.GetUserByStaffCodeAsync(staffCode);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(staffCode, result.StaffCode);
            Assert.Equal("John", result.FirstName);
            Assert.Equal("Doe", result.LastName);
            Assert.Equal("johndoe", result.Username);
            Assert.Equal("01/01/1990", result.DateOfBirth);
            Assert.Equal("15/01/2023", result.JoinedDate);
            Assert.Equal((int)UserType.Staff, result.Type);
            Assert.Equal((int)Location.HCM, result.Location);
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
                () => _userService.GetUserByStaffCodeAsync(staffCode));

            Assert.Contains(staffCode, exception.Message);
        }

        [Fact]
        public async Task GetByStaffCodeAsync_EmptyStaffCode_ThrowsArgumentException()
        {
            // Arrange
            string emptyStaffCode = string.Empty;

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(
                () => _userService.GetUserByStaffCodeAsync(emptyStaffCode));

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
                () => _userService.GetUserByStaffCodeAsync(nullStaffCode!));

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
                Type = UserType.Staff,
                Location = Location.HCM
            };

            _mockUserRepository.Setup(r => r.GetByStaffCodeAsync(staffCode)).ReturnsAsync(user);

            // Act
            var result = await _userService.GetUserByStaffCodeAsync(staffCode);

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
                Type = UserType.Staff,
                Location = Location.HCM
            };

            _mockUserRepository.Setup(r => r.GetByStaffCodeAsync(staffCode)).ReturnsAsync(user);

            // Act
            var result = await _userService.GetUserByStaffCodeAsync(staffCode);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(staffCode, result.StaffCode);
            Assert.Equal("", result.FirstName);
            Assert.Equal("", result.LastName);
        }
    }
}