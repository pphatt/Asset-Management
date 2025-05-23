using System.Security.Claims;
using AssetManagement.Application.Services.Interfaces;
using AssetManagement.Contracts.Common.Pagination;
using AssetManagement.Contracts.DTOs;
using AssetManagement.Contracts.DTOs.Requests;
using AssetManagement.Contracts.DTOs.Responses;
using AssetManagement.Contracts.Parameters;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace AssetManagement.Application.Controllers.Tests
{
    public class UsersControllerTests
    {
        private readonly Mock<IUserService> _userServiceMock;
        private readonly UsersController _controller;

        public UsersControllerTests()
        {
            _userServiceMock = new Mock<IUserService>();
            _controller = new UsersController(_userServiceMock.Object);

            // Mock HttpContext User
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Role, "Admin")
            }, "mock"));
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };
        }

        [Fact]
        public async Task Get_ReturnsPagedUsers_WhenAuthorized()
        {
            // Arrange
            var dtoList = new List<UserDto> { new UserDto { FirstName = "A", LastName = "B", JoinedDate = DateTimeOffset.Now, StaffCode = "S1", Type = "Staff", Username = "u" } };
            var paged = new PagedResult<UserDto>(dtoList, dtoList.Count, 10, 1);
            _userServiceMock.Setup(s => s.GetUsersAsync(It.IsAny<string>(), It.IsAny<UserQueryParameters>())).ReturnsAsync(paged);

            // Act
            var actionResult = await _controller.Get(new UserQueryParameters { PageNumber = 1, PageSize = 10 });
            var okResult = Assert.IsType<ActionResult<ApiResponse<PagedResult<UserDto>>>>(actionResult);
            var response = okResult.Value;

            // Assert
            Assert.NotNull(response);
            Assert.True(response.Success);
            Assert.Equal(paged, response.Data);
        }

        [Fact]
        public async Task Get_ThrowsInvalidOperationException_WhenNoUserId()
        {
            // Arrange: remove claim
            _controller.ControllerContext.HttpContext.User = new ClaimsPrincipal();

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(
                () => _controller.Get(new UserQueryParameters()));
        }
        
        [Fact]
        public async Task Create_ReturnsCreatedUser_WhenValidInput()
        {
            // Arrange
            var adminId = _controller.User.FindFirst(ClaimTypes.NameIdentifier)?.Value!;

            var requestDto = new CreateUserRequestDto
            {
                FirstName = "Test",
                LastName = "User",
                DateOfBirth = new DateTime(2000, 1, 1).ToString("yyyy-MM-dd"),
                JoinedDate = DateTimeOffset.UtcNow.ToString("yyyy-MM-dd"),
                Gender = Contracts.Enums.GenderDtoEnum.Male,
                Type = Contracts.Enums.UserTypeDtoEnum.Staff
            };

            var responseDto = new CreateUserResponseDto
            {
                StaffCode = "SD0001",
                Username = "testuser",
                FullName = "User Test",
                Location = Contracts.Enums.LocationDtoEnum.HCM
            };

            _userServiceMock
                .Setup(s => s.CreateUserAsync(adminId, requestDto))
                .ReturnsAsync(responseDto);

            // Act
            var result = await _controller.Create(requestDto);

            // Assert
            var createdResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(StatusCodes.Status201Created, createdResult.StatusCode);

            var apiResponse = Assert.IsType<ApiResponse<CreateUserResponseDto>>(createdResult.Value);
            Assert.True(apiResponse.Success);
            Assert.Equal("Successfully created a new user", apiResponse.Message);
            Assert.Equal(responseDto, apiResponse.Data);
        }

        [Fact]
        public async Task Create_ThrowsUnauthorizedAccess_WhenMissingUserId()
        {
            // Arrange: remove claims
            _controller.ControllerContext.HttpContext.User = new ClaimsPrincipal();

            // Act & Assert
            await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
                _controller.Create(new CreateUserRequestDto()));
        }

        [Fact]
        public async Task GetByStaffCode_ValidStaffCode_ReturnsUser()
        {
            // Arrange
            var staffCode = "SD0001";
            var userDto = new UserDetailsDto
            {
                StaffCode = staffCode,
                FirstName = "John",
                LastName = "Doe",
                Username = "johndoe",
                DateOfBirth = "01/01/1990",
                JoinedDate = "15/01/2023",
                Type = 1,
                Location = 1
            };

            // Act
            _userServiceMock.Setup(s => s.GetByStaffCodeAsync(staffCode)).ReturnsAsync(userDto);
            var actionResult = await _controller.GetByStaffCode(staffCode);
            var result = Assert.IsType<ActionResult<ApiResponse<UserDetailsDto>>>(actionResult);
            var okObjectResult = Assert.IsType<OkObjectResult>(result.Result);
            var response = Assert.IsType<ApiResponse<UserDetailsDto>>(okObjectResult.Value);

            // Assert
            Assert.NotNull(response);
            Assert.True(response.Success);
            Assert.Equal(userDto, response.Data);
        }

        [Fact]
        public async Task GetByStaffCode_InvalidStaffCode_ReturnsNotFound()
        {
            // Arrange
            var staffCode = "InvalidCode";
            _userServiceMock.Setup(s => s.GetByStaffCodeAsync(staffCode))
                .ThrowsAsync(new KeyNotFoundException($"Cannot find user with staff code {staffCode}"));

            // Act
            var actionResult = await _controller.GetByStaffCode(staffCode);
            var result = Assert.IsType<ActionResult<ApiResponse<UserDetailsDto>>>(actionResult);
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
            var response = Assert.IsType<ApiResponse<UserDetailsDto>>(notFoundResult.Value);

            // Assert
            Assert.NotNull(response);
            Assert.False(response.Success);
            Assert.Contains(staffCode, response.Message);
        }

        [Fact]
        public async Task GetByStaffCode_EmptyStaffCode_ReturnsBadRequest()
        {
            // Arrange
            var staffCode = string.Empty;

            // Act
            var actionResult = await _controller.GetByStaffCode(staffCode);
            var result = Assert.IsType<ActionResult<ApiResponse<UserDetailsDto>>>(actionResult);
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            var response = Assert.IsType<ApiResponse<UserDetailsDto>>(badRequestResult.Value);

            // Assert
            Assert.NotNull(response);
            Assert.False(response.Success);
            Assert.Contains("Staff code cannot be empty", response.Message);
        }

        [Fact]
        public async Task GetByStaffCode_NullStaffCode_ReturnsBadRequest()
        {
            // Arrange
            string? staffCode = null;

            // Act
            var actionResult = await _controller.GetByStaffCode(staffCode!);
            var result = Assert.IsType<ActionResult<ApiResponse<UserDetailsDto>>>(actionResult);
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            var response = Assert.IsType<ApiResponse<UserDetailsDto>>(badRequestResult.Value);

            // Assert
            Assert.NotNull(response);
            Assert.False(response.Success);
            Assert.Contains("Staff code cannot be empty", response.Message);
        }

        [Fact]
        public async Task GetByStaffCode_ServiceThrowsArgumentException_ReturnsBadRequest()
        {
            // Arrange
            var staffCode = "SD0001";
            var errorMessage = "Invalid staff code format";
            _userServiceMock.Setup(s => s.GetByStaffCodeAsync(staffCode))
                .ThrowsAsync(new ArgumentException(errorMessage));

            // Act
            var actionResult = await _controller.GetByStaffCode(staffCode);
            var result = Assert.IsType<ActionResult<ApiResponse<UserDetailsDto>>>(actionResult);
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            var response = Assert.IsType<ApiResponse<UserDetailsDto>>(badRequestResult.Value);

            // Assert
            Assert.NotNull(response);
            Assert.False(response.Success);
            Assert.Contains(errorMessage, response.Message);
        }

        [Fact]
        public async Task GetByStaffCode_ServiceThrowsGenericException_ReturnsErrorResponse()
        {
            // Arrange
            var staffCode = "SD0001";
            var errorMessage = "Database connection failed";
            _userServiceMock.Setup(s => s.GetByStaffCodeAsync(staffCode))
                .ThrowsAsync(new Exception(errorMessage));

            // Act
            var actionResult = await _controller.GetByStaffCode(staffCode);
            var result = Assert.IsType<ActionResult<ApiResponse<UserDetailsDto>>>(actionResult);
            var statusCodeResult = Assert.IsType<ObjectResult>(result.Result);
            var response = Assert.IsType<ApiResponse<UserDetailsDto>>(statusCodeResult.Value);

            // Assert
            Assert.NotNull(response);
            Assert.False(response.Success);
            Assert.Equal(StatusCodes.Status500InternalServerError, statusCodeResult.StatusCode);
            Assert.Contains(errorMessage, response.Message);
        }
    }
}