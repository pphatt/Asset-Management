using AssetManagement.Contracts.Common.Pagination;
using AssetManagement.Contracts.DTOs;
using AssetManagement.Contracts.Parameters;

namespace AssetManagement.Application.Services.Interfaces
{
    public interface IAssignmentService
    {
        Task<PagedResult<AssignmentDto>> GetAssignmentsAsync(string adminId, AssignmentQueryParameters queryParams);
    }
}