using AssetManagement.Contracts.Enums;

namespace AssetManagement.Contracts.DTOs.Requests;

public class CreateAssetRequestDto
{
    public string? Name { get; set; }
    public Guid CategoryId { get; set; }
    public string? Specifications { get; set; }
    public string? InstalledDate { get; set; }
    public AssetStateDto State { get; set; }
}   