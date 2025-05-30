namespace AssetManagement.Contracts.DTOs.Requests
{
    public class UpdateAssignmentRequestDto
    {
        public string? AssetId { get; set; }
        public string? AssigneeId { get; set; }
        public string? AssignedDate { get; set; }
        public string? Note { get; set; }
    }
}