using AssetManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Diagnostics.CodeAnalysis;

namespace AssetManagement.Data.Configuration;

[ExcludeFromCodeCoverage]
public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public static readonly List<Category> Categories =
    [
        new Category
        {
            Id = new Guid("123e4567-e89b-12d3-a456-426614174007"),
            Name = "Monitor",
            Prefix = "MO",
            CreatedDate = new DateTime(2025, 5, 1, 0, 0, 0, DateTimeKind.Utc)
        }
    ];

    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.HasKey(c => c.Id);
        //builder.HasData(Categories);
    }
}