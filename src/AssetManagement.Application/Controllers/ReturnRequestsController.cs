using System.Security.Claims;
using AssetManagement.Application.Services.Interfaces;
using AssetManagement.Application.Extensions;
using AssetManagement.Contracts.Common.Pagination;
using AssetManagement.Contracts.DTOs;
using AssetManagement.Contracts.Parameters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AssetManagement.Contracts.Common;

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
    }
}