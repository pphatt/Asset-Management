using AssetManagement.Application.Extensions;
using AssetManagement.Application.Services.Interfaces;
using AssetManagement.Application.Validators;
using AssetManagement.Contracts.Common.Pagination;
using AssetManagement.Contracts.DTOs;
using AssetManagement.Contracts.Enums;
using AssetManagement.Contracts.Parameters;
using AssetManagement.Domain.Entities;
using AssetManagement.Domain.Enums;
using AssetManagement.Domain.Extensions;
using AssetManagement.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;
using static AssetManagement.Contracts.Exceptions.ApiExceptionTypes;
using AssetManagement.Contracts.DTOs.Responses;
using AssetManagement.Contracts.DTOs.Requests;

namespace AssetManagement.Application.Services
{
    public class ReturnRequestService : IReturnRequestService
    {
        private readonly IReturnRequestRepository _returnRequestRepository;
        private readonly IUserRepository _userRepository;
        private readonly IAssignmentRepository _assignmentRepository;

        public ReturnRequestService(IReturnRequestRepository returnRequestRepository,
            IUserRepository userRepository,
            IAssignmentRepository assignmentRepository)
        {
            ArgumentNullException.ThrowIfNull(returnRequestRepository);
            ArgumentNullException.ThrowIfNull(userRepository);
            ArgumentNullException.ThrowIfNull(assignmentRepository);

            _returnRequestRepository = returnRequestRepository;
            _userRepository = userRepository;
            _assignmentRepository = assignmentRepository;
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

        public async Task<PagedResult<ReturnRequestDto>> GetReturnRequestsAsync(Guid adminId, ReturnRequestQueryParameters queryParams)
        {
            var currentAdminLocation = await GetLocationByUserId(adminId);

            var stateFilters = ReturnRequestValidator.ParseStates(queryParams.States);
            var dateFilter = ReturnRequestValidator.ParseDate(queryParams.ReturnedDate);

            IQueryable<ReturnRequest> query = _returnRequestRepository.GetAll()
                .AsNoTracking()
                .Include(rr => rr.Assignment)
                .ThenInclude(a => a.Asset)
                .Include(rr => rr.Requester)
                .Include(rr => rr.Acceptor)
                .ApplySearch(queryParams.SearchTerm)
                .ApplyFilters(states: stateFilters, date: dateFilter, location: currentAdminLocation)
                .ApplySorting(queryParams.GetSortCriteria())
                .Where(rr => rr.IsDeleted != true);

            int total = await query.CountAsync();
            bool isNoDescending = queryParams.GetSortCriteria()
                .Any(s => s.property.Equals("no", StringComparison.OrdinalIgnoreCase) &&
                        s.order.Equals("desc", StringComparison.OrdinalIgnoreCase));

            var returnRequests = await query
                .Skip((queryParams.PageNumber - 1) * queryParams.PageSize)
                .Take(queryParams.PageSize)
                .ToListAsync();

            var items = returnRequests
                .Select((rr, idx) => new ReturnRequestDto
                {
                    Id = rr.Id,
                    No = isNoDescending ?
                        total - (idx + (queryParams.PageSize * (queryParams.PageNumber - 1))) :
                        idx + (queryParams.PageSize * (queryParams.PageNumber - 1)) + 1,
                    AssetCode = rr.Assignment.Asset.Code,
                    AssetName = rr.Assignment.Asset.Name,
                    AssignedDate = rr.Assignment.AssignedDate.ToString("dd/MM/yyyy"),
                    RequestedBy = rr.Requester.Username,
                    AcceptedBy = rr.Acceptor == null ? "" : rr.Acceptor.Username,
                    ReturnedDate = rr.ReturnedDate == default ? "" : rr.ReturnedDate?.ToString("dd/MM/yyyy"),
                    State = rr.State.GetDisplayName(),
                }).ToList();

            return new PagedResult<ReturnRequestDto>(items, total, queryParams.PageSize, queryParams.PageNumber);
        }

        public async Task<bool> AcceptReturnRequestAsync(Guid returnRequestId, Guid userId)
        {
            var returnRequest = await _returnRequestRepository
                .GetAll()
                .Where(a => a.Id.Equals(returnRequestId))
                .Include(a => a.Assignment)
                    .ThenInclude(a => a.Asset)
                .FirstOrDefaultAsync();

            if (returnRequest == null)
            {
                throw new KeyNotFoundException($"ReturnRequest with id {returnRequestId} not found");
            }
            else if (returnRequest.State == ReturnRequestState.Completed)
            {
                throw new InvalidOperationException($"ReturnRequest with id {returnRequestId} is already completed");
            }

            // Update the state of the return request
            returnRequest.State = ReturnRequestState.Completed;
            returnRequest.ReturnedDate = DateTime.UtcNow;
            returnRequest.AcceptorId = userId;

            // Update the assignment states
            returnRequest.Assignment.State = AssignmentState.Returned;
            returnRequest.Assignment.LastModifiedDate = DateTime.UtcNow;

            // Update the asset state
            returnRequest.Assignment.Asset.State = AssetState.Available;
            returnRequest.Assignment.Asset.LastModifiedDate = DateTime.UtcNow;

            _returnRequestRepository.Update(returnRequest);

            return await _returnRequestRepository.SaveChangesAsync();
        }

        public async Task<CreateReturnRequestResponseDto> CreateReturnRequestAsync(string assignmentId,
            string requesterId,
            string role)
        {
            CreateReturningRequestValidator.Validate(new CreateReturnRequestDto
            {
                AssignmentId = assignmentId,
            });

            if (role == UserType.Admin.ToString())
            {
                var assignment = await _assignmentRepository.GetAll()
                    .AsNoTracking()
                    .Include(a => a.Asset)
                    .FirstOrDefaultAsync(a => a.Id == Guid.Parse(assignmentId) && a.IsDeleted != true)
                    ?? throw new KeyNotFoundException("Assignment does not exist");

                if (assignment.State != AssignmentState.Accepted)
                    throw new ConflictException($"Cannot return the asset with assignment's state is: {assignment.State.GetDisplayName()}");

                assignment.State = AssignmentState.WaitingForReturning;
                _assignmentRepository.Update(assignment);

                var returningRequest = new ReturnRequest
                {
                    Id = Guid.NewGuid(),
                    AssignmentId = assignment.Id,
                    RequesterId = Guid.Parse(requesterId),
                    CreatedBy = Guid.Parse(requesterId),
                    CreatedDate = DateTime.UtcNow,
                    State = ReturnRequestState.WaitingForReturning,
                };
                await _returnRequestRepository.AddAsync(returningRequest);
                await _returnRequestRepository.SaveChangesAsync();

                return new CreateReturnRequestResponseDto
                {
                    AssetCode = assignment.Asset.Code,
                    AssignmentStatus = assignment.State.GetDisplayName(),
                };
            }
            else
            {
                var assignment = await _assignmentRepository.GetAll()
                    .AsNoTracking()
                    .Include(a => a.Asset)
                    .FirstOrDefaultAsync(a => a.Id == Guid.Parse(assignmentId)
                        && a.AssigneeId == Guid.Parse(requesterId)
                        && a.IsDeleted != true)
                    ?? throw new KeyNotFoundException("Assignment does not exist");

                if (assignment.State != AssignmentState.Accepted)
                    throw new ConflictException($"Cannot return the asset with assignment's state is: {assignment.State.GetDisplayName()}");

                assignment.State = AssignmentState.WaitingForReturning;
                _assignmentRepository.Update(assignment);

                var returningRequest = new ReturnRequest
                {
                    Id = Guid.NewGuid(),
                    AssignmentId = assignment.Id,
                    RequesterId = Guid.Parse(requesterId),
                    CreatedBy = Guid.Parse(requesterId),
                    CreatedDate = DateTime.UtcNow,
                    State = ReturnRequestState.WaitingForReturning,
                };
                await _returnRequestRepository.AddAsync(returningRequest);
                await _returnRequestRepository.SaveChangesAsync();

                return new CreateReturnRequestResponseDto
                {
                    AssetCode = assignment.Asset.Code,
                    AssignmentStatus = assignment.State.GetDisplayName(),
                };
            }
        }
        
        public async Task<string> CancelReturnRequestAsync(Guid returnRequestId, Guid adminId)
        {
            var returnRequest = await _returnRequestRepository.GetByIdAsync(returnRequestId);

            if (returnRequest == null || returnRequest.IsDeleted == true)
            {
                throw new KeyNotFoundException($"Return request with id {returnRequestId} not found");
            }

            if (returnRequest.State != ReturnRequestState.WaitingForReturning)
            {
                throw new InvalidOperationException($"Return request with id {returnRequestId} is already completed.");
            }
            
            if(returnRequest.IsDeleted == true)
            {
                throw new InvalidOperationException($"Return request is already cancelled.");
            }

            var admin = await _userRepository.GetByIdAsync(adminId);
            if (admin == null)
            {
                throw new KeyNotFoundException($"Admin with id {adminId} not found");
            }

            var assignment = await _assignmentRepository.GetByIdAsync(returnRequest.AssignmentId);
            if (assignment == null)
            {
                throw new KeyNotFoundException($"Assignment with id {returnRequest.AssignmentId} not found");
            }
            assignment.State = AssignmentState.Accepted;
            _assignmentRepository.Update(assignment);

            _returnRequestRepository.Delete(returnRequest);
            await _returnRequestRepository.SaveChangesAsync();

            return returnRequest.Id.ToString();
        }
    }
}