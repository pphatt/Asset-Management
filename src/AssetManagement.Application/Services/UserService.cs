using AssetManagement.Domain.Entities;
using AssetManagement.Domain.Repositories;
using AssetManagement.Domain.Extensions;
using AssetManagement.Application.Services.Interfaces;
using AssetManagement.Contracts.Common.Pagination;
using AssetManagement.Contracts.DTOs;
using AssetManagement.Contracts.Parameters;
using Microsoft.EntityFrameworkCore;

namespace AssetManagement.Domain.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<PagedResult<UserDto>> GetUsersAsync(UserQueryParameters queryParams)
        {
            IQueryable<User> query = _userRepository.GetAll();

            int total = await query.CountAsync();

            IQueryable<User> processedQuery = query
                .ApplySearch(queryParams.SearchTerm)
                .ApplyFilters(queryParams.UserType)
                .ApplySorting(queryParams.GetSortCriteria());

            var items = await query
                .Skip((queryParams.PageNumber - 1) * queryParams.PageSize)
                .Take(queryParams.PageSize)
                .Select(u => new UserDto
                {
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    JoinedDate = u.JoinedDate,
                    StaffCode = u.StaffCode,
                    Type = u.Type.ToString(),
                    Username = u.Username,
                })
                .ToListAsync();

            return new PagedResult<UserDto>(items, total, queryParams.PageSize, queryParams.PageNumber);
        }
    }
}