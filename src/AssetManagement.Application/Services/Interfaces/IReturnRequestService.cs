using AssetManagement.Contracts.Common.Pagination;
using AssetManagement.Contracts.DTOs;
using AssetManagement.Contracts.DTOs.Requests;
using AssetManagement.Contracts.DTOs.Responses;
using AssetManagement.Contracts.Parameters;

namespace AssetManagement.Application.Services.Interfaces
{
    public interface IReturnRequestService
    {
        Task<PagedResult<ReturnRequestDto>> GetReturnRequestsAsync(Guid adminId, ReturnRequestQueryParameters queryParams);

        Task<CreateReturnRequestResponseDto> CreateReturnRequestAsync(string assignmentId, 
            string requesterId,
            string role);
    }
}