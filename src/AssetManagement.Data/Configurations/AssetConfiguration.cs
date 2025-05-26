using AssetManagement.Domain.Entities;
using AssetManagement.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Diagnostics.CodeAnalysis;

namespace AssetManagement.Data.Configuration;

[ExcludeFromCodeCoverage]
public class AssetConfiguration : IEntityTypeConfiguration<Asset>
{
    public void Configure(EntityTypeBuilder<Asset> builder)
    {
        var categoryIds = CategoryConfiguration.Categories.Select(c => c.Id).ToList();

        var assets = new List<Asset>
        {
            new() 
            {
                Id = new Guid("223e4567-e89b-12d3-a456-426614174003"),
                Code = "MON-001",
                Name = "Dell UltraSharp 27",
                CategoryId = new Guid("123e4567-e89b-12d3-a456-426614174007"), // Monitor
                State = AssetState.Available,
                InstalledDate = new DateTimeOffset(2022, 5, 21, 0, 0, 0, TimeSpan.Zero),
                Location = Location.HCM,
                Specification = "27-inch, 4K, IPS Panel",
                CreatedDate = new DateTime(2025, 5, 21, 0, 0, 0, DateTimeKind.Utc)
            },
            new() 
            {
                Id = new Guid("223e4567-e89b-12d3-a456-426614174004"),
                Code = "MON-002",
                Name = "LG 32GN600",
                CategoryId = new Guid("123e4567-e89b-12d3-a456-426614174007"), // Monitor
                State = AssetState.Available,
                InstalledDate = new DateTimeOffset(2023, 5, 21, 0, 0, 0, TimeSpan.Zero),
                Location = Location.HCM,
                Specification = "32-inch, QHD, 144Hz",
                CreatedDate = new DateTime(2025, 5, 21, 0, 0, 0, DateTimeKind.Utc)
            },
        };

        builder.HasData(assets);

        // Define relationships and constraints
        builder.HasOne(a => a.Category)
               .WithMany(c => c.Assets)
               .HasForeignKey(a => a.CategoryId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}