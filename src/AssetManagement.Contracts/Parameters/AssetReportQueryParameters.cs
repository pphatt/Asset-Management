namespace AssetManagement.Contracts.Parameters
{
    public class AssetReportQueryParameters
    {
        public string? SortBy { get; set; } = "category";
        public string? SortOrder { get; set; } = "asc";

        public (string Property, string Order) GetSortCriteria()
        {
            return (SortBy?.ToLower() ?? "category", SortOrder?.ToLower() ?? "asc");
        }
    }
}