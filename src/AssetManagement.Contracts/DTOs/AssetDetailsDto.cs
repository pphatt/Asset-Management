namespace AssetManagement.Contracts.DTOs;

public class AssetDetailsDto : AssetDto
{
    public DateTimeOffset InstalledDate { get; set; }
    public string? Location { get; set; }
    public string? Specification { get; set; }
    public Guid CategoryId { get; set; }
    public ICollection<AssignmentHistoryDto> Assignments { get; set; } = new List<AssignmentHistoryDto>();
}
