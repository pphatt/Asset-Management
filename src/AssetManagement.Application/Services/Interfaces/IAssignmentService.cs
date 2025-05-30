using AssetManagement.Contracts.Common.Pagination;
using AssetManagement.Contracts.DTOs;
using AssetManagement.Contracts.DTOs.Requests;
using AssetManagement.Contracts.Parameters;

namespace AssetManagement.Application.Services.Interfaces
{
    public interface IAssignmentService
    {
        Task<PagedResult<AssignmentDto>> GetAssignmentsAsync(string adminId, AssignmentQueryParameters queryParams);
        Task<AssignmentDto?> GetAssignmentByIdAsync(Guid id);
        Task<AssignmentDto> CreateAssignmentAsync(Guid adminId, CreateAssignmentRequestDto dto);
    }
}