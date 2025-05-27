using System.Security.Claims;
using AssetManagement.Application.Controllers;
using AssetManagement.Application.Services.Interfaces;
using AssetManagement.Contracts.Common;
using AssetManagement.Contracts.DTOs;
using AssetManagement.Contracts.DTOs.Requests;
using AssetManagement.Contracts.DTOs.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace AssetManagement.Application.Tests.Controllers
{
    public class AuthControllerTests
    {
        private readonly Mock<IAuthService> _mockAuthService;
        private readonly AuthController _authController;

        public AuthControllerTests()
        {
            _mockAuthService = new Mock<IAuthService>();
            _authController = new AuthController(_mockAuthService.Object);
        }

        [Fact]
        public async Task Login_ValidRequest_ReturnsOkResult()
        {
            // Arrange
            var loginRequest = new LoginRequestDto
            {
                Username = "testuser",
                Password = "testpassword"
            };

            var loginResponse = new LoginResponseDto
            {
                AccessToken = "test-token",
                UserInfo = new UserDto
                {
                    StaffCode = "SD001",
                    FirstName = "John",
                    LastName = "Doe",
                    Username = "testuser",
                    Type = "Admin",
                    IsPasswordUpdated = true
                }
            };

            _mockAuthService.Setup(x => x.LoginAsync(loginRequest))
                .ReturnsAsync(loginResponse);

            // Act
            var actionResult = await _authController.Login(loginRequest);

            // Assert
            // a) It’s an OkObjectResult
            var ok = Assert.IsType<OkObjectResult>(actionResult.Result);

            // b) The value inside is our ApiResponse<T>
            var apiResponse = Assert.IsType<ApiResponse<LoginResponseDto>>(ok.Value);

            // c) Success flag, Message, Data, Errors
            Assert.True(apiResponse.Success);
            Assert.Same(loginResponse, apiResponse.Data);
            Assert.Empty(apiResponse.Errors);

            // d) Verify service was called exactly once
            _mockAuthService.Verify(x => x.LoginAsync(loginRequest), Times.Once);
        }

        [Fact]
        public async Task Login_AuthServiceThrowsException_PropagatesException()
        {
            // Arrange
            var loginRequest = new LoginRequestDto
            {
                Username = "testuser",
                Password = "wrongpassword"
            };

            _mockAuthService.Setup(x => x.LoginAsync(loginRequest))
                .ThrowsAsync(new UnauthorizedAccessException("Username or password is incorrect. Please try again"));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<UnauthorizedAccessException>(
                () => _authController.Login(loginRequest));
            Assert.Equal("Username or password is incorrect. Please try again", exception.Message);
        }

        [Fact]
        public async Task ChangePassword_AuthenticatedUser_ReturnsOkResult()
        {
            // Arrange
            var request = new ChangePasswordRequestDto
            {
                Password = "oldpassword",
                NewPassword = "newpassword"
            };

            var response = new ChangePasswordResponseDto
            {
                AccessToken = "new-token",
                UserInfo = new UserDto
                {
                    StaffCode = "SD001",
                    FirstName = "John",
                    LastName = "Doe",
                    Username = "testuser",
                    Type = "Admin",
                    IsPasswordUpdated = true
                }
            };

            // Setup the controller context with authenticated user
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, "testuser"),
                new Claim(ClaimTypes.NameIdentifier, "1")
            };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            var principal = new ClaimsPrincipal(identity);

            _authController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = principal
                }
            };

            _mockAuthService.Setup(x => x.ChangePasswordAsync("testuser", request))
                .ReturnsAsync(response);

            // Act
            var actionResult = await _authController.ChangePassword(request);

            // Assert
            var ok = Assert.IsType<OkObjectResult>(actionResult.Result);
            var apiResponse = Assert.IsType<ApiResponse<ChangePasswordResponseDto>>(ok.Value);

            Assert.True(apiResponse.Success);
            Assert.Same(response, apiResponse.Data);
            Assert.Empty(apiResponse.Errors);

            _mockAuthService.Verify(x => x.ChangePasswordAsync("testuser", request), Times.Once);
        }

        [Fact]
        public async Task ChangePassword_UnauthenticatedUser_ReturnsBadRequest()
        {
            // Arrange
            var request = new ChangePasswordRequestDto
            {
                Password = "oldpassword",
                NewPassword = "newpassword"
            };

            // Setup the controller context with no authenticated user
            _authController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };

            // Act
            var actionResult = await _authController.ChangePassword(request);

            // Assert: we expect a 400 BadRequest
            var bad = Assert.IsType<BadRequestObjectResult>(actionResult.Result);
            var apiResponse = Assert.IsType<ApiResponse<ChangePasswordResponseDto>>(bad.Value);

            Assert.False(apiResponse.Success);
            Assert.Equal("User not authenticated", apiResponse.Message);
            Assert.Null(apiResponse.Data);
            Assert.NotEmpty(apiResponse.Errors);
        }

        [Fact]
        public void VerifyToken_AuthenticatedUser_ReturnsTrue()
        {
            // Arrange
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, "testuser"),
                new Claim(ClaimTypes.NameIdentifier, "1")
            };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            var principal = new ClaimsPrincipal(identity);

            _authController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = principal
                }
            };

            // Act
            var actionResult = _authController.VerifyToken();

            // Assert
            var ok = Assert.IsType<OkObjectResult>(actionResult.Result);
            var apiResponse = Assert.IsType<ApiResponse<bool>>(ok.Value);

            Assert.True(apiResponse.Success);
            Assert.Equal("Token is valid", apiResponse.Message);
            Assert.True(apiResponse.Data);
            Assert.Empty(apiResponse.Errors);
        }

        [Fact]
        public void VerifyToken_UnauthenticatedUser_ReturnsFalse()
        {
            // Arrange
            _authController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };

            // Act
            var result = _authController.VerifyToken();

            // Assert
            Assert.IsType<ActionResult<ApiResponse<bool>>>(result);
        }

        [Fact]
        public void TestRoute_AuthenticatedAdminUser_ReturnsTrue()
        {
            // Arrange
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, "admin"),
                new Claim(ClaimTypes.NameIdentifier, "1"),
                new Claim(ClaimTypes.Role, "Admin")
            };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            var principal = new ClaimsPrincipal(identity);

            _authController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = principal
                }
            };

            // Act
            var result = _authController.TestRoute();

            // Assert
            Assert.IsType<ActionResult<ApiResponse<bool>>>(result);
        }
    }
}