namespace AssetManagement.Contracts.DTOs
{
    public class AssignmentDto
    {
        public Guid Id { get; set; }
        public int No { get; set; }
        public string? AssetCode { get; set; }
        public string? AssetName { get; set; }
        public string? AssignedTo { get; set; }
        public string? AssignedBy { get; set; }
        public string? AssignedDate { get; set; } // Format: dd/MM/yyyy
        public string? State { get; set; }
    }
}