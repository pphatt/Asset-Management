using AssetManagement.Contracts.Common.Pagination;
using AssetManagement.Contracts.DTOs;
using AssetManagement.Contracts.Parameters;

namespace AssetManagement.Application.Services.Interfaces
{
    public interface IUserService
    {
        // <summary>
        // Retrieves a paginated list of users based on the provided query parameters
        // </summary>
        Task<PagedResult<UserDto>> GetUsersAsync(UserQueryParameters p);
    }
}