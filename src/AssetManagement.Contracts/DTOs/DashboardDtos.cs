namespace AssetManagement.Contracts.DTOs
{
    public class DashboardStatsDto
    {
        public int TotalAssets { get; set; }
        public int AvailableAssets { get; set; }
        public int AssignedAssets { get; set; }
        public int NotAvailableAssets { get; set; }
        public int TotalUsers { get; set; }
        public int ActiveAssignments { get; set; }
        public int PendingReturns { get; set; }
        public int TotalCategories { get; set; }
    }

    public class AssetByCategoryDto
    {
        public Guid CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public int Count { get; set; }
        public string Prefix { get; set; } = string.Empty;
    }

    public class AssetByStateDto
    {
        public string? State { get; set; }
        public int Count { get; set; }
    }

    public class AssetByLocationDto
    {
        public string? Location { get; set; }
        public int Count { get; set; }
    }

    public class MonthlyAssignmentStatsDto
    {
        public string Month { get; set; } = string.Empty;
        public int Year { get; set; }
        public int Assignments { get; set; }
        public int Returns { get; set; }
    }

    public class RecentActivityDto
    {
        public Guid Id { get; set; }
        public string Type { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
        public Guid UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public Guid? AssetId { get; set; }
        public string? AssetCode { get; set; }
    }

    public class DashboardFilters
    {
        public string TimeRange { get; set; } = "30d"; // 7d, 30d, 90d, 1y
        public string? Location { get; set; }
    }
}