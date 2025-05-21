using AssetManagement.Domain.Entities;
using AssetManagement.Domain.Repositories;
using AssetManagement.Domain.Extensions;
using AssetManagement.Application.Services.Interfaces;
using AssetManagement.Contracts.Common.Pagination;
using AssetManagement.Contracts.DTOs;
using AssetManagement.Contracts.Parameters;
using Microsoft.EntityFrameworkCore;

namespace AssetManagement.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<PagedResult<UserDto>> GetUsersAsync(string userId, UserQueryParameters queryParams)
        {
            var user = await _userRepository.GetByIdAsync(Guid.Parse(userId));
            if (user is null)
            {
                throw new KeyNotFoundException($"Cannot find user with id {userId}");
            }

            IQueryable<User> query = _userRepository.GetAll()
                .ApplySearch(queryParams.SearchTerm)
                .ApplyFilters(queryParams.UserType, user.Location)
                .ApplySorting(queryParams.GetSortCriteria());

            int total = await query.CountAsync();

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