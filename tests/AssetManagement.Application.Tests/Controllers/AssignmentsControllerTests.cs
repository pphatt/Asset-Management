using System.Security.Claims;
using AssetManagement.Application.Controllers;
using AssetManagement.Application.Services.Interfaces;
using AssetManagement.Contracts.Common;
using AssetManagement.Contracts.Common.Pagination;
using AssetManagement.Contracts.DTOs;
using AssetManagement.Contracts.Parameters;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace AssetManagement.Application.Tests.Controllers
{
    public class AssignmentsControllerTests
    {
        private readonly Mock<IAssignmentService> _mockAssignmentService;
        private readonly AssignmentsController _controller;

        public AssignmentsControllerTests()
        {
            _mockAssignmentService = new Mock<IAssignmentService>();
            _controller = new AssignmentsController(_mockAssignmentService.Object);
        }

        private ClaimsPrincipal CreateUserPrincipal(string userId, string role = "Admin")
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId),
                new Claim(ClaimTypes.Role, role)
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
        public async Task Get_WithValidUser_ReturnsOkResult()
        {
            // Arrange
            var userId = "123e4567-e89b-12d3-a456-426614174000";
            var queryParams = new AssignmentQueryParameters();

            var expectedResult = new PagedResult<AssignmentDto>(
                new List<AssignmentDto>
                {
                    new AssignmentDto
                    {
                        AssetCode = "LP001",
                        AssetName = "Laptop Dell",
                        AssignedBy = "admin1",
                        AssignedTo = "user1",
                        AssignedDate = "01/01/2024",
                        State = "Accepted"
                    }
                },
                totalCount: 1,
                pageSize: 5,
                pageNumber: 1
            );

            _mockAssignmentService.Setup(x => x.GetAssignmentsAsync(userId, queryParams))
                .ReturnsAsync(expectedResult);

            // Setup user claims
            var claimsPrincipal = CreateUserPrincipal(userId);
            ApplyUserToController(claimsPrincipal);

            // Act
            var result = await _controller.Get(queryParams);

            // Assert
            Assert.NotNull(result);
            var okResult = Assert.IsType<ActionResult<ApiResponse<PagedResult<AssignmentDto>>>>(result);
            var okObjectResult = Assert.IsType<OkObjectResult>(okResult.Result);
            var apiResponse = Assert.IsType<ApiResponse<PagedResult<AssignmentDto>>>(okObjectResult.Value);

            Assert.True(apiResponse.Success);
            Assert.Equal("Successfully fetched a paginated list of assignments", apiResponse.Message);
            Assert.NotNull(apiResponse.Data);
            Assert.Single(apiResponse.Data.Items);
        }

        [Fact]
        public async Task Get_WithoutUserClaim_ThrowsUnauthorizedAccessException()
        {
            // Arrange
            var queryParams = new AssignmentQueryParameters();

            // Setup empty claims
            ApplyUserToController(new ClaimsPrincipal());

            // Act & Assert
            var exception = await Assert.ThrowsAsync<UnauthorizedAccessException>(
                () => _controller.Get(queryParams));

            Assert.Equal("Cannot retrieve user id from claims", exception.Message);
        }

        [Fact]
        public async Task Get_WithServiceException_ThrowsException()
        {
            // Arrange
            var userId = "123e4567-e89b-12d3-a456-426614174000";
            var queryParams = new AssignmentQueryParameters();

            _mockAssignmentService.Setup(x => x.GetAssignmentsAsync(userId, queryParams))
                .ThrowsAsync(new KeyNotFoundException("User not found"));

            // Setup user claims
            var claimsPrincipal = CreateUserPrincipal(userId);
            ApplyUserToController(claimsPrincipal);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<KeyNotFoundException>(
                () => _controller.Get(queryParams));

            Assert.Equal("User not found", exception.Message);
        }

        [Fact]
        public async Task Get_WithComplexQueryParams_CallsServiceCorrectly()
        {
            // Arrange
            var userId = "123e4567-e89b-12d3-a456-426614174000";
            var queryParams = new AssignmentQueryParameters
            {
                SearchTerm = "laptop",
                State = "Accepted",
                Date = "01/01/2024",
                SortBy = "assetcode:asc,assigneddate:desc",
                PageNumber = 2,
                PageSize = 10
            };

            var expectedResult = new PagedResult<AssignmentDto>(
                new List<AssignmentDto>(),
                totalCount: 0,
                pageSize: 10,
                pageNumber: 2
            );

            _mockAssignmentService.Setup(x => x.GetAssignmentsAsync(userId, queryParams))
                .ReturnsAsync(expectedResult);

            // Setup user claims
            var claimsPrincipal = CreateUserPrincipal(userId);
            ApplyUserToController(claimsPrincipal);

            // Act
            var result = await _controller.Get(queryParams);

            // Assert
            _mockAssignmentService.Verify(x => x.GetAssignmentsAsync(userId, queryParams), Times.Once);

            Assert.NotNull(result);
            var okResult = Assert.IsType<ActionResult<ApiResponse<PagedResult<AssignmentDto>>>>(result);
            var okObjectResult = Assert.IsType<OkObjectResult>(okResult.Result);
            var apiResponse = Assert.IsType<ApiResponse<PagedResult<AssignmentDto>>>(okObjectResult.Value);

            Assert.True(apiResponse.Success);
            Assert.NotNull(apiResponse.Data);
            Assert.Empty(apiResponse.Data.Items);
        }
    }
}