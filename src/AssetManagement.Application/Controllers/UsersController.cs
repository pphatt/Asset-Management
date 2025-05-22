using AssetManagement.Application.Services.Interfaces;
using AssetManagement.Contracts.Common.Pagination;
using AssetManagement.Contracts.DTOs;
using AssetManagement.Contracts.DTOs.Resquest;
using AssetManagement.Contracts.Parameters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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

        [HttpPatch("{userId}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponse<Guid>>> UpdateUser(Guid userId, [FromBody] UpdateUserRequest request)
        {
            var updatedUser = await _userService.UpdateUserAsync(userId, request);

            return new ApiResponse<Guid>
            {
                Success = true,
                Message = "Successfully updated user.",
                Data = updatedUser,
            };
        }

        [HttpDelete("{staffCode}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponse<string>>> DeleteUser(string staffCode)
        {
            var deletedBy = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? null;

            var deleteUser = await _userService.DeleteUser(Guid.Parse(deletedBy!), staffCode);

            return new ApiResponse<string>
            {
                Success = true,
                Message = "Successfully deleted user.",
                Data = deleteUser,
            };
        }

    }
}