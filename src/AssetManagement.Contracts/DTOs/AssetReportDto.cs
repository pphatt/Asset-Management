namespace AssetManagement.Contracts.DTOs
{
    public class AssetReportDto
    {
        public string Category { get; set; } = null!;
        public int Total { get; set; }
        public int Assigned { get; set; }
        public int Available { get; set; }
        public int NotAvailable { get; set; }
        public int WaitingForRecycling { get; set; }
        public int Recycled { get; set; }
    }
}