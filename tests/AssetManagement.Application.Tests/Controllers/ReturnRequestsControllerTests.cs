using AssetManagement.Application.Controllers;
using AssetManagement.Application.Services.Interfaces;
using AssetManagement.Contracts.Common;
using AssetManagement.Contracts.Common.Pagination;
using AssetManagement.Contracts.DTOs;
using AssetManagement.Contracts.DTOs.Requests;
using AssetManagement.Contracts.DTOs.Responses;
using AssetManagement.Contracts.Parameters;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security.Claims;

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

        [Fact]
        public async Task CancelReturnRequest_ReturnsOk_WithReturnRequestDto()
        {
            // Arrange
            var returnRequestId = Guid.NewGuid();
            var returnRequestDto = new ReturnRequestDto { Id = returnRequestId, AssetCode = "A123" };
            _serviceMock.Setup(s => s.CancelReturnRequestAsync(returnRequestId, _adminId)).ReturnsAsync(returnRequestDto);

            var claimsPrincipal = CreateUserPrincipal();
            ApplyUserToController(claimsPrincipal);

            // Act
            var result = await _controller.CancelReturnRequest(returnRequestId);

            // Assert
            var okResult = Assert.IsType<ActionResult<ApiResponse<ReturnRequestDto>>>(result);
            var okObjectResult = Assert.IsType<OkObjectResult>(okResult.Result);
            var apiResponse = Assert.IsType<ApiResponse<ReturnRequestDto>>(okObjectResult.Value);
            Assert.True(apiResponse.Success);
            Assert.Equal("Successfully cancelled the return request", apiResponse.Message);
            Assert.Equal(returnRequestDto, apiResponse.Data);
        }

        [Fact]
        public async Task CancelReturnRequest_WithNoUserIdClaim_ThrowsUnauthorizedAccessException()
        {
            // Arrange
            var returnRequestId = Guid.NewGuid();

            // Setup controller with ClaimsPrincipal that has no NameIdentifier claim
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] {
                new Claim(ClaimTypes.Role, "Admin")
            }));

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };

            // Act & Assert
            await Assert.ThrowsAsync<UnauthorizedAccessException>(
                async () => await _controller.CancelReturnRequest(returnRequestId));
        }
        #region Create asset's returning request tests

        [Fact]
        public async Task Create_ReturnsOk_WithApiResponse_WhenRequestIsValid()
        {
            var request = new CreateReturnRequestDto { AssignmentId = "assignment-guid" };
            var responseDto = new CreateReturnRequestResponseDto 
            { 
                AssetCode = "A001", 
                AssignmentStatus = "Waiting for returning" 
            };
            var role = "Admin";
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, _adminId.ToString()),
                new(ClaimTypes.Role, role)
            };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var claimsPrincipal = new ClaimsPrincipal(identity);
            ApplyUserToController(claimsPrincipal);

            _serviceMock.Setup(s => s.CreateReturnRequestAsync(request.AssignmentId, 
                _adminId.ToString(), role))
                .ReturnsAsync(responseDto);

            var result = await _controller.Create(request);

            var actionResult = Assert.IsType<ActionResult<ApiResponse<CreateReturnRequestResponseDto>>>(result);
            var okObjectResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var apiResponse = Assert.IsType<ApiResponse<CreateReturnRequestResponseDto>>(okObjectResult.Value);
            Assert.True(apiResponse.Success);
            Assert.Equal("Successfully created a returning request", apiResponse.Message);
            Assert.Equal(responseDto.AssetCode, apiResponse.Data!.AssetCode);
            Assert.Equal(responseDto.AssignmentStatus, apiResponse.Data.AssignmentStatus);
        }

        [Fact]
        public async Task Create_ThrowsUnauthorizedAccessException_WhenUserIdIsMissing()
        {
            var claims = new List<Claim>
            {
                new(ClaimTypes.Role, "Admin")
            };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var claimsPrincipal = new ClaimsPrincipal(identity);
            ApplyUserToController(claimsPrincipal);

            await Assert.ThrowsAsync<UnauthorizedAccessException>(() => 
                _controller.Create(new CreateReturnRequestDto { AssignmentId = "assignment-guid" }));
        }

        [Fact]
        public async Task Create_ThrowsUnauthorizedAccessException_WhenRoleIsMissing()
        {
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, _adminId.ToString())
            };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var claimsPrincipal = new ClaimsPrincipal(identity);
            ApplyUserToController(claimsPrincipal);

            await Assert.ThrowsAsync<UnauthorizedAccessException>(() 
                => _controller.Create(new CreateReturnRequestDto { AssignmentId = "assignment-guid" }));
        }

        #endregion
    }
}