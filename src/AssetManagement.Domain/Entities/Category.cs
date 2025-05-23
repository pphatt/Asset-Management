namespace AssetManagement.Domain.Entities;

public class Category : BaseEntity
{
    public string Name { get; set; }
    public string Slug { get; set; }

    public ICollection<Asset> Assets { get; set; }
}
