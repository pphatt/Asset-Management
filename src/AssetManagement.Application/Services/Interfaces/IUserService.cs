using AssetManagement.Contracts.Common.Pagination;
using AssetManagement.Contracts.DTOs;
using AssetManagement.Contracts.DTOs.Requests;
using AssetManagement.Contracts.DTOs.Responses;
using AssetManagement.Contracts.Parameters;

namespace AssetManagement.Application.Services.Interfaces
{
    public interface IUserService
    {
        // <summary>
        // Retrieves a paginated list of users based on the provided query parameters
        // </summary>
        Task<PagedResult<UserDto>> GetUsersAsync(string userId, UserQueryParameters queryParams);

        // <summary>
        // Retrieves a user by their staff code
        // </summary>
        Task<UserDetailsDto> GetByStaffCodeAsync(string staffCode);

        /// <summary>
        /// Updates the user information based on the provided user ID and request data
        /// </summary>
        /// <param name="adminId">Id of the admin performs this action</param>
        /// <param name="staffCode">Staff code of the user to be updated</param>
        /// <param name="request">Data to update user</param>
        /// <returns></returns>
        Task<string> UpdateUserAsync(string adminId, string staffCode, UpdateUserRequestDto request);

        /// <summary>
        /// Deletes a user based on the provided user ID
        /// </summary>
        /// <param name="deletedBy">Guid of the admin performs this action</param>
        /// <param name="staffCode">Staff code of the user to be deleted</param>
        /// <returns></returns>
        Task<string> DeleteUser(Guid deletedBy, string staffCode);

        // <summary>
        // Creates a new user with the provided details
        // </summary>
        Task<CreateUserResponseDto> CreateUserAsync(string adminUserId, CreateUserRequestDto requestDto);
    }
}