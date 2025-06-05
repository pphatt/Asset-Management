using AssetManagement.Contracts.DTOs.Requests;
using AssetManagement.Contracts.Exceptions;

namespace AssetManagement.Application.Validators
{
    public static class CreateReturningRequestValidator
    {
        public static void Validate(CreateReturnRequestDto request)
        {
            var errors = new List<FieldValidationException>();

            AddErrorIfEmpty(errors, request.AssignmentId, "AssignmentId", "Assignment id is required");

            ThrowIfErrors(errors);
        }

        private static void AddErrorIfEmpty(List<FieldValidationException> errors,
            string value,
            string field,
            string message)
        {
            if (string.IsNullOrWhiteSpace(value))
                errors.Add(new FieldValidationException(field, message));
        }

        private static void ThrowIfErrors(List<FieldValidationException> errors)
        {
            if (errors.Count > 0)
                throw new AggregateFieldValidationException(errors);
        }
    }
}
