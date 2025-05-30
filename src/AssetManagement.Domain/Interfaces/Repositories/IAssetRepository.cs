using System.Linq.Expressions;
using AssetManagement.Domain.Entities;

namespace AssetManagement.Domain.Interfaces.Repositories
{
    public interface IAssetRepository : IGenericRepository<Asset>
    {
        Task<Asset?> GetSingleAsync(Expression<Func<Asset, bool>> predicate, CancellationToken cancellationToken,
            bool isTracking = false, params Expression<Func<Asset, object>>[]? includeProperties);
        Task<Asset?> GetByCodeAsync(string assetCode);
    }
}
