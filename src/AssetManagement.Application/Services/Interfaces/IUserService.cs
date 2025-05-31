using AssetManagement.Contracts.Common.Pagination;
using AssetManagement.Contracts.DTOs;
using AssetManagement.Contracts.DTOs.Requests;
using AssetManagement.Contracts.DTOs.Responses;
using AssetManagement.Contracts.Parameters;

namespace AssetManagement.Application.Services.Interfaces
{
    public interface IUserService
    {
        Task<PagedResult<UserDto>> GetUsersAsync(string userId, UserQueryParameters queryParams);
        Task<UserDetailsDto?> GetUserByIdAsync(Guid id);
        Task<UserDetailsDto> GetUserByStaffCodeAsync(string staffCode);

        Task<CreateUserResponseDto> CreateUserAsync(string adminUserId, CreateUserRequestDto requestDto);
        Task<string> UpdateUserAsync(string adminId, string staffCode, UpdateUserRequestDto request);
        Task<string> DeleteUserAsync(Guid deletedBy, string staffCode);
    }
}