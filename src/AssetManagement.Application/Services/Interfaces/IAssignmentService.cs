using AssetManagement.Contracts.Common.Pagination;
using AssetManagement.Contracts.DTOs;
using AssetManagement.Contracts.DTOs.Requests;
using AssetManagement.Contracts.Parameters;

namespace AssetManagement.Application.Services.Interfaces
{
    public interface IAssignmentService
    {
        Task<PagedResult<AssignmentDto>> GetAssignmentsAsync(Guid adminId, AssignmentQueryParameters queryParams);
        Task<AssignmentDto?> GetAssignmentByIdAsync(Guid id);
        Task<AssignmentDetailsDto?> GetAssignmentDetailsByIdAsync(Guid adminId, Guid id);
        Task<AssignmentDto> CreateAssignmentAsync(Guid adminId, CreateAssignmentRequestDto dto);
        Task<AssignmentDto> UpdateAssignmentAsync(Guid id, Guid adminId, UpdateAssignmentRequestDto dto);
        Task<bool> DeleteAssignmentAsync(Guid id, Guid adminId);
        Task<AssignmentDto?> AcceptAssignmentAsync(Guid id, Guid userId);
        Task<AssignmentDto?> DeclineAssignmentAsync(Guid id, Guid userId);
        Task<PagedResult<MyAssignmentDto>> GetMyAssignmentsAsync(Guid userId, MyAssignmentQueryParameters queryParams);
    }
}