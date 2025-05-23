using System.Reflection;
using AssetManagement.Domain.Entities;
using AssetManagement.Data.Extensions;
using Microsoft.EntityFrameworkCore;

namespace AssetManagement.Data;

public class AssetManagementDbContext : DbContext
{
    public AssetManagementDbContext(DbContextOptions<AssetManagementDbContext> options) : base(options) { }

    public DbSet<Asset> Assets { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Category> Categories { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(
            Assembly.GetExecutingAssembly());

        #region Seeding Data

        // modelBuilder.SeedUsers();

        #endregion
    }
}