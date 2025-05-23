using AssetManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AssetManagement.Data.Configuration;

public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public static readonly List<Category> Categories = new()
    {
        new Category
        {
            Id = new Guid("123e4567-e89b-12d3-a456-426614174001"),
            Name = "Bluetooth Mouse",
            Slug = "Bluetooth Mouse",
            CreatedDate = new DateTime(2025, 5, 1, 0, 0, 0, DateTimeKind.Utc)
        },
        new Category
        {
            Id = new Guid("123e4567-e89b-12d3-a456-426614174002"),
            Name = "Headset",
            Slug = "Headset",
            CreatedDate = new DateTime(2025, 5, 1, 0, 0, 0, DateTimeKind.Utc)
        },
        new Category
        {
            Id = new Guid("123e4567-e89b-12d3-a456-426614174003"),
            Name = "Ipad",
            Slug = "Ipad",
            CreatedDate = new DateTime(2025, 5, 1, 0, 0, 0, DateTimeKind.Utc)
        },
        new Category
        {
            Id = new Guid("123e4567-e89b-12d3-a456-426614174004"),
            Name = "Iphone",
            Slug = "Iphone",
            CreatedDate = new DateTime(2025, 5, 1, 0, 0, 0, DateTimeKind.Utc)
        },
        new Category
        {
            Id = new Guid("123e4567-e89b-12d3-a456-426614174005"),
            Name = "Laptop",
            Slug = "Laptop",
            CreatedDate = new DateTime(2025, 5, 1, 0, 0, 0, DateTimeKind.Utc)
        },
        new Category
        {
            Id = new Guid("123e4567-e89b-12d3-a456-426614174006"),
            Name = "Mobile",
            Slug = "Mobile",
            CreatedDate = new DateTime(2025, 5, 1, 0, 0, 0, DateTimeKind.Utc)
        },
        new Category
        {
            Id = new Guid("123e4567-e89b-12d3-a456-426614174007"),
            Name = "Monitor",
            Slug = "Monitor",
            CreatedDate = new DateTime(2025, 5, 1, 0, 0, 0, DateTimeKind.Utc)
        },
        new Category
        {
            Id = new Guid("123e4567-e89b-12d3-a456-426614174008"),
            Name = "Personal Computer",
            Slug = "Personal Computer",
            CreatedDate = new DateTime(2025, 5, 1, 0, 0, 0, DateTimeKind.Utc)
        },
        new Category
        {
            Id = new Guid("123e4567-e89b-12d3-a456-426614174009"),
            Name = "Tablet",
            Slug = "Tablet",
            CreatedDate = new DateTime(2025, 5, 1, 0, 0, 0, DateTimeKind.Utc)
        }
    };

    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.HasKey(c => c.Id);
        builder.HasData(Categories);
    }
}