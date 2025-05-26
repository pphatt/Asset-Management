namespace AssetManagement.Domain.Entities;

public class Category : BaseEntity
{
    public string Name { get; set; } = null!;
    public string Prefix { get; set; } = null!;
    public ICollection<Asset> Assets { get; set; } = new List<Asset>();
}
