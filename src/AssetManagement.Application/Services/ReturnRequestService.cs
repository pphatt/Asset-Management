using AssetManagement.Application.Services.Interfaces;
using AssetManagement.Application.Validators;
using AssetManagement.Application.Extensions;
using AssetManagement.Contracts.Common.Pagination;
using AssetManagement.Contracts.DTOs;
using AssetManagement.Contracts.Parameters;
using AssetManagement.Domain.Entities;
using AssetManagement.Domain.Enums;
using AssetManagement.Domain.Extensions;
using AssetManagement.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace AssetManagement.Application.Services
{
    public class ReturnRequestService : IReturnRequestService
    {
        private readonly IReturnRequestRepository _returnRequestRepository;
        private readonly IUserRepository _userRepository;

        public ReturnRequestService(IReturnRequestRepository returnRequestRepository, IUserRepository userRepository)
        {
            _returnRequestRepository = returnRequestRepository;
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

        public async Task<PagedResult<ReturnRequestDto>> GetReturnRequestsAsync(Guid adminId, ReturnRequestQueryParameters queryParams)
        {
            var currentAdminLocation = await GetLocationByUserId(adminId);

            var stateFilters = ReturnRequestValidator.ParseStates(queryParams.States);
            var dateFilter = ReturnRequestValidator.ParseDate(queryParams.ReturnedDate);

            IQueryable<ReturnRequest> query = _returnRequestRepository.GetAll()
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
                        idx + (queryParams.PageSize * (queryParams.PageNumber - 1)),
                    AssetCode = rr.Assignment.Asset.Code,
                    AssetName = rr.Assignment.Asset.Name,
                    AssignedDate = rr.Assignment.AssignedDate.ToString("dd/MM/yyyy"),
                    RequestedBy = rr.Requester.Username,
                    AcceptedBy = rr.Acceptor.Username,
                    ReturnedDate = rr.ReturnedDate.ToString("dd/MM/yyyy"),
                    State = rr.State.GetDisplayName(),
                }).ToList();

            return new PagedResult<ReturnRequestDto>(items, total, queryParams.PageSize, queryParams.PageNumber);
        }
    }
}