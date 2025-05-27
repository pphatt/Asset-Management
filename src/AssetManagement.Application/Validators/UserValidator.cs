using AssetManagement.Contracts.DTOs.Requests;
using AssetManagement.Contracts.Exceptions;
using AssetManagement.Domain.Entities;
using AssetManagement.Domain.Enums;

namespace AssetManagement.Application.Validators
{
    public static class UserValidator
    {
        public static void ValidateCreateUser(CreateUserRequestDto dto)
        {
            var errors = new List<FieldValidationException>();

            // Basic required fields
            AddErrorIfEmpty(errors, dto.FirstName, "FirstName", "First name is required");
            AddErrorIfEmpty(errors, dto.LastName, "LastName", "Last name is required");
            AddErrorIfEmpty(errors, dto.DateOfBirth, "DateOfBirth", "Date of birth is required");
            AddErrorIfEmpty(errors, dto.JoinedDate, "JoinedDate", "Joined date is required");

            // Enum validations
            AddErrorIfInvalidEnum(errors, dto.Type, "Type", "Invalid user type value");
            AddErrorIfInvalidEnum(errors, dto.Gender, "Gender", "Invalid gender value");

            ThrowIfErrors(errors); // Early return for basic validations

            // Date validations
            var dateOfBirth = ValidateDateOfBirth(errors, dto.DateOfBirth);
            var joinedDate = ValidateJoinedDate(errors, dto.JoinedDate);

            // Cross-field validation
            ValidateAgeAtJoining(errors, dateOfBirth, joinedDate);

            ThrowIfErrors(errors);
        }

        public static void ValidateUpdateUser(UpdateUserRequestDto dto, User existingUser)
        {
            var errors = new List<FieldValidationException>();

            // Check if user can be updated
            if (!existingUser.IsActive)
                throw new InvalidOperationException("Cannot update disabled user");

            // Optional enum validations
            if (dto.Type.HasValue)
                AddErrorIfInvalidEnum(errors, dto.Type.Value, "Type", "Invalid user type value");

            if (dto.Gender.HasValue)
                AddErrorIfInvalidEnum(errors, dto.Gender.Value, "Gender", "Invalid gender value");

            // Date validations
            DateTime? newDateOfBirth = null;
            DateTimeOffset? newJoinedDate = null;

            if (!string.IsNullOrWhiteSpace(dto.DateOfBirth))
                newDateOfBirth = ValidateDateOfBirth(errors, dto.DateOfBirth);

            if (!string.IsNullOrWhiteSpace(dto.JoinedDate))
                newJoinedDate = ValidateJoinedDate(errors, dto.JoinedDate);

            // Use effective dates for cross-validation
            var effectiveDateOfBirth = newDateOfBirth ?? existingUser.DateOfBirth;
            var effectiveJoinedDate = newJoinedDate ?? existingUser.JoinedDate;

            // Cross-field validation
            if (effectiveDateOfBirth.HasValue)
                ValidateAgeAtJoining(errors, effectiveDateOfBirth.Value, effectiveJoinedDate);
            else if (newJoinedDate.HasValue)
                errors.Add(new FieldValidationException("JoinedDate", "Please Select Date of Birth"));

            ThrowIfErrors(errors);
        }

        private static void AddErrorIfEmpty(List<FieldValidationException> errors, string value, string field, string message)
        {
            if (string.IsNullOrWhiteSpace(value))
                errors.Add(new FieldValidationException(field, message));
        }

        private static void AddErrorIfInvalidEnum<T>(List<FieldValidationException> errors, T value, string field, string message) where T : Enum
        {
            if (Convert.ToInt32(value) == 0 || !Enum.IsDefined(typeof(T), value))
                errors.Add(new FieldValidationException(field, message));
        }

        private static DateTime ValidateDateOfBirth(List<FieldValidationException> errors, string dateString)
        {
            if (!DateTime.TryParse(dateString, out var date))
            {
                errors.Add(new FieldValidationException("DateOfBirth", "Invalid Date of Birth format"));
                return default;
            }

            if (date.Year < 1900 || date.Year > DateTime.Today.Year)
            {
                errors.Add(new FieldValidationException("DateOfBirth", "Invalid Date of Birth value"));
                return default;
            }

            var age = DateTime.Today.Year - date.Year;
            if (date.Date > DateTime.Today.AddYears(-age)) age--;

            if (age < 18)
                errors.Add(new FieldValidationException("DateOfBirth", "User is under 18. Please select a different date"));

            return date;
        }

        private static DateTimeOffset ValidateJoinedDate(List<FieldValidationException> errors, string dateString)
        {
            if (!DateTimeOffset.TryParse(dateString, out var date))
            {
                errors.Add(new FieldValidationException("JoinedDate", "Invalid Joined Date format"));
                return default;
            }

            if (date.Year < 1900 || date.Year > DateTime.Today.AddYears(1).Year)
            {
                errors.Add(new FieldValidationException("JoinedDate", "Invalid Joined Date value"));
                return default;
            }

            if (date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday)
                errors.Add(new FieldValidationException("JoinedDate", "Joined date is Saturday or Sunday. Please select a different date"));

            return date;
        }

        private static void ValidateAgeAtJoining(List<FieldValidationException> errors, DateTime dateOfBirth, DateTimeOffset joinedDate)
        {
            var ageAtJoining = joinedDate.Year - dateOfBirth.Year;
            if (dateOfBirth.Date > joinedDate.Date.AddYears(-ageAtJoining)) ageAtJoining--;

            if (ageAtJoining < 18)
                errors.Add(new FieldValidationException("JoinedDate", "User under the age of 18 may not join company. Please select a different date"));
        }

        private static void ThrowIfErrors(List<FieldValidationException> errors)
        {
            if (errors.Any())
                throw new AggregateFieldValidationException(errors);
        }
    }
}