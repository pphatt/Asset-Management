using AssetManagement.Contracts.Common.Pagination;
using AssetManagement.Contracts.DTOs;
using AssetManagement.Contracts.DTOs.Resquest;
using AssetManagement.Contracts.Parameters;

namespace AssetManagement.Application.Services.Interfaces
{
    public interface IUserService
    {
        // <summary>
        // Retrieves a paginated list of users based on the provided query parameters
        // </summary>
        Task<PagedResult<UserDto>> GetUsersAsync(string userId, UserQueryParameters queryParams);

        /// <summary>
        /// Updates the user information based on the provided user ID and request data
        /// </summary>
        /// <param name="userId">Id of user perform this action</param>
        /// <param name="request">Data to update user</param>
        /// <returns></returns>
        Task<Guid> UpdateUserAsync(Guid userId, UpdateUserRequest request);

        /// <summary>
        /// Deletes a user based on the provided user ID
        /// </summary>
        /// <param name="deletedBy">Id of user perform this action</param>
        /// <param name="userId">Data to disable user</param>
        /// <returns></returns>
        Task<string> DeleteUser(Guid deletedBy, string staffCode);
    }
}