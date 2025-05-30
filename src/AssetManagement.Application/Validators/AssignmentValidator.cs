using AssetManagement.Domain.Enums;

namespace AssetManagement.Application.Validators
{
    public static class AssignmentValidator
    {
        public static AssignmentState? ParseState(string? stateString)
        {
            if (string.IsNullOrEmpty(stateString))
                return null;
            if (Enum.TryParse<AssignmentState>(stateString, true, out var state))
                return state;
            throw new ArgumentException("Invalid state for filtering. Valid values are 'Accepted' or 'WaitingForAcceptance'.");
        }

        public static DateTimeOffset? ParseDate(string? dateString)
        {
            if (string.IsNullOrEmpty(dateString))
                return null;
            if (DateTimeOffset.TryParse(dateString, out var date))
                return date;
            throw new ArgumentException("Invalid date format for filtering.");
        }
    }
}