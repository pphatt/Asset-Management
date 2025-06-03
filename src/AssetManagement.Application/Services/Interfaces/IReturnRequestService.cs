using AssetManagement.Contracts.Common.Pagination;
using AssetManagement.Contracts.DTOs;
using AssetManagement.Contracts.Parameters;

namespace AssetManagement.Application.Services.Interfaces
{
    public interface IReturnRequestService
    {
        Task<PagedResult<ReturnRequestDto>> GetReturnRequestsAsync(Guid adminId, ReturnRequestQueryParameters queryParams);
    }
}