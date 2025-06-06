using AssetManagement.Data.Extensions;
using AssetManagement.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace AssetManagement.Data;

[ExcludeFromCodeCoverage]
public class AssetManagementDbContext : DbContext
{
    public AssetManagementDbContext(DbContextOptions<AssetManagementDbContext> options) : base(options) { }

    public DbSet<Asset> Assets { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Assignment> Assignments { get; set; }
    public DbSet<ReturnRequest> ReturnRequests { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);


        // Add this line to bypass the error
        // The model for context 'AssetManagementDbContext' has pending changes ...
        optionsBuilder.ConfigureWarnings(w => w.Log(RelationalEventId.PendingModelChangesWarning));
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(
            Assembly.GetExecutingAssembly());

        modelBuilder.Entity<Assignment>()
            .HasOne(a => a.Assignee)
            .WithMany(u => u.Assignments)
            .HasForeignKey(a => a.AssigneeId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<ReturnRequest>()
            .HasOne(rr => rr.Acceptor)
            .WithMany()
            .HasForeignKey(rr => rr.AcceptorId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<ReturnRequest>()
            .HasOne(rr => rr.Requester)
            .WithMany()
            .HasForeignKey(rr => rr.RequesterId)
            .OnDelete(DeleteBehavior.Restrict);

        #region Seeding Data

        modelBuilder.SeedUsers();

        #endregion
    }
}