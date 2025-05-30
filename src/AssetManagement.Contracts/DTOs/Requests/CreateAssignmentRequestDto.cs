namespace AssetManagement.Contracts.DTOs.Requests
{
    public class CreateAssignmentRequestDto
    {
        public string AssetId { get; set; } = string.Empty;
        public string AssigneeId { get; set; } = string.Empty;
        public string AssignedDate { get; set; } = string.Empty;
        public string Note { get; set; } = string.Empty;
    }
}