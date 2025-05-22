using AssetManagement.Domain.Entities;
using AssetManagement.Domain.Repositories;
using AssetManagement.Domain.Extensions;
using AssetManagement.Application.Services.Interfaces;
using AssetManagement.Contracts.Common.Pagination;
using AssetManagement.Contracts.DTOs;
using AssetManagement.Contracts.DTOs.Response;
using AssetManagement.Contracts.DTOs.Resquest;
using AssetManagement.Contracts.Enums;
using AssetManagement.Contracts.Exceptions;
using AssetManagement.Contracts.Parameters;
using AssetManagement.Data.Helpers.Hashing;
using AssetManagement.Data.Helpers.Users;
using AssetManagement.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using AssetManagement.Application.Extensions;

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

        private async Task<LocationEnum> GetCurrentAdminLocation(string userId)
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

        public async Task<CreateUserResponseDto> CreateUserAsync(string adminUserId, CreateUserRequestDto dto)
        {
            var errors = new List<FieldValidationException>();

            if (dto == null)
            {
                throw new ArgumentNullException(nameof(dto));
            }
            else
            {
                if (string.IsNullOrWhiteSpace(dto.FirstName))
                    errors.Add(new FieldValidationException("FirstName", "First name is required"));

                if (string.IsNullOrWhiteSpace(dto.LastName))
                    errors.Add(new FieldValidationException("LastName", "Last name is required"));

                // Validate Date of Birth
                if (string.IsNullOrWhiteSpace(dto.DateOfBirth))
                {
                    errors.Add(new FieldValidationException("DateOfBirth", "Date of Birth is required"));
                }

                // Validate Joined Date
                if (string.IsNullOrWhiteSpace(dto.JoinedDate))
                {
                    errors.Add(new FieldValidationException("JoinedDate", "Joined date is required"));
                }

                // Validate user type is specified
                if (dto.Type == 0 || !Enum.IsDefined(typeof(UserTypeDtoEnum), dto.Type))
                    errors.Add(new FieldValidationException("Type", "Invalid user type value"));

                // Validate gender is specified
                if (dto.Gender == 0 || !Enum.IsDefined(typeof(GenderDtoEnum), dto.Gender))
                    errors.Add(new FieldValidationException("Gender", "Invalid gender value"));
            }

            // Early return if basic required fields are missing
            if (errors.Any())
                throw new AggregateFieldValidationException(errors);

            // Parse DateOfBirth
            DateTime dateOfBirth;
            if (!DateTime.TryParse(dto.DateOfBirth, out dateOfBirth))
                errors.Add(new FieldValidationException("DateOfBirth", "Invalid Date of Birth format"));
            else
            {
                if (dateOfBirth.Year < 1900 || dateOfBirth.Year > DateTime.Today.Year)
                    errors.Add(new FieldValidationException("DateOfBirth", "Invalid Date of Birth value"));

                // Check if user is under 18
                var today = DateTime.Today;
                var age = today.Year - dateOfBirth.Year;
                if (dateOfBirth.Date > today.AddYears(-age))
                    age--;
                if (age < 18)
                    errors.Add(new FieldValidationException("DateOfBirth",
                        "User is under 18. Please select a different date"));
            }

            // Parse JoinedDate
            DateTimeOffset joinedDate;
            if (!DateTimeOffset.TryParse(dto.JoinedDate, out joinedDate))
                errors.Add(new FieldValidationException("JoinedDate", "Invalid Joined Date format"));
            else
            {
                if (joinedDate.Year < 1900 || joinedDate.Year > DateTime.Today.AddYears(1).Year)
                    errors.Add(new FieldValidationException("JoinedDate", "Invalid Joined Date value"));

                // Check if joined date is on weekend
                if (joinedDate.DayOfWeek == DayOfWeek.Saturday || joinedDate.DayOfWeek == DayOfWeek.Sunday)
                    errors.Add(new FieldValidationException("JoinedDate",
                        "Joined date is Saturday or Sunday. Please select a different date"));

                // Check if joined date is before person turns 18
                if (DateTime.TryParse(dto.DateOfBirth, out dateOfBirth))
                {
                    var ageAtJoining = joinedDate.Year - dateOfBirth.Year;
                    if (dateOfBirth.Date > joinedDate.Date.AddYears(-ageAtJoining))
                        ageAtJoining--;
                    if (ageAtJoining < 18)
                        errors.Add(new FieldValidationException("JoinedDate",
                            "User under the age of 18 may not join company. Please select a different date"));
                }
            }

            if (errors.Any())
                throw new AggregateFieldValidationException(errors);

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

            _userRepository.Add(user);

            return new CreateUserResponseDto
            {
                StaffCode = user.StaffCode,
                Username = user.Username,
                FullName = $"{user.LastName} {user.FirstName}",
                Location = MapLocationToDto(location)
            };
        }

        private static UserTypeEnum MapUserType(UserTypeDtoEnum dto)
        {
            return dto switch
            {
                UserTypeDtoEnum.Admin => UserTypeEnum.Admin,
                UserTypeDtoEnum.Staff => UserTypeEnum.Staff,
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        private static GenderEnum MapGender(GenderDtoEnum dto)
        {
            return dto switch
            {
                GenderDtoEnum.Male => GenderEnum.Male,
                GenderDtoEnum.Female => GenderEnum.Female,
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        private static LocationDtoEnum MapLocationToDto(LocationEnum domain)
        {
            return domain switch
            {
                LocationEnum.HCM => LocationDtoEnum.HCM,
                LocationEnum.DN => LocationDtoEnum.DN,
                LocationEnum.HN => LocationDtoEnum.HN,
                _ => throw new ArgumentOutOfRangeException()
            };
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
            user.LastModifiedDate = DateTime.UtcNow;

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