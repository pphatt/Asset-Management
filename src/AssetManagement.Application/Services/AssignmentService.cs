using AssetManagement.Application.Extensions;
using AssetManagement.Application.Services.Interfaces;
using AssetManagement.Application.Validators;
using AssetManagement.Contracts.Common.Pagination;
using AssetManagement.Contracts.DTOs;
using AssetManagement.Contracts.DTOs.Requests;
using AssetManagement.Contracts.Parameters;
using AssetManagement.Contracts.Exceptions;
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

        public AssignmentService(IAssignmentRepository assignmentRepository, IAssetRepository assetRepository,
            IUserRepository userRepository)
        {
            _assignmentRepository = assignmentRepository;
            _assetRepository = assetRepository;
            _userRepository = userRepository;
        }

        private async Task<Location> GetLocationByUserId(Guid userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user is null)
            {
                throw new KeyNotFoundException($"User with id {userId} not found");
            }

            return user.Location;
        }

        public async Task<PagedResult<MyAssignmentDto>> GetMyAssignmentsAsync(Guid userId, MyAssignmentQueryParameters queryParams)
        {
            var currentAdminLocation = await GetLocationByUserId(userId);

            // Handling searching, filtering, sorting here
            IQueryable<Assignment> query = _assignmentRepository.GetAll()
                .ApplySorting(queryParams.GetSortCriteria())
                .Where(a => a.AssigneeId.Equals(userId) && (a.State != AssignmentState.Declined));

            // Pagination below here
            int total = await query.CountAsync();

            var assignments = await query
                .Skip((queryParams.PageNumber - 1) * queryParams.PageSize)
                .Take(queryParams.PageSize)
                .Select(a => new MyAssignmentDto
                {
                    AssignmentId = a.Id,
                    AssetCode = a.Asset.Code,
                    AssetName = a.Asset.Name,
                    AssignedDate = a.AssignedDate.ToString("dd/MM/yyyy"),
                    State = a.State.GetDisplayName(),
                    Category = a.Asset.Category.Name
                })
                .ToListAsync();

            return new PagedResult<MyAssignmentDto>(assignments, total, queryParams.PageSize, queryParams.PageNumber);
        }

        public async Task<PagedResult<AssignmentDto>> GetAssignmentsAsync(Guid adminId, AssignmentQueryParameters queryParams)
        {
            var currentAdminLocation = await GetLocationByUserId(adminId);

            var stateFilters = AssignmentValidator.ParseStates(queryParams.States);
            var dateFilter = AssignmentValidator.ParseDate(queryParams.Date);

            // Handling searching, filtering, sorting here
            IQueryable<Assignment> query = _assignmentRepository.GetAll()
                .Include(a => a.Assignee)
                .Include(a => a.Assignor)
                .Include(a => a.Asset)
                .ApplySearch(queryParams.SearchTerm)
                .ApplyFilters(states: stateFilters, date: dateFilter, location: currentAdminLocation)
                .ApplySorting(queryParams.GetSortCriteria())
                .Where(a => a.IsDeleted != true);

            // Pagination below here
            int total = await query.CountAsync();
            // Check if the list is currently sorted by no in descending order
            bool isNoDescending = queryParams.GetSortCriteria()
                .Any(s => s.property.Equals("no", StringComparison.OrdinalIgnoreCase) &&
                         s.order.Equals("desc", StringComparison.OrdinalIgnoreCase));

            var assignments = await query
                .Skip((queryParams.PageNumber - 1) * queryParams.PageSize)
                .Take(queryParams.PageSize)
                .ToListAsync();

            var items = assignments
                .Select((a, idx) => new AssignmentDto
                {
                    Id = a.Id,
                    No = isNoDescending ?
                        total - (idx + (queryParams.PageSize * (queryParams.PageNumber - 1))) :
                        idx + (queryParams.PageSize * (queryParams.PageNumber - 1)) + 1,
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
            var assignment = await _assignmentRepository
                .GetAll()
                .Include(a => a.Asset)
                .Include(a => a.Assignee)
                .Include(a => a.Assignor)
                .Where(a => a.Id == id)
                .FirstOrDefaultAsync();

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

        public async Task<AssignmentDetailsDto?> GetAssignmentDetailsByIdAsync(Guid adminId, Guid id)
        {
            var currentAdminLocation = await GetLocationByUserId(adminId);
            var assignment = await _assignmentRepository
                .GetAll()
                .Include(a => a.Asset)
                .Include(a => a.Assignee)
                .Include(a => a.Assignor)
                .Where(a => a.Id == id && a.Asset.Location == currentAdminLocation)
                .FirstOrDefaultAsync();

            if (assignment is null)
            {
                return null;
            }

            return new AssignmentDetailsDto
            {
                Id = assignment.Id,
                AssetCode = assignment.Asset.Code,
                AssetName = assignment.Asset.Name,
                AssetId = assignment.AssetId,
                AssignedBy = assignment.Assignor.Username,
                AssignedTo = assignment.Assignee.Username,
                AssignedDate = assignment.AssignedDate.ToString("dd/MM/yyyy"),
                AssignorId = assignment.AssignorId,
                AssigneeId = assignment.AssigneeId,
                AssigneeStaffCode = assignment.Assignee.StaffCode,
                State = assignment.State.GetDisplayName(),
                Specification = assignment.Asset.Specification,
                Note = assignment.Note,
            };
        }

        public async Task<AssignmentDto> CreateAssignmentAsync(Guid adminId, CreateAssignmentRequestDto dto)
        {
            await AssignmentValidator.ValidateCreateAssignmentAsync(dto, adminId, _assetRepository, _userRepository,
                _assignmentRepository);

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

        public async Task<AssignmentDto> UpdateAssignmentAsync(Guid id, Guid adminId, UpdateAssignmentRequestDto dto)
        {
            await AssignmentValidator.ValidateUpdateAssignmentAsync(id, dto, adminId, _assetRepository, _userRepository,
                _assignmentRepository);

            var assignment = await _assignmentRepository.GetByIdAsync(id);
            if (assignment is null)
            {
                throw new KeyNotFoundException($"Assignment with id {id} not found");
            }

            // Update fields if provided
            if (!string.IsNullOrWhiteSpace(dto.AssetId))
            {
                assignment.AssetId = Guid.Parse(dto.AssetId);
            }

            if (!string.IsNullOrWhiteSpace(dto.AssigneeId))
            {
                assignment.AssigneeId = Guid.Parse(dto.AssigneeId);
            }

            if (!string.IsNullOrWhiteSpace(dto.AssignedDate))
            {
                assignment.AssignedDate = DateTimeOffset.Parse(dto.AssignedDate);
            }

            if (dto.Note != null)
            {
                assignment.Note = dto.Note;
            }

            assignment.LastModifiedBy = adminId;
            assignment.LastModifiedDate = DateTime.UtcNow;

            _assignmentRepository.Update(assignment);
            await _assignmentRepository.SaveChangesAsync();

            var updated = await GetAssignmentByIdAsync(assignment.Id);
            return updated!;
        }

        public async Task<bool> DeleteAssignmentAsync(Guid id, Guid adminId)
        {
            var currentAdminLocation = await GetLocationByUserId(adminId);

            // TODO: move these validation logic to the validator class
            var assignment = await _assignmentRepository.GetAll()
                .Include(a => a.Asset)
                .Where(a => a.Id == id && a.Asset.Location == currentAdminLocation)
                .FirstOrDefaultAsync();

            if (assignment is null)
            {
                return false;
            }

            // Only allow deletion of waiting assignments
            if (assignment.State != AssignmentState.WaitingForAcceptance)
            {
                throw new InvalidOperationException("You can only delete assignments that are waiting for acceptance");
            }

            // Throw exception if this assignment is already deleted
            if (assignment.IsDeleted == true)
            {
                throw new ApiExceptionTypes.ConflictException("This assignment is already deleted by someone else.");
            }

            // Update asset state back to Available
            // We don't need to update the asset state since 'WaitingForAcceptance' assignment hasn't
            // changed the state of asset yet. (For now I will just comment out these lines below)
            // var asset = await _assetRepository.GetByIdAsync(assignment.AssetId);
            // if (asset != null)
            // {
            //     asset.State = AssetState.Available;
            //     _assetRepository.Update(asset);
            // }

            // Soft delete
            assignment.IsDeleted = true;
            assignment.DeletedBy = adminId;
            assignment.DeletedDate = DateTime.UtcNow;

            _assignmentRepository.Update(assignment);
            await _assignmentRepository.SaveChangesAsync();
            return true;
        }

        public async Task<AssignmentDto?> AcceptAssignmentAsync(Guid id, Guid userId)
        {
            // TODO: move these validation logic to the validator class
            var assignment = await _assignmentRepository.GetByIdAsync(id)
                ?? throw new KeyNotFoundException($"Assignment with id {id} not found");

            if (assignment.AssigneeId != userId)
            {
                throw new UnauthorizedAccessException("Only the assignee can accept this assignment");
            }

            if (assignment.State != AssignmentState.WaitingForAcceptance)
            {
                throw new InvalidOperationException("Can only accept assignments that are waiting for acceptance");
            }

            var asset = await _assetRepository.GetByIdAsync(assignment.AssetId);
            if (asset == null || asset.State != AssetState.Available)
            {
                throw new InvalidOperationException("This asset is no longer available for assignment.");
            }

            // Update asset state
            asset.State = AssetState.Assigned;
            _assetRepository.Update(asset);

            // Accept this assignment
            assignment.State = AssignmentState.Accepted;
            assignment.LastModifiedBy = userId;
            assignment.LastModifiedDate = DateTime.UtcNow;
            _assignmentRepository.Update(assignment);

            // TODO (optional): Decline other pending assignments for this asset

            // Save pending changes
            await _assignmentRepository.SaveChangesAsync();

            return await GetAssignmentByIdAsync(id);
        }

        public async Task<AssignmentDto?> DeclineAssignmentAsync(Guid id, Guid userId)
        {
            // TODO: move these validation logic to the validator class
            var assignment = await _assignmentRepository.GetByIdAsync(id);
            if (assignment == null)
            {
                throw new KeyNotFoundException($"Assignment with id {id} not found");
            }

            if (assignment.AssigneeId != userId)
            {
                throw new UnauthorizedAccessException("Only the assignee can decline this assignment");
            }

            if (assignment.State != AssignmentState.WaitingForAcceptance)
            {
                throw new InvalidOperationException("Can only decline assignments that are waiting for acceptance");
            }

            assignment.State = AssignmentState.Declined;
            assignment.LastModifiedBy = userId;
            assignment.LastModifiedDate = DateTime.UtcNow;

            // Update asset state to available (we might not need this)
            var asset = await _assetRepository.GetByIdAsync(assignment.AssetId);
            if (asset != null)
            {
                asset.State = AssetState.Available;
                _assetRepository.Update(asset);
            }

            _assignmentRepository.Update(assignment);
            await _assignmentRepository.SaveChangesAsync();

            return await GetAssignmentByIdAsync(id);
        }
    }
}