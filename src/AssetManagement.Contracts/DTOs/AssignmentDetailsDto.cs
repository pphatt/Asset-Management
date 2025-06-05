namespace AssetManagement.Contracts.DTOs
{
    public class AssignmentDetailsDto : AssignmentDto
    {
        public string? Specification { get; set; }
        public string? Note { get; set; }
        public Guid? AssigneeId { get; set; }
        public string? AssigneeStaffCode { get; set; }
        public Guid? AssignorId { get; set; }
        public Guid? AssetId { get; set; }
    }
}