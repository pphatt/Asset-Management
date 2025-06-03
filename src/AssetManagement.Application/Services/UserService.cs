using AssetManagement.Domain.Entities;
using AssetManagement.Domain.Extensions;
using AssetManagement.Application.Services.Interfaces;
using AssetManagement.Contracts.Common.Pagination;
using AssetManagement.Contracts.DTOs;
using AssetManagement.Contracts.Enums;
using AssetManagement.Contracts.Parameters;
using AssetManagement.Data.Helpers.Hashing;
using AssetManagement.Data.Helpers.Users;
using AssetManagement.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using AssetManagement.Contracts.DTOs.Responses;
using AssetManagement.Contracts.DTOs.Requests;
using static AssetManagement.Contracts.Exceptions.ApiExceptionTypes;
using AssetManagement.Domain.Interfaces.Repositories;
using AssetManagement.Application.Validators;

namespace AssetManagement.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;

        public UserService(IUserRepository userRepository, IPasswordHasher passwordHasher)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
        }

        private async Task<Location> GetCurrentAdminLocation(string userId)
        {
            var adminUser = await _userRepository.GetByIdAsync(Guid.Parse(userId));

            if (adminUser is null)
                throw new UnauthorizedAccessException("Cannot find admin to extract location");

            return adminUser.Location;
        }

        public async Task<PagedResult<UserDto>> GetUsersAsync(string userId, UserQueryParameters queryParams)
        {
            var user = await _userRepository.GetByIdAsync(Guid.Parse(userId));
            if (user is null)
            {
                throw new KeyNotFoundException($"Cannot find user with id {userId}");
            }

            IQueryable<User> query = _userRepository.GetAll()
                .Include(x => x.Assignments)
                .ApplySearch(queryParams.SearchTerm)
                .ApplyFilters(queryParams.UserType, user.Location)
                .ApplySorting(queryParams.GetSortCriteria())
                .Where(u => u.IsActive);

            int total = await query.CountAsync();

            var items = await query
                .Skip((queryParams.PageNumber - 1) * queryParams.PageSize)
                .Take(queryParams.PageSize)
                .Select(u => new UserDto
                {
                    Id = u.Id,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    JoinedDate = u.JoinedDate,
                    StaffCode = u.StaffCode,
                    Type = u.Type.ToString(),
                    Username = u.Username,
                    HasAssignment = u.Assignments.Count > 0,
                })
                .ToListAsync();

            return new PagedResult<UserDto>(items, total, queryParams.PageSize, queryParams.PageNumber);
        }

        public async Task<UserDetailsDto?> GetUserByIdAsync(Guid id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user is null)
            {
                return null;
            }

            return new UserDetailsDto
            {
                Id = user.Id,
                StaffCode = user.StaffCode,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Username = user.Username,
                DateOfBirth = user.DateOfBirth?.ToString("dd/MM/yyyy"),
                JoinedDate = user.JoinedDate.ToString("dd/MM/yyyy"),
                Gender = (int)user.Gender,
                Type = (int)user.Type,
                Location = (int)user.Location,
            };
        }

        public async Task<UserDetailsDto> GetUserByStaffCodeAsync(string staffCode)
        {
            if (string.IsNullOrWhiteSpace(staffCode))
            {
                throw new ArgumentException("Staff code cannot be empty or null", nameof(staffCode));
            }

            var user = await _userRepository.GetByStaffCodeAsync(staffCode);
            if (user is null)
            {
                throw new KeyNotFoundException($"Cannot find user with staff code {staffCode}");
            }

            return new UserDetailsDto
            {
                Id = user.Id,
                StaffCode = user.StaffCode,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Username = user.Username,
                DateOfBirth = user.DateOfBirth?.ToString("dd/MM/yyyy"),
                JoinedDate = user.JoinedDate.ToString("dd/MM/yyyy"),
                Gender = (int)user.Gender,
                Type = (int)user.Type,
                Location = (int)user.Location,
            };
        }

        public async Task<CreateUserResponseDto> CreateUserAsync(string adminUserId, CreateUserRequestDto dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            UserValidator.ValidateCreateUser(dto);

            var dateOfBirth = DateTime.Parse(dto.DateOfBirth);
            var joinedDate = DateTime.Parse(dto.JoinedDate);

            var existingUsernames = await _userRepository.GetAllUsernamesAsync();
            var lastStaffCodes = await _userRepository.GetStaffCodesAsync();

            var staffCode = UserGenerationHelper.GenerateStaffCode(lastStaffCodes);
            var username = UserGenerationHelper.GenerateUsername(dto.FirstName, dto.LastName, existingUsernames);
            var password = UserGenerationHelper.GeneratePassword(username, dateOfBirth);

            var hashedPassword = _passwordHasher.HashPassword(password);

            var location = await GetCurrentAdminLocation(adminUserId);

            var user = new User
            {
                StaffCode = staffCode,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Username = username,
                Password = hashedPassword,
                DateOfBirth = dateOfBirth,
                JoinedDate = joinedDate,
                Type = MapUserType(dto.Type),
                Gender = MapGender(dto.Gender),
                Location = location,
                CreatedBy = Guid.Parse(adminUserId),
                CreatedDate = DateTime.UtcNow,
            };

            await _userRepository.AddAsync(user);
            await _userRepository.SaveChangesAsync();

            return new CreateUserResponseDto
            {
                StaffCode = user.StaffCode,
                Username = user.Username,
                FullName = $"{user.LastName} {user.FirstName}",
                Location = MapLocationToDto(location)
            };
        }

        public async Task<string> UpdateUserAsync(string adminId, string staffCode, UpdateUserRequestDto dto)
        {
            if (dto == null)
            {
                throw new ArgumentNullException(nameof(dto));
            }

            // Get existing user
            var existingUser = await _userRepository.GetByStaffCodeAsync(staffCode);
            if (existingUser == null)
            {
                throw new NotFoundException($"User with staff code {staffCode} not found");
            }

            UserValidator.ValidateUpdateUser(dto, existingUser);

            // Update the user properties
            if (!string.IsNullOrWhiteSpace(dto.DateOfBirth))
                existingUser.DateOfBirth = DateTime.Parse(dto.DateOfBirth);

            if (!string.IsNullOrWhiteSpace(dto.JoinedDate))
                existingUser.JoinedDate = DateTimeOffset.Parse(dto.JoinedDate);

            if (dto.Type.HasValue)
                existingUser.Type = MapUserType(dto.Type.Value);

            if (dto.Gender.HasValue)
                existingUser.Gender = MapGender(dto.Gender.Value);

            existingUser.LastModifiedBy = Guid.Parse(adminId);
            existingUser.LastModifiedDate = DateTime.UtcNow;

            _userRepository.Update(existingUser);
            await _userRepository.SaveChangesAsync();
            return existingUser.StaffCode;
        }

        public async Task<string> DeleteUserAsync(Guid deletedBy, string staffCode)
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
            await _userRepository.SaveChangesAsync();

            return staffCode;
        }

        private static UserType MapUserType(UserTypeDto dto)
        {
            return dto switch
            {
                UserTypeDto.Admin => UserType.Admin,
                UserTypeDto.Staff => UserType.Staff,
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        private static Gender MapGender(GenderDto dto)
        {
            return dto switch
            {
                GenderDto.Male => Gender.Male,
                GenderDto.Female => Gender.Female,
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        private static LocationDto MapLocationToDto(Location domain)
        {
            return domain switch
            {
                Location.HCM => LocationDto.HCM,
                Location.DN => LocationDto.DN,
                Location.HN => LocationDto.HN,
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }
}