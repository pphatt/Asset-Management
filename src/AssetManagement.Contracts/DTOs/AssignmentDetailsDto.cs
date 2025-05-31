namespace AssetManagement.Contracts.DTOs
{
    public class AssignmentDetailsDto : AssignmentDto
    {
        public string? Specification { get; set; }
        public string? Note { get; set; }
    }
}