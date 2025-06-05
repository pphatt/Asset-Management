using System.Security.Claims;
using AssetManagement.Application.Services.Interfaces;
using AssetManagement.Application.Extensions;
using AssetManagement.Contracts.Common.Pagination;
using AssetManagement.Contracts.DTOs;
using AssetManagement.Contracts.Parameters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AssetManagement.Contracts.Common;
using AssetManagement.Contracts.DTOs.Requests;
using AssetManagement.Contracts.DTOs.Responses;

namespace AssetManagement.Application.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    public class ReturnRequestsController : ControllerBase
    {
        private readonly IReturnRequestService _returnRequestService;

        public ReturnRequestsController(IReturnRequestService returnRequestService)
        {
            _returnRequestService = returnRequestService;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponse<PagedResult<ReturnRequestDto>>>> Get([FromQuery] ReturnRequestQueryParameters queryParams)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId is null)
            {
                throw new UnauthorizedAccessException("Cannot retrieve user id from claims");
            }

            var pagedData = await _returnRequestService.GetReturnRequestsAsync(Guid.Parse(userId), queryParams);
            return this.ToApiResponse(pagedData, "Successfully fetched a paginated list of return requests");
        }

        [HttpPost("")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<CreateReturnRequestResponseDto>>> Create([FromBody] CreateReturnRequestDto request)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value 
                ?? throw new UnauthorizedAccessException("Cannot retrieve user id from claims");

            var role = User.FindFirst(ClaimTypes.Role)?.Value 
                ?? throw new UnauthorizedAccessException("Cannot retrieve user role from claims");

            var response = await _returnRequestService.CreateReturnRequestAsync(request.AssignmentId, userId, role);
            
            return this.ToApiResponse(response, "Successfully created a returning request");
        }
        
        [HttpDelete("{returnRequestId}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponse<ReturnRequestDto>>> CancelReturnRequest(Guid returnRequestId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId is null)
            {
                throw new UnauthorizedAccessException("Cannot retrieve user id from claims");
            }

            var returnRequest = await _returnRequestService.CancelReturnRequestAsync(returnRequestId, Guid.Parse(userId));

            return this.ToApiResponse(returnRequest, "Successfully cancelled the return request");
        }
    }
}