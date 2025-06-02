namespace AssetManagement.Contracts.DTOs
{
    public class AssignmentDetailsDto : AssignmentDto
    {
        public string? Specification { get; set; }
        public string? Note { get; set; }
        public Guid? AssignedToId { get; set; }
        public Guid? AssignedById { get; set; }
        public Guid? AssetId { get; set; }
    }
}