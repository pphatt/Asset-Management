using System.Linq.Expressions;
using AssetManagement.Domain.Entities;
using System.Linq.Expressions;

namespace AssetManagement.Domain.Repositories;

public interface IAssetRepository: IGenericRepository<Asset>
{
    Task<Asset?> GetSingleAsync(Expression<Func<Asset, bool>> predicate, CancellationToken cancellationToken,
        bool isTracking = false, params Expression<Func<Asset, object>>[]? includeProperties);
}