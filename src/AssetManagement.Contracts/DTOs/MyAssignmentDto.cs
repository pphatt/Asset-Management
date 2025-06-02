namespace AssetManagement.Contracts.DTOs
{
    public class MyAssignmentDto
    {
        public Guid AssignmentId { get; set; }
        public string AssetCode { get; set; } = null!;
        public string AssetName { get; set; } = null!;
        public string Category { get; set; } = null!;
        public string AssignedDate { get; set; } = null!;
        public string State { get; set; } = null!;
    }
}
