namespace AssetManagement.Contracts.Parameters;

public class AssetQueryParameters: PaginationParameters
{
    // <summary>
    // The search term to filter entities
    // </summary>
    public string? SearchTerm { get; set; }

    // <summary>
    // The sorting criteria as a comma-separated string (e.g., "name:asc,code:desc")
    // </summary>
    public string? SortBy { get; set; }

    // <summary>
    // The asset state for filtering entities
    // </summary>
    public List<string>? AssetState { get; set; }

    // <summary>
    // The asset category for filtering entities
    // </summary>
    public List<string>? AssetCategory { get; set; }

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