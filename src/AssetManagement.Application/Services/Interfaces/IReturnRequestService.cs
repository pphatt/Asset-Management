using AssetManagement.Contracts.Common.Pagination;
using AssetManagement.Contracts.DTOs;
using AssetManagement.Contracts.DTOs.Responses;
using AssetManagement.Contracts.Parameters;

namespace AssetManagement.Application.Services.Interfaces
{
    public interface IReturnRequestService
    {
        Task<CreateReturnRequestResponseDto> CreateReturnRequestAsync(string assignmentId, string requesterId, string role);
        Task<PagedResult<ReturnRequestDto>> GetReturnRequestsAsync(Guid adminId, ReturnRequestQueryParameters queryParams);
        Task<bool> AcceptReturnRequestAsync(Guid returnRequestId, Guid userId);
        Task<string> CancelReturnRequestAsync(Guid returnRequestId, Guid adminId);
    }
}