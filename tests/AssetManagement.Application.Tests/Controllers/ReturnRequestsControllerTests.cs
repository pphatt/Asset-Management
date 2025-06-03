using Moq;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using AssetManagement.Application.Services.Interfaces;
using AssetManagement.Application.Controllers;
using AssetManagement.Contracts.Parameters;
using AssetManagement.Contracts.Common.Pagination;
using AssetManagement.Contracts.DTOs;
using AssetManagement.Contracts.Common;

namespace AssetManagement.Application.Tests.Controllers
{
    public class ReturnRequestsControllerTests
    {
        private readonly Mock<IReturnRequestService> _serviceMock;
        private readonly ReturnRequestsController _controller;
        private readonly Guid _adminId = Guid.NewGuid();

        public ReturnRequestsControllerTests()
        {
            _serviceMock = new Mock<IReturnRequestService>();
            _controller = new ReturnRequestsController(_serviceMock.Object);
        }

        private ClaimsPrincipal CreateUserPrincipal()
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, _adminId.ToString()),
            };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            return new ClaimsPrincipal(identity);
        }

        private void ApplyUserToController(ClaimsPrincipal user)
        {
            _controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };
        }

        [Fact]
        public async Task Get_ReturnsOk_WithPagedResult()
        {
            // Arrange
            var queryParams = new ReturnRequestQueryParameters { PageNumber = 1, PageSize = 10 };
            var pagedResult = new PagedResult<ReturnRequestDto>(new List<ReturnRequestDto>(), 0, 10, 1);
            _serviceMock.Setup(s => s.GetReturnRequestsAsync(_adminId, queryParams)).ReturnsAsync(pagedResult);

            var claimsPrincipal = CreateUserPrincipal();
            ApplyUserToController(claimsPrincipal);

            // Act
            var result = await _controller.Get(queryParams);

            // Assert
            var okResult = Assert.IsType<ActionResult<ApiResponse<PagedResult<ReturnRequestDto>>>>(result);
            var okObjectResult = Assert.IsType<OkObjectResult>(okResult.Result);
            var apiResponse = Assert.IsType<ApiResponse<PagedResult<ReturnRequestDto>>>(okObjectResult.Value);
            Assert.True(apiResponse.Success);
            Assert.Equal("Successfully fetched a paginated list of return requests", apiResponse.Message);
            Assert.Equal(pagedResult, apiResponse.Data);
        }

        [Fact]
        public async Task Get_ThrowsUnauthorizedAccessException_WhenUserIdIsMissing()
        {
            // Arrange
            ApplyUserToController(new ClaimsPrincipal());

            // Act & Assert
            await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _controller.Get(new ReturnRequestQueryParameters()));
        }
    }
}