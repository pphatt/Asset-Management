using AssetManagement.Contracts.Enums;

namespace AssetManagement.Application.Validators
{
    public class ReturnRequestValidator
    {
        public static IList<ReturnRequestState> ParseStates(IList<string>? stateStrings)
        {
            IList<ReturnRequestState> states = new List<ReturnRequestState>();
            if (stateStrings is not null)
            {
                foreach (var stateString in stateStrings)
                {
                    if (!string.IsNullOrEmpty(stateString) && Enum.TryParse<ReturnRequestState>(stateString, true, out var state))
                    {
                        states.Add(state);
                    }
                }
            }

            return states;
        }

        public static DateTimeOffset? ParseDate(string? returnedDate)
        {
            if (string.IsNullOrEmpty(returnedDate))
                return null;
            if (DateTimeOffset.TryParse(returnedDate, out var date))
                return date;
            throw new ArgumentException("Invalid date format for filtering.");
        }
    }
}