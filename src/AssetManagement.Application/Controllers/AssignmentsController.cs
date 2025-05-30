using System.Security.Claims;
using AssetManagement.Application.Services.Interfaces;
using AssetManagement.Contracts.Common;
using AssetManagement.Contracts.Common.Pagination;
using AssetManagement.Contracts.DTOs;
using AssetManagement.Contracts.Parameters;
using AssetManagement.Application.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
                throw new UnauthorizedAccessException("Cannot retrieve user id from claims");

            var pagedDate = await _assignmentService.GetAssignmentsAsync(userId, queryParams);

            return this.ToApiResponse(pagedDate, message: "Successfully fetched a paginated list of assignments");
        }
    }
}