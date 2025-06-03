using System.Security.Claims;
using AssetManagement.Application.Controllers;
using AssetManagement.Application.Services.Interfaces;
using AssetManagement.Contracts.Common;
using AssetManagement.Contracts.Common.Pagination;
using AssetManagement.Contracts.DTOs;
using AssetManagement.Contracts.DTOs.Requests;
using AssetManagement.Contracts.Exceptions;
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

        #region Helpers
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
        #endregion

        #region GetAssignments
        [Fact]
        public async Task Get_WithValidUser_ReturnsOkResult()
        {
            // Arrange
            var userId = Guid.NewGuid();
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
            var claimsPrincipal = CreateUserPrincipal(userId.ToString());
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
            var userId = Guid.NewGuid();
            var queryParams = new AssignmentQueryParameters();

            _mockAssignmentService.Setup(x => x.GetAssignmentsAsync(userId, queryParams))
                .ThrowsAsync(new KeyNotFoundException("User not found"));

            // Setup user claims
            var claimsPrincipal = CreateUserPrincipal(userId.ToString());
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
            var userId = Guid.NewGuid();
            var queryParams = new AssignmentQueryParameters
            {
                SearchTerm = "laptop",
                States = new List<string>(["Accepted"]),
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
            var claimsPrincipal = CreateUserPrincipal(userId.ToString());
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
        #endregion

        #region CreateAssignment
        [Fact]
        public async Task Create_WithValidData_ReturnsCreatedResult()
        {
            // Arrange
            var adminId = Guid.NewGuid();
            var dto = new CreateAssignmentRequestDto
            {
                AssetId = Guid.NewGuid().ToString(),
                AssigneeId = Guid.NewGuid().ToString(),
                AssignedDate = DateTimeOffset.Now.AddDays(1).ToString("dd/MM/yyyy"),
                Note = "Test assignment"
            };

            var createdAssignment = new AssignmentDto
            {
                Id = Guid.NewGuid(),
                AssetCode = "LP001",
                AssetName = "Laptop Dell",
                AssignedBy = "admin1",
                AssignedTo = "user1",
                AssignedDate = DateTimeOffset.Now.ToString("dd/MM/yyyy"),
                State = "WaitingForAcceptance"
            };

            _mockAssignmentService.Setup(x => x.CreateAssignmentAsync(adminId, dto))
                .ReturnsAsync(createdAssignment);

            var claimsPrincipal = CreateUserPrincipal(adminId.ToString());
            ApplyUserToController(claimsPrincipal);

            // Act
            var result = await _controller.Create(dto);

            // Assert
            var createdResult = Assert.IsType<ActionResult<ApiResponse<AssignmentDto>>>(result);
            var objectResult = Assert.IsType<ObjectResult>(createdResult.Result);
            Assert.Equal(StatusCodes.Status201Created, objectResult.StatusCode);
            var apiResponse = Assert.IsType<ApiResponse<AssignmentDto>>(objectResult.Value);
            Assert.True(apiResponse.Success);
            Assert.Equal("New assignment created successfully!", apiResponse.Message);
            Assert.Equal(createdAssignment, apiResponse.Data);
        }

        [Fact]
        public async Task Create_WithInvalidData_ThrowsAggregateFieldValidationException()
        {
            // Arrange
            var adminId = Guid.NewGuid();
            var dto = new CreateAssignmentRequestDto
            {
                AssetId = "invalid-guid",
                AssigneeId = Guid.NewGuid().ToString(),
                AssignedDate = DateTimeOffset.Now.AddDays(1).ToString("dd/MM/yyyy"),
                Note = "Test assignment"
            };

            _mockAssignmentService.Setup(x => x.CreateAssignmentAsync(adminId, dto))
                .ThrowsAsync(new AggregateFieldValidationException(new List<FieldValidationException>
                {
                    new FieldValidationException("AssetId", "Invalid Asset ID format")
                }));

            var claimsPrincipal = CreateUserPrincipal(adminId.ToString());
            ApplyUserToController(claimsPrincipal);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<AggregateFieldValidationException>(
                () => _controller.Create(dto));

            Assert.Single(exception.Errors);
            Assert.Equal("AssetId", exception.Errors[0].Field);
            Assert.Equal("Invalid Asset ID format", exception.Errors[0].Message);
        }

        [Fact]
        public async Task Create_WithoutUserClaim_ThrowsUnauthorizedAccessException()
        {
            // Arrange
            var dto = new CreateAssignmentRequestDto
            {
                AssetId = Guid.NewGuid().ToString(),
                AssigneeId = Guid.NewGuid().ToString(),
                AssignedDate = DateTimeOffset.Now.AddDays(1).ToString("dd/MM/yyyy"),
                Note = "Test assignment"
            };

            ApplyUserToController(new ClaimsPrincipal());

            // Act & Assert
            var exception = await Assert.ThrowsAsync<UnauthorizedAccessException>(
                () => _controller.Create(dto));

            Assert.Equal("Cannot retrieve user id from claims", exception.Message);
        }
        #endregion

        #region UpdateAssignment
        [Fact]
        public async Task Update_ValidRequest_ReturnsOkResult()
        {
            // Arrange
            var adminId = Guid.NewGuid();
            var assignmentId = Guid.NewGuid();
            var dto = new UpdateAssignmentRequestDto();
            var updatedDto = new AssignmentDto { Id = assignmentId };

            _mockAssignmentService.Setup(x => x.UpdateAssignmentAsync(assignmentId, adminId, dto))
                .ReturnsAsync(updatedDto);
            ApplyUserToController(CreateUserPrincipal(adminId.ToString()));

            // Act
            var result = await _controller.Update(assignmentId, dto);

            // Assert
            var okResult = Assert.IsType<ActionResult<ApiResponse<AssignmentDto>>>(result);
            var objectResult = Assert.IsType<OkObjectResult>(okResult.Result);
            var apiResponse = Assert.IsType<ApiResponse<AssignmentDto>>(objectResult.Value);
            Assert.True(apiResponse.Success);
            Assert.Equal("Assignment updated successfully", apiResponse.Message);
            Assert.Equal(updatedDto, apiResponse.Data);
        }

        [Fact]
        public async Task Update_NoUserId_ThrowsUnauthorizedAccessException()
        {
            // Arrange
            var assignmentId = Guid.NewGuid();
            var dto = new UpdateAssignmentRequestDto();
            ApplyUserToController(new ClaimsPrincipal());

            // Act & Assert
            var exception = await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
                _controller.Update(assignmentId, dto));
            Assert.Equal("Cannot retrieve user id from claims", exception.Message);
        }

        [Fact]
        public async Task Update_ServiceThrowsException_PropagatesException()
        {
            // Arrange
            var adminId = Guid.NewGuid();
            var assignmentId = Guid.NewGuid();
            var dto = new UpdateAssignmentRequestDto();

            _mockAssignmentService.Setup(x => x.UpdateAssignmentAsync(assignmentId, adminId, dto))
                .ThrowsAsync(new AggregateFieldValidationException(new List<FieldValidationException>
                {
                    new FieldValidationException("AssetId", "Asset not found")
                }));
            ApplyUserToController(CreateUserPrincipal(adminId.ToString()));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<AggregateFieldValidationException>(() =>
                _controller.Update(assignmentId, dto));
            Assert.Contains("Asset not found", exception.Errors.Select(e => e.Message));
        }
        #endregion

        #region DeleteAssignment
        [Fact]
        public async Task Delete_AssignmentExists_ShouldReturnSuccess()
        {
            // Arrange
            var assignmentId = Guid.NewGuid();
            var adminId = Guid.NewGuid();
            _mockAssignmentService.Setup(s => s.DeleteAssignmentAsync(assignmentId, adminId)).ReturnsAsync(true);

            var claimsPrincipal = CreateUserPrincipal(adminId.ToString());
            ApplyUserToController(claimsPrincipal);

            // Act
            var result = await _controller.Delete(assignmentId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var apiResponse = Assert.IsType<ApiResponse<bool>>(okResult.Value);
            Assert.True(apiResponse.Success);
            Assert.Equal("Assignment deleted successfully", apiResponse.Message);
        }

        [Fact]
        public async Task Delete_AssignmentNotFound_ShouldReturnNotFound()
        {
            // Arrange
            var assignmentId = Guid.NewGuid();
            var adminId = Guid.NewGuid();
            _mockAssignmentService.Setup(s => s.DeleteAssignmentAsync(assignmentId, adminId)).ReturnsAsync(false);

            var claimsPrincipal = CreateUserPrincipal(adminId.ToString());
            ApplyUserToController(claimsPrincipal);

            // Act
            var result = await _controller.Delete(assignmentId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
            var apiResponse = Assert.IsType<ApiResponse<bool>>(notFoundResult.Value);
            Assert.False(apiResponse.Success);
            Assert.Equal("Assignment deleted failed due to unexpected circumstance.", apiResponse.Message);
        }
        #endregion

        #region AcceptAssignment
        [Fact]
        public async Task AcceptAssignment_ValidAssignment_ShouldReturnSuccess()
        {
            // Arrange
            var assignmentId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var acceptedDto = new AssignmentDto { Id = assignmentId, State = "Accepted" };
            _mockAssignmentService.Setup(s => s.AcceptAssignmentAsync(assignmentId, userId)).ReturnsAsync(acceptedDto);

            var claimsPrincipal = CreateUserPrincipal(userId.ToString());
            ApplyUserToController(claimsPrincipal);

            // Act
            var result = await _controller.AcceptAssignment(assignmentId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var apiResponse = Assert.IsType<ApiResponse<AssignmentDto>>(okResult.Value);
            Assert.True(apiResponse.Success);
            Assert.Equal("Assignment accepted successfully", apiResponse.Message);
        }

        [Fact]
        public async Task AcceptAssignment_UnauthorizedUser_ThrowsUnauthorizedAccessException()
        {
            // Arrange
            var assignmentId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            _mockAssignmentService
              .Setup(s => s.AcceptAssignmentAsync(assignmentId, userId))
              .ThrowsAsync(new UnauthorizedAccessException("Not the assignee"));
            ApplyUserToController(CreateUserPrincipal(userId.ToString()));

            // Act & Assert
            await Assert.ThrowsAsync<UnauthorizedAccessException>(
              () => _controller.AcceptAssignment(assignmentId)
            );
        }

        [Fact]
        public async Task AcceptAssignment_NoUserClaim_ShouldReturnBadRequest()
        {
            // Arrange
            var assignmentId = Guid.NewGuid();
            // Do *not* set up any NameIdentifier claim:
            ApplyUserToController(new ClaimsPrincipal(new ClaimsIdentity()));

            // Act
            var result = await _controller.AcceptAssignment(assignmentId);

            // Assert
            var badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);
            var response = Assert.IsType<ApiResponse<AssignmentDto>>(badRequest.Value);
            Assert.False(response.Success);
            Assert.Equal("Cannot retrieve user ID from claims", response.Message);
        }
        #endregion

        #region DeclineAssignment
        [Fact]
        public async Task DeclineAssignment_ValidAssignment_ShouldReturnSuccess()
        {
            // Arrange
            var assignmentId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var declinedDto = new AssignmentDto { Id = assignmentId, State = "Declined" };
            _mockAssignmentService.Setup(s => s.DeclineAssignmentAsync(assignmentId, userId)).ReturnsAsync(declinedDto);

            var claimsPrincipal = CreateUserPrincipal(userId.ToString());
            ApplyUserToController(claimsPrincipal);

            // Act
            var result = await _controller.DeclineAssignment(assignmentId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var apiResponse = Assert.IsType<ApiResponse<AssignmentDto>>(okResult.Value);
            Assert.True(apiResponse.Success);
            Assert.Equal("Assignment declined successfully", apiResponse.Message);
        }

        [Fact]
        public async Task DeclineAssignment_AssignmentNotFound_ShouldReturnNotFound()
        {
            // Arrange
            var assignmentId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            _mockAssignmentService.Setup(s => s.DeclineAssignmentAsync(assignmentId, userId))
                .ThrowsAsync(new KeyNotFoundException("Assignment not found"));
            ApplyUserToController(CreateUserPrincipal(userId.ToString()));

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(
              () => _controller.DeclineAssignment(assignmentId)
            );
        }
        #endregion
    }
}