using System.Linq.Expressions;
using AssetManagement.Domain.Entities;
using AssetManagement.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace AssetManagement.Data.Repositories;

public class AssetRepository : GenericRepository<Asset>, IAssetRepository
{
    private readonly AssetManagementDbContext context;

    public AssetRepository(AssetManagementDbContext dbContext) : base(dbContext)
    {
        context = dbContext;
    }

    public Task<Asset?> GetByCodeAsync(string assetCode)
    {
        var asset = context.Assets
            .FirstOrDefaultAsync(a => a.Code == assetCode);
        return asset;
    }

    public async Task<Asset?> GetSingleAsync(Expression<Func<Asset, bool>> predicate,
        CancellationToken cancellationToken, bool isTracking = false, params Expression<Func<Asset, object>>[]? includeProperties)
    {
        var query = GetAll();
        query = !isTracking ? query.AsTracking() : query.AsNoTracking();

        if (includeProperties != null)
        {
            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }
        }

        return await query.SingleOrDefaultAsync(predicate, cancellationToken);
    }
}