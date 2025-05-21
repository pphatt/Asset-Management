using System.Security.Claims;
using AssetManagement.Application.Services.Interfaces;
using AssetManagement.Contracts.Common.Pagination;
using AssetManagement.Contracts.DTOs;
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
    }
}