using AssetManagement.Domain.Entities;
using AssetManagement.Domain.Repositories;
using AssetManagement.Domain.Extensions;
using AssetManagement.Application.Services.Interfaces;
using AssetManagement.Contracts.Common.Pagination;
using AssetManagement.Contracts.DTOs;
using AssetManagement.Contracts.Parameters;
using Microsoft.EntityFrameworkCore;
using AssetManagement.Contracts.DTOs.Resquest;
using AssetManagement.Application.Extensions;
using AssetManagement.Domain.Enums;

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
                .Where(u => u.IsActive)
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

        public async Task<Guid> UpdateUserAsync(Guid userId, UpdateUserRequest request)
        {
            // Check if the user exists
            var user = await _userRepository.GetByIdAsync(userId);

            if (user is null)
            {
                throw new KeyNotFoundException($"Cannot find user with id {userId}");
            }

            // Validation field
            if (request.DateOfBirth != null && (request.DateOfBirth.Value > DateTime.UtcNow.AddYears(-18)))
            {
                throw new ArgumentException("User is under 18. Please select a different date.");
            }

            if(request.JoinedDate != null && 
                (request.JoinedDate.Value.Year < (request.DateOfBirth?.Year ?? user.DateOfBirth!.Value.Year + 18)))
            {

                throw new ArgumentException("User under the age of 18 may not join company. Please select a different date");

            } else if(request.JoinedDate != null && (request.JoinedDate.Value.DayOfWeek == DayOfWeek.Saturday 
                || request.JoinedDate.Value.DayOfWeek == DayOfWeek.Sunday))
            {
                throw new ArgumentException("Joined date is Saturday or Sunday. Please select a different date");
            }

            // Check valid UserType
            var type = request.Type?.GetEnum<UserTypeEnum>("Type should be Male or Female!");
            var gender = request.Gender?.GetEnum<GenderEnum>("Gender should be Male or Female!");

            // Update partial user
            user.DateOfBirth = request.DateOfBirth ?? user.DateOfBirth;
            user.Type = type ?? user.Type;
            user.Gender = gender ?? user.Gender;
            user.JoinedDate = request.JoinedDate ?? user.JoinedDate;
            user.LastModificatedDate = DateTime.UtcNow;

            // Update user
            _userRepository.Update(user);

            return userId;
        }

        public async Task<string> DeleteUser(Guid deletedBy, string staffCode)
        {
            // Check if the user exists
            var user = await _userRepository.GetAll()
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.StaffCode == staffCode)
                ?? throw new KeyNotFoundException($"Cannot find user with code {staffCode}");
            
            // Update state user to disable
            user.DeletedBy = deletedBy;
            user.DeletedDate = DateTime.UtcNow;
            user.IsActive = false;

            // Update user
            _userRepository.Update(user);

            return staffCode;
        }
    }
}