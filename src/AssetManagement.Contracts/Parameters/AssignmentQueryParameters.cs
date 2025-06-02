namespace AssetManagement.Contracts.Parameters
{
    public class AssignmentQueryParameters : PaginationParameters
    {
        // <summary>
        // The search term
        // </summary>
        public string? SearchTerm { get; set; }

        // <summary>
        // The sorting criteria as a comma-separated string (e.g., "name:asc,code:desc")
        // </summary>
        public string? SortBy { get; set; }

        // <summary>
        // The assignment state for filtering (2 options: Accepted, Waiting for acceptance)
        // </summary>
        public string? State { get; set; }

        // <summay>
        // The assigned date for filtering
        // Valid format: MM/dd/yyyy or yyyy/MM/dd
        // </summary>
        public string? Date { get; set; }

        // <summary>
        // Parses the SortBy string into a list of (property, order) tuples
        // </summary>
        // <returns>
        // A list of tuples containing the property name and sort order.
        // </returns>
        public List<(string property, string order)> GetSortCriteria()
        {
            if (string.IsNullOrEmpty(SortBy))
                return new List<(string, string)>();

            return SortBy.Split(',')
                .Select(s =>
                {
                    var parts = s.Split(':');
                    var property = parts[0].Trim();
                    var order = parts.Length > 1 ? parts[1].Trim().ToLower() : "asc";
                    return (property, order);
                })
                .ToList();
        }
    }
}