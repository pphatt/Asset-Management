using AssetManagement.Domain.Enums;

namespace AssetManagement.Domain.Entities;
public class Asset : BaseEntity
{
    public string Code { get; set; } = null!;

    public string Name { get; set; } = null!;
    
    public AssetState State { get; set; }
    
    public DateTimeOffset InstalledDate { get; set; }
    
    public Location Location { get; set; }

    public string Specification { get; set; } = string.Empty;

    public Guid CategoryId { get; set; }

    public Category Category { get; set; } = null!;
}
