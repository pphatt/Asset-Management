namespace AssetManagement.Contracts.Parameters
{
    public class MyAssignmentQueryParameters : PaginationParameters
    {
        public string? SortBy { get; set; }
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
