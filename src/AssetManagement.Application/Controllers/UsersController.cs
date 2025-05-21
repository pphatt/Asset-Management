using System.Security.Claims;
using AssetManagement.Application.Services.Interfaces;
using AssetManagement.Contracts.Common.Pagination;
using AssetManagement.Contracts.DTOs;
using AssetManagement.Contracts.Parameters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AssetManagement.Application.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponse<PagedResult<UserDto>>>> Get([FromQuery] UserQueryParameters queryParams)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? null;
            if (userId is null)
            {
                throw new InvalidOperationException("Cannot retrieve user id from claims");
            }
            var pagedData = await _userService.GetUsersAsync(userId, queryParams);

            return new ApiResponse<PagedResult<UserDto>>
            {
                Success = true,
                Message = "Successfully fetched a paginated list of users",
                Data = pagedData,
            };
        }
    }
}