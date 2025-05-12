using AssetManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace AssetManagement.Data;
public class AssetManagementDbContext : DbContext
{
    public AssetManagementDbContext(DbContextOptions<AssetManagementDbContext> options)
        : base(options)
    {
    }

    public DbSet<Asset> Assets { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(
            Assembly.GetExecutingAssembly());
    }
}
