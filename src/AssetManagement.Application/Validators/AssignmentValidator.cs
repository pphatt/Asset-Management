using AssetManagement.Contracts.DTOs.Requests;
using AssetManagement.Contracts.Exceptions;
using AssetManagement.Domain.Enums;
using AssetManagement.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

/*
 * 2 main approaches
 *
 * We chose approach 2, the code which implements approach 1 is now turned into comments.
 *
 * 1. Allow multiple pendings per asset (first come, first serve)
 * * * You can create N pending assignments for the same asset against N different staff
 * * * The asset remains “Available” until one of those N staff actually Accepts.
 *
 * 2. Only 1 pending per asset at a time (process oriented)
 * * * You only allow one "WaitingForAcceptance" record on an asset
 * * * Until that invite is accepted or declined, nobody else—even the admin—can issue a new assignment on that asset.
 * * * Once it’s accepted/declined, the admin can issue a fresh assignment.
 */

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
                    if (stateString == "All")
                    {
                        return [];
                    }
                    else if (!string.IsNullOrEmpty(stateString) && Enum.TryParse<AssignmentState>(stateString, true, out var state))
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
            {
                errors.Add(new FieldValidationException("AssetId", "Invalid Asset ID format"));
            }

            if (!Guid.TryParse(dto.AssigneeId, out var assigneeId))
            {
                errors.Add(new FieldValidationException("AssigneeId", "Invalid Assignee ID format"));
            }

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
            // Support approach 1
            // await ValidateNoDuplicatePendingAsync(errors, assetId, assigneeId, assignmentRepository);
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
            if (assignment == null || assignment.IsDeleted == true)
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
                if (!Guid.TryParse(dto.AssetId, out var parsedAssetId))
                {
                    errors.Add(new FieldValidationException("AssetId", "Invalid Asset ID format"));
                }
                else if (parsedAssetId != assignment.AssetId)
                {
                    await ValidateAssetForAssignmentAsync(errors, parsedAssetId, adminLocation, assetRepository, assignmentRepository);
                }
            }

            // Validate new assignee if provided
            if (!string.IsNullOrWhiteSpace(dto.AssigneeId))
            {
                if (!Guid.TryParse(dto.AssigneeId, out var parsedAssigneeId))
                {
                    errors.Add(new FieldValidationException("AssigneeId", "Invalid Assignee ID format"));
                }
                else if (parsedAssigneeId != assignment.AssigneeId)
                {
                    await ValidateAssigneeAsync(errors, parsedAssigneeId, adminLocation, userRepository);
                }
            }

            // Support approach 1
            // if (newAssetId != assignment.AssetId ||
            //     newAssigneeId != assignment.AssigneeId)
            // {
            //     await ValidateNoDuplicatePendingAsync(
            //         errors,
            //         newAssetId,
            //         newAssigneeId,
            //         assignmentRepository
            //     );
            // }

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
            IAssignmentRepository assignmentRepository)
        {
            // Check if asset exists
            var asset = await assetRepository.GetByIdAsync(assetId);
            if (asset == null || asset.IsDeleted == true)
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
            var hasActiveAssignments = await assignmentRepository.GetAll()
                .AnyAsync(a => a.AssetId == assetId && a.IsDeleted != true &&
                   (a.State == AssignmentState.Accepted || a.State == AssignmentState.WaitingForReturning));

            if (hasActiveAssignments)
            {
                errors.Add(new FieldValidationException("AssetId", "This asset already has active assignments"));
            }

            // Support approach 2
            var hasPendings = await assignmentRepository.GetAll()
                .AnyAsync(a => a.AssetId == assetId && a.IsDeleted != true &&
                    a.State == AssignmentState.WaitingForAcceptance);

            if (hasPendings)
            {
                errors.Add(new FieldValidationException("AssetId", "Another assignment is already pending for this asset"));
            }

            ThrowIfErrors(errors);
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

            ThrowIfErrors(errors);
        }

        // Support approach 1
        // private static async Task ValidateNoDuplicatePendingAsync(
        //     List<FieldValidationException> errors,
        //     Guid assetId,
        //     Guid assigneeId,
        //     IAssignmentRepository assignmentRepository)
        // {
        //     // Check if this assignee already has a pending assignment of the same asset
        //     var existingPendingAssignment = await assignmentRepository.GetAll()
        //         .Where(a => a.AssigneeId == assigneeId
        //             && a.AssetId == assetId
        //             && a.State == AssignmentState.WaitingForAcceptance)
        //         .FirstOrDefaultAsync();

        //     if (existingPendingAssignment != null)
        //     {
        //         errors.Add(new FieldValidationException("AssigneeId", "User already has a pending assignment for this asset"));
        //     }

        //     ThrowIfErrors(errors);
        // }

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