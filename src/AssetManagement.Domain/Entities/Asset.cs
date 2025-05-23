using AssetManagement.Domain.Enums;

namespace AssetManagement.Domain.Entities;
public class Asset : BaseEntity
{
    public string AssetCode { get; set; }
    public string Name { get; set; }
    public AssetStateEnum State { get; set; }
    public DateTimeOffset InstallDate { get; set; }
    public LocationEnum Location { get; set; }
    public string Specification { get; set; }
    
    public Guid CategoryId { get; set; }
    public virtual Category Category { get; set; }
}
