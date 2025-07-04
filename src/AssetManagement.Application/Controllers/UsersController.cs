using System.Security.Claims;
using AssetManagement.Application.Extensions;
using AssetManagement.Application.Services.Interfaces;
using AssetManagement.Contracts.Common;
using AssetManagement.Contracts.Common.Pagination;
using AssetManagement.Contracts.DTOs;
using AssetManagement.Contracts.DTOs.Requests;
using AssetManagement.Contracts.DTOs.Responses;
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

        [HttpGet("{id:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponse<UserDetailsDto>>> GetById(Guid id)
        {
            try
            {
                var result = await _userService.GetUserByIdAsync(id);

                if (result == null)
                    return this.ToNotFoundApiResponse<UserDetailsDto>($"User with Id {id} not found");

                return this.ToApiResponse(result, $"User with Id {id} found successfully");
            }
            catch (Exception ex)
            {
                return this.ToErrorApiResponse<UserDetailsDto>(
                    $"An error occurred while getting user by Id: {ex.Message}");
            }
        }

        [HttpGet("{staffCode}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponse<UserDetailsDto>>> GetByStaffCode(string staffCode)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(staffCode))
                {
                    return this.ToBadRequestApiResponse<UserDetailsDto>("Staff code cannot be empty");
                }

                var result = await _userService.GetUserByStaffCodeAsync(staffCode);
                return this.ToApiResponse(result, $"User with staff code {staffCode} found successfully");
            }
            catch (KeyNotFoundException ex)
            {
                return this.ToNotFoundApiResponse<UserDetailsDto>(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return this.ToBadRequestApiResponse<UserDetailsDto>(ex.Message);
            }
            catch (Exception ex)
            {
                return this.ToErrorApiResponse<UserDetailsDto>(
                    $"An error occurred while getting user: {ex.Message}");
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponse<CreateUserResponseDto>>> Create([FromBody] CreateUserRequestDto dto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId is null)
                throw new UnauthorizedAccessException("Cannot retrieve user id from claims");

            var createdUser = await _userService.CreateUserAsync(userId, dto);

            return this.ToCreatedApiResponse(createdUser, "Successfully created a new user");
        }

        [HttpPatch("{staffCode}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponse<string>>> UpdateUser(string staffCode, [FromBody] UpdateUserRequestDto request)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId is null)
                throw new UnauthorizedAccessException("Cannot retrieve user id from claims");

            var updatedUserCode = await _userService.UpdateUserAsync(userId, staffCode, request);

            return new ApiResponse<string>
            {
                Success = true,
                Message = "Successfully updated user.",
                Data = updatedUserCode,
            };
        }

        [HttpDelete("{staffCode}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponse<string>>> DeleteUser(string staffCode)
        {
            var deletedBy = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? null;

            var deleteUser = await _userService.DeleteUserAsync(Guid.Parse(deletedBy!), staffCode);

            return new ApiResponse<string>
            {
                Success = true,
                Message = "Successfully deleted user.",
                Data = deleteUser,
            };
        }
    }
}