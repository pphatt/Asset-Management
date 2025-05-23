using AssetManagement.Domain.Entities;
using AssetManagement.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AssetManagement.Data.Configuration;

public class AssetConfiguration : IEntityTypeConfiguration<Asset>
{
    public void Configure(EntityTypeBuilder<Asset> builder)
    {
        var categoryIds = CategoryConfiguration.Categories.Select(c => c.Id).ToList();

        var assets = new List<Asset>
        {
            // Laptops (CategoryId = categoryIds[4])
            new Asset
            {
                Id = new Guid("223e4567-e89b-12d3-a456-426614174001"),
                AssetCode = "LAP-001",
                Name = "Dell XPS 13",
                CategoryId = categoryIds[4], // Laptop
                State = AssetStateEnum.Available,
                InstallDate = new DateTimeOffset(2023, 5, 21, 0, 0, 0, TimeSpan.Zero),
                Location = LocationEnum.HCM,
                Specification = "Intel i7, 16GB RAM, 512GB SSD",
                CreatedDate = new DateTime(2025, 5, 21, 0, 0, 0, DateTimeKind.Utc)
            },
            new Asset
            {
                Id = new Guid("223e4567-e89b-12d3-a456-426614174002"),
                AssetCode = "LAP-002",
                Name = "MacBook Pro 14",
                CategoryId = categoryIds[4], // Laptop
                State = AssetStateEnum.Available,
                InstallDate = new DateTimeOffset(2024, 5, 21, 0, 0, 0, TimeSpan.Zero),
                Location = LocationEnum.DN,
                Specification = "M1 Pro, 32GB RAM, 1TB SSD",
                CreatedDate = new DateTime(2025, 5, 21, 0, 0, 0, DateTimeKind.Utc)
            },

            // Monitors (CategoryId = categoryIds[6])
            new Asset
            {
                Id = new Guid("223e4567-e89b-12d3-a456-426614174003"),
                AssetCode = "MON-001",
                Name = "Dell UltraSharp 27",
                CategoryId = categoryIds[6], // Monitor
                State = AssetStateEnum.Available,
                InstallDate = new DateTimeOffset(2022, 5, 21, 0, 0, 0, TimeSpan.Zero),
                Location = LocationEnum.HCM,
                Specification = "27-inch, 4K, IPS Panel",
                CreatedDate = new DateTime(2025, 5, 21, 0, 0, 0, DateTimeKind.Utc)
            },
            new Asset
            {
                Id = new Guid("223e4567-e89b-12d3-a456-426614174004"),
                AssetCode = "MON-002",
                Name = "LG 32GN600",
                CategoryId = categoryIds[6], // Monitor
                State = AssetStateEnum.Available,
                InstallDate = new DateTimeOffset(2023, 5, 21, 0, 0, 0, TimeSpan.Zero),
                Location = LocationEnum.HCM,
                Specification = "32-inch, QHD, 144Hz",
                CreatedDate = new DateTime(2025, 5, 21, 0, 0, 0, DateTimeKind.Utc)
            },

            // Ipads (CategoryId = categoryIds[2])
            new Asset
            {
                Id = new Guid("223e4567-e89b-12d3-a456-426614174005"),
                AssetCode = "IPD-001",
                Name = "iPad Pro 12.9",
                CategoryId = categoryIds[2], // Ipad
                State = AssetStateEnum.Available,
                InstallDate = new DateTimeOffset(2023, 11, 21, 0, 0, 0, TimeSpan.Zero),
                Location = LocationEnum.DN,
                Specification = "M2 Chip, 256GB, Wi-Fi",
                CreatedDate = new DateTime(2025, 5, 21, 0, 0, 0, DateTimeKind.Utc)
            },
            new Asset
            {
                Id = new Guid("223e4567-e89b-12d3-a456-426614174006"),
                AssetCode = "IPD-002",
                Name = "iPad Air 5",
                CategoryId = categoryIds[2], // Ipad
                State = AssetStateEnum.Available,
                InstallDate = new DateTimeOffset(2024, 11, 21, 0, 0, 0, TimeSpan.Zero),
                Location = LocationEnum.HN,
                Specification = "M1 Chip, 64GB, Wi-Fi",
                CreatedDate = new DateTime(2025, 5, 21, 0, 0, 0, DateTimeKind.Utc)
            },

            // Iphones (CategoryId = categoryIds[3])
            new Asset
            {
                Id = new Guid("223e4567-e89b-12d3-a456-426614174007"),
                AssetCode = "IPH-001",
                Name = "iPhone 14 Pro",
                CategoryId = categoryIds[3], // Iphone
                State = AssetStateEnum.Available,
                InstallDate = new DateTimeOffset(2024, 5, 21, 0, 0, 0, TimeSpan.Zero),
                Location = LocationEnum.HCM,
                Specification = "256GB, Black, 5G",
                CreatedDate = new DateTime(2025, 5, 21, 0, 0, 0, DateTimeKind.Utc)
            },

            // Bluetooth Mouse (CategoryId = categoryIds[0])
            new Asset
            {
                Id = new Guid("223e4567-e89b-12d3-a456-426614174008"),
                AssetCode = "MOU-001",
                Name = "Logitech MX Master 3",
                CategoryId = categoryIds[0], // Bluetooth Mouse
                State = AssetStateEnum.Available,
                InstallDate = new DateTimeOffset(2024, 5, 21, 0, 0, 0, TimeSpan.Zero),
                Location = LocationEnum.DN,
                Specification = "Wireless, 4000 DPI, Ergonomic",
                CreatedDate = new DateTime(2025, 5, 21, 0, 0, 0, DateTimeKind.Utc)
            },

            // Headset (CategoryId = categoryIds[1])
            new Asset
            {
                Id = new Guid("223e4567-e89b-12d3-a456-426614174009"),
                AssetCode = "HST-001",
                Name = "Sony WH-1000XM5",
                CategoryId = categoryIds[1], // Headset
                State = AssetStateEnum.Available,
                InstallDate = new DateTimeOffset(2024, 9, 21, 0, 0, 0, TimeSpan.Zero),
                Location = LocationEnum.HCM,
                Specification = "Wireless, Noise-Cancelling, 30hr Battery",
                CreatedDate = new DateTime(2025, 5, 21, 0, 0, 0, DateTimeKind.Utc)
            },

            // Personal Computer (CategoryId = categoryIds[7])
            new Asset
            {
                Id = new Guid("223e4567-e89b-12d3-a456-426614174010"),
                AssetCode = "PC-001",
                Name = "HP EliteDesk 800",
                CategoryId = categoryIds[7], // Personal Computer
                State = AssetStateEnum.Available,
                InstallDate = new DateTimeOffset(2020, 5, 21, 0, 0, 0, TimeSpan.Zero),
                Location = LocationEnum.HN,
                Specification = "Intel i5, 8GB RAM, 1TB HDD",
                CreatedDate = new DateTime(2025, 5, 21, 0, 0, 0, DateTimeKind.Utc)
            },

            // Tablet (CategoryId = categoryIds[8])
            new Asset
            {
                Id = new Guid("223e4567-e89b-12d3-a456-426614174011"),
                AssetCode = "TAB-001",
                Name = "Samsung Galaxy Tab S8",
                CategoryId = categoryIds[8], // Tablet
                State = AssetStateEnum.Available,
                InstallDate = new DateTimeOffset(2024, 7, 21, 0, 0, 0, TimeSpan.Zero),
                Location = LocationEnum.DN,
                Specification = "Snapdragon 8, 128GB, 11-inch",
                CreatedDate = new DateTime(2025, 5, 21, 0, 0, 0, DateTimeKind.Utc)
            },

            // Mobile (CategoryId = categoryIds[5])
            new Asset
            {
                Id = new Guid("223e4567-e89b-12d3-a456-426614174012"),
                AssetCode = "MOB-001",
                Name = "Samsung Galaxy S23",
                CategoryId = categoryIds[5], // Mobile
                State = AssetStateEnum.Available,
                InstallDate = new DateTimeOffset(2025, 1, 21, 0, 0, 0, TimeSpan.Zero),
                Location = LocationEnum.HN,
                Specification = "256GB, 5G, Snapdragon 8 Gen 2",
                CreatedDate = new DateTime(2025, 5, 21, 0, 0, 0, DateTimeKind.Utc)
            }
        };

        builder.HasData(assets);

        // Define relationships and constraints
        builder.HasOne(a => a.Category)
               .WithMany()
               .HasForeignKey(a => a.CategoryId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}