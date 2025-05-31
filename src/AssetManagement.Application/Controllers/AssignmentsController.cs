using System.Security.Claims;
using AssetManagement.Application.Services.Interfaces;
using AssetManagement.Contracts.Common;
using AssetManagement.Contracts.Common.Pagination;
using AssetManagement.Contracts.DTOs;
using AssetManagement.Contracts.Parameters;
using AssetManagement.Application.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AssetManagement.Contracts.DTOs.Requests;

namespace AssetManagement.Application.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    public class AssignmentsController : ControllerBase
    {
        private readonly IAssignmentService _assignmentService;

        public AssignmentsController(IAssignmentService assignmentService)
        {
            _assignmentService = assignmentService;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponse<PagedResult<AssignmentDto>>>> Get([FromQuery] AssignmentQueryParameters queryParams)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId is null)
            {
                throw new UnauthorizedAccessException("Cannot retrieve user id from claims");
            }

            var pagedDate = await _assignmentService.GetAssignmentsAsync(Guid.Parse(userId), queryParams);

            return this.ToApiResponse(pagedDate, message: "Successfully fetched a paginated list of assignments");
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<AssignmentDetailsDto>>> GetById(Guid id)
        {

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId is null)
            {
                throw new UnauthorizedAccessException("Cannot retrieve user id from claims");
            }

            var assignment = await _assignmentService.GetAssignmentDetailsByIdAsync(Guid.Parse(userId), id);
            if (assignment is null)
            {
                throw new KeyNotFoundException($"Assignment with id {id} not found");
            }

            return this.ToApiResponse(assignment, message: $"Successfully fetched an assignment with id {id}");
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponse<AssignmentDto>>> Create(CreateAssignmentRequestDto dto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId is null)
            {
                throw new UnauthorizedAccessException("Cannot retrieve user id from claims");
            }

            var createdAssignment = await _assignmentService.CreateAssignmentAsync(Guid.Parse(userId), dto);
            return this.ToCreatedApiResponse(createdAssignment, "New assignment created successfully!");
        }

        [HttpPatch("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponse<AssignmentDto>>> Update(Guid id, [FromBody] UpdateAssignmentRequestDto dto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId is null)
            {
                throw new UnauthorizedAccessException("Cannot retrieve user id from claims");
            }

            var updatedAssignment = await _assignmentService.UpdateAssignmentAsync(id, Guid.Parse(userId), dto);
            return this.ToApiResponse(updatedAssignment, "Assignment updated successfully");
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponse<bool>>> Delete(Guid id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId is null)
            {
                throw new UnauthorizedAccessException("Cannot retrieve user id from claims");
            }

            var result = await _assignmentService.DeleteAssignmentAsync(id, Guid.Parse(userId));
            if (!result)
            {
                return this.ToNotFoundApiResponse<bool>("Assignment deleted failed due to unexpected circumstance.");
            }
            return this.ToApiResponse(true, "Assignment deleted successfully");
        }

        [HttpPost("{id}/accept")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<AssignmentDto>>> AcceptAssignment(Guid id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return this.ToBadRequestApiResponse<AssignmentDto>("Cannot retrieve user ID from claims");
            }

            var assignment = await _assignmentService.AcceptAssignmentAsync(id, Guid.Parse(userId));
            return this.ToApiResponse(assignment!, "Assignment accepted successfully");
        }

        [HttpPost("{id}/decline")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<AssignmentDto>>> DeclineAssignment(Guid id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return this.ToBadRequestApiResponse<AssignmentDto>("Cannot retrieve user ID from claims");
            }

            var assignment = await _assignmentService.DeclineAssignmentAsync(id, Guid.Parse(userId));
            return this.ToApiResponse(assignment!, "Assignment declined successfully");
        }
    }
}