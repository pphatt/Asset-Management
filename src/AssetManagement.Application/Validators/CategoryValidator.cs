using AssetManagement.Contracts.DTOs.Requests;
using AssetManagement.Contracts.Exceptions;
using AssetManagement.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;
using static AssetManagement.Contracts.Exceptions.ApiExceptionTypes;
using Microsoft.EntityFrameworkCore;

namespace AssetManagement.Application.Validators
{
    public static class CategoryValidator
    {
        public static void ValidateCreateCategory(CreateCategoryRequestDto request)
        {
            var errors = new List<FieldValidationException>();

            AddErrorIfEmpty(errors, request.Name, "CategoryName", "Category name is required");
            AddErrorIfEmpty(errors, request.Name, "CategoryPrefix", "Prefix is required");
            ValidatePrefixLength(errors, request.Prefix, "CategoryPrefix", 
                "The category prefix must be exactly 2 characters and is mandatory");

            ThrowIfErrors(errors); 
        }
        
        public static async Task ValidateUserAsync(IUserRepository userRepository, string userId)
        {
            var user = await userRepository.GetAll()
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == Guid.Parse(userId))
                ?? throw new KeyNotFoundException($"Cannot find user with id: {userId}");
        }

        public static void ValidatePrefixLength(string prefix)
        {
            if (prefix.Trim().Length != 2)
                throw new ValidationException("Prefix must be exactly 2 characters long");
        }

        public static async Task ValidateDuplicateNameAsync(ICategoryRepository categoryRepository, string name)
        {
            var duplicatedName = await categoryRepository.GetAll()
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Name.ToLower() == name.ToLower());
            if (duplicatedName is not null)
                throw new DuplicateResourceException("Category is already existed. Please enter a different category");
        }

        public static async Task ValidateDuplicatePrefixAsync(ICategoryRepository categoryRepository, string prefix)
        {
            var duplicatedPrefix = await categoryRepository.GetAll()
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Prefix.ToLower() == prefix.ToLower());
            if (duplicatedPrefix is not null)
                throw new DuplicateResourceException("Prefix is already existed. Please enter a different prefix");
        }

        private static void AddErrorIfEmpty(List<FieldValidationException> errors, 
            string value, 
            string field, 
            string message)
        {
            if (string.IsNullOrWhiteSpace(value))
                errors.Add(new FieldValidationException(field, message));
        }

        private static void ValidatePrefixLength(List<FieldValidationException> errors,
            string value,
            string field, 
            string message)
        {
            if (value.Trim().Length != 2)
                errors.Add(new FieldValidationException(field, message));
        }

        private static void ThrowIfErrors(List<FieldValidationException> errors)
        {
            if (errors.Count > 0) throw new AggregateFieldValidationException(errors);
        }
    }
}
