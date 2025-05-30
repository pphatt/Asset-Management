using AssetManagement.Application.Extensions;
using AssetManagement.Application.Services.Interfaces;
using AssetManagement.Application.Validators;
using AssetManagement.Contracts.Common.Pagination;
using AssetManagement.Contracts.DTOs;
using AssetManagement.Contracts.DTOs.Requests;
using AssetManagement.Contracts.Parameters;
using AssetManagement.Domain.Entities;
using AssetManagement.Domain.Enums;
using AssetManagement.Domain.Extensions;
using AssetManagement.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace AssetManagement.Application.Services
{
    public class AssignmentService : IAssignmentService
    {
        private readonly IAssignmentRepository _assignmentRepository;
        private readonly IAssetRepository _assetRepository;
        private readonly IUserRepository _userRepository;

        public AssignmentService(IAssignmentRepository assignmentRepository, IAssetRepository assetRepository, IUserRepository userRepository)
        {
            _assignmentRepository = assignmentRepository;
            _assetRepository = assetRepository;
            _userRepository = userRepository;
        }

        private async Task<Location> GetLocationByUserId(string userId)
        {
            var user = await _userRepository.GetByIdAsync(Guid.Parse(userId));
            if (user is null)
            {
                throw new KeyNotFoundException($"User with id {userId} not found");
            }

            return user.Location;
        }

        public async Task<PagedResult<AssignmentDto>> GetAssignmentsAsync(string adminId, AssignmentQueryParameters queryParams)
        {
            var currentAdminLocation = await GetLocationByUserId(adminId);

            var stateFilter = AssignmentValidator.ParseState(queryParams.State);
            var dateFilter = AssignmentValidator.ParseDate(queryParams.Date);

            // Handling searching, filtering, sorting here
            IQueryable<Assignment> query = _assignmentRepository.GetAll()
                .Include(a => a.Assignee)
                .Include(a => a.Assignor)
                .Include(a => a.Asset)
                .ApplySearch(queryParams.SearchTerm)
                .ApplyFilters(state: stateFilter, date: dateFilter, location: currentAdminLocation)
                .ApplySorting(queryParams.GetSortCriteria());

            // Pagination below here
            int total = await query.CountAsync();

            var assignments = await query
                .Skip((queryParams.PageNumber - 1) * queryParams.PageSize)
                .Take(queryParams.PageSize)
                .ToListAsync();

            var items = assignments
                .Select((a, idx) => new AssignmentDto
                {
                    Id = a.Id,
                    No = idx + (queryParams.PageSize * (queryParams.PageNumber - 1)) + 1,
                    AssetCode = a.Asset.Code,
                    AssetName = a.Asset.Name,
                    AssignedBy = a.Assignor.Username,
                    AssignedTo = a.Assignee.Username,
                    AssignedDate = a.AssignedDate.ToString("dd/MM/yyyy"),
                    State = a.State.GetDisplayName(),
                }).ToList();

            return new PagedResult<AssignmentDto>(items, total, queryParams.PageSize, queryParams.PageNumber);
        }

        public async Task<AssignmentDto?> GetAssignmentByIdAsync(Guid id)
        {
            var assignment = await _assignmentRepository.GetByIdAsync(id);
            if (assignment is null)
            {
                return null;
            }

            return new AssignmentDto
            {
                Id = assignment.Id,
                AssetCode = assignment.Asset.Code,
                AssetName = assignment.Asset.Name,
                AssignedBy = assignment.Assignor.Username,
                AssignedTo = assignment.Assignee.Username,
                AssignedDate = assignment.AssignedDate.ToString("dd/MM/yyyy"),
                State = assignment.State.GetDisplayName(),
            };
        }

        public async Task<AssignmentDto> CreateAssignmentAsync(Guid adminId, CreateAssignmentRequestDto dto)
        {
            await AssignmentValidator.ValidateCreateAssignmentAsync(dto, adminId, _assetRepository, _userRepository, _assignmentRepository);

            var assignment = new Assignment
            {
                AssignorId = adminId,
                AssigneeId = Guid.Parse(dto.AssigneeId),
                AssetId = Guid.Parse(dto.AssetId),
                Note = dto.Note,
                State = AssignmentState.WaitingForAcceptance,
                AssignedDate = DateTimeOffset.Parse(dto.AssignedDate),
                CreatedBy = adminId,
                CreatedDate = DateTime.UtcNow,
                LastModifiedBy = adminId,
                LastModifiedDate = DateTime.UtcNow,
            };

            await _assignmentRepository.AddAsync(assignment);
            await _assignmentRepository.SaveChangesAsync();

            var created = await GetAssignmentByIdAsync(assignment.Id);
            return created!;
        }
    }
}