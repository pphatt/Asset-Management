using System.Reflection;
using AssetManagement.Data.Data;
using AssetManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AssetManagement.Data;

public class AssetManagementDbContext : DbContext
{
    public AssetManagementDbContext(DbContextOptions<AssetManagementDbContext> options)
        : base(options)
    {
    }

    public DbSet<Asset> Assets { get; set; }

    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(
            Assembly.GetExecutingAssembly());

        #region Seeding Data

        modelBuilder.SeedUsers();

        #endregion
    }


}