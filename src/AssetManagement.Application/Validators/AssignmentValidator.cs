using System.Security.Cryptography.X509Certificates;
using AssetManagement.Contracts.DTOs.Requests;
using AssetManagement.Contracts.Exceptions;
using AssetManagement.Domain.Enums;
using AssetManagement.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace AssetManagement.Application.Validators
{
    public static class AssignmentValidator
    {
        public static IList<AssignmentState> ParseStates(IList<string>? stateStrings)
        {
            IList<AssignmentState> states = new List<AssignmentState>();
            if (stateStrings is not null)
            {
                foreach (var stateString in stateStrings)
                {
                    if (!string.IsNullOrEmpty(stateString) && Enum.TryParse<AssignmentState>(stateString, true, out var state))
                    {
                        states.Add(state);
                    }
                }
            }
            return states;
        }

        public static DateTimeOffset? ParseDate(string? dateString)
        {
            if (string.IsNullOrEmpty(dateString))
                return null;
            if (DateTimeOffset.TryParse(dateString, out var date))
                return date;
            throw new ArgumentException("Invalid date format for filtering.");
        }

        public static async Task ValidateCreateAssignmentAsync(
            CreateAssignmentRequestDto dto,
            Guid adminId,
            IAssetRepository assetRepository,
            IUserRepository userRepository,
            IAssignmentRepository assignmentRepository
        )
        {
            var errors = new List<FieldValidationException>();

            // Basic required fields
            AddErrorIfEmpty(errors, dto.AssetId, "AssetId", "AssetId is required");
            AddErrorIfEmpty(errors, dto.AssigneeId, "AssigneeId", "AssigneeId is required");

            // Parse and validate GUIDs
            if (!Guid.TryParse(dto.AssetId, out var assetId))
                errors.Add(new FieldValidationException("AssetId", "Invalid Asset ID format"));

            if (!Guid.TryParse(dto.AssigneeId, out var assigneeId))
                errors.Add(new FieldValidationException("AssigneeId", "Invalid Assignee ID format"));

            DateTimeOffset? assignedDate = null;
            if (!string.IsNullOrWhiteSpace(dto.AssignedDate))
            {
                assignedDate = ValidateAssignedDate(errors, dto.AssignedDate);
            }

            ThrowIfErrors(errors);

            var admin = await userRepository.GetByIdAsync(adminId);
            if (admin is null)
            {
                throw new UnauthorizedAccessException("Admin user not found");
            }

            var adminLocation = admin.Location;

            // Async validations
            await ValidateAssetForAssignmentAsync(errors, assetId, adminLocation, assetRepository, assignmentRepository);
            await ValidateAssigneeAsync(errors, assigneeId, adminLocation, userRepository);

            ThrowIfErrors(errors);
        }

        public static async Task ValidateUpdateAssignmentAsync(
            Guid assignmentId,
            UpdateAssignmentRequestDto dto,
            Guid adminId,
            IAssetRepository assetRepository,
            IUserRepository userRepository,
            IAssignmentRepository assignmentRepository
        )
        {
            var errors = new List<FieldValidationException>();

            var assignment = await assignmentRepository.GetByIdAsync(assignmentId);
            if (assignment == null)
            {
                throw new KeyNotFoundException($"Assignment with id {assignmentId} not found");
            }

            // Only waiting for acceptance assignments can be updated
            if (assignment.State != AssignmentState.WaitingForAcceptance)
            {
                throw new InvalidOperationException("Can only edit assignments with state 'Waiting for acceptance'");
            }

            // Get admin and validate location
            var admin = await userRepository.GetByIdAsync(adminId);
            if (admin is null)
            {
                throw new UnauthorizedAccessException("Admin user not found");
            }

            var adminLocation = admin.Location;

            // Validate that admin is the assignor
            // Admin can only edit assignments that they created (Business rule - optional)
            // if (assignment.AssignorId != adminId)
            // {
            //     throw new UnauthorizedAccessException("You can only update assignments you created");
            // }

            // Validate new asset if provided
            if (!string.IsNullOrWhiteSpace(dto.AssetId))
            {
                if (!Guid.TryParse(dto.AssetId, out var newAssetId))
                {
                    errors.Add(new FieldValidationException("AssetId", "Invalid Asset ID format"));
                }
                else if (newAssetId != assignment.AssetId)
                {
                    await ValidateAssetForAssignmentAsync(errors, newAssetId, adminLocation, assetRepository, assignmentRepository, assignmentId);
                }
            }

            // Validate new assignee if provided
            if (!string.IsNullOrWhiteSpace(dto.AssigneeId))
            {
                if (!Guid.TryParse(dto.AssigneeId, out var newAssigneeId))
                {
                    errors.Add(new FieldValidationException("AssigneeId", "Invalid Assignee ID format"));
                }
                else if (newAssigneeId != assignment.AssigneeId)
                {
                    await ValidateAssigneeAsync(errors, newAssigneeId, adminLocation, userRepository);
                }
            }

            // Validate new assigned date if provided
            if (!string.IsNullOrWhiteSpace(dto.AssignedDate))
            {
                ValidateAssignedDate(errors, dto.AssignedDate, assignment.AssignedDate);
            }

            ThrowIfErrors(errors);
        }

        private static DateTimeOffset ValidateAssignedDate(List<FieldValidationException> errors, string dateString, DateTimeOffset? minDate = null)
        {
            if (!DateTimeOffset.TryParse(dateString, out var date))
            {
                errors.Add(new FieldValidationException("AssignedDate", "Invalid Assigned Date format"));
                return default;
            }

            if (minDate is null)
            {
                minDate = DateTimeOffset.Now.Date;
            }
            if (date.Date < minDate)
            {
                errors.Add(new FieldValidationException("AssignedDate", "AssignedDate must be either today or a day in the future"));
                return default;
            }

            return date;
        }

        private static async Task ValidateAssetForAssignmentAsync(
            List<FieldValidationException> errors,
            Guid assetId,
            Location adminLocation,
            IAssetRepository assetRepository,
            IAssignmentRepository assignmentRepository,
            Guid? excludeAssignmentId = null)
        {
            // Check if asset exists
            var asset = await assetRepository.GetByIdAsync(assetId);
            if (asset == null)
            {
                errors.Add(new FieldValidationException("AssetId", "Asset not found"));
                return;
            }

            // Check if asset is in the same location as admin
            if (asset.Location != adminLocation)
            {
                errors.Add(new FieldValidationException("AssetId", "Asset is not in your location"));
                return;
            }

            // Check if asset is available for assignment
            if (asset.State != AssetState.Available)
            {
                errors.Add(new FieldValidationException("AssetId", "Asset is not available for assignment"));
                return;
            }

            // Check if asset already has an active assignment
            var query = assignmentRepository.GetAll()
                .Where(a => a.AssetId == assetId &&
                           (a.State == AssignmentState.Accepted || a.State == AssignmentState.WaitingForReturning || a.Asset.State != AssetState.Available));

            if (excludeAssignmentId.HasValue)
            {
                query = query.Where(a => a.AssetId != excludeAssignmentId);
            }

            var existingAssignment = await query.FirstOrDefaultAsync();

            if (existingAssignment is not null)
            {
                errors.Add(new FieldValidationException("AssetId", "Asset is already assigned or accepted"));
            }
        }

        private static async Task ValidateAssigneeAsync(
            List<FieldValidationException> errors,
            Guid assigneeId,
            Location adminLocation,
            IUserRepository userRepository)
        {
            // Check if assignee exists
            var assignee = await userRepository.GetByIdAsync(assigneeId);
            if (assignee == null)
            {
                errors.Add(new FieldValidationException("AssigneeId", "Assignee not found"));
                return;
            }

            // Check if assignee is in the same location as admin
            if (assignee.Location != adminLocation)
            {
                errors.Add(new FieldValidationException("AssigneeId", "Assignee is not in your location"));
                return;
            }

            // Check if assignee is active
            if (!assignee.IsActive)
            {
                errors.Add(new FieldValidationException("AssigneeId", "Cannot assign to inactive user"));
                return;
            }
        }

        private static void AddErrorIfEmpty(List<FieldValidationException> errors, string value, string field, string message)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                errors.Add(new FieldValidationException(field, message));
            }
        }

        private static void ThrowIfErrors(List<FieldValidationException> errors)
        {
            if (errors.Any())
            {
                throw new AggregateFieldValidationException(errors);
            }
        }
    }
}