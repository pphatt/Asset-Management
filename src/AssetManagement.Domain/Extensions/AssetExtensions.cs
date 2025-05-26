using AssetManagement.Domain.Entities;
using AssetManagement.Domain.Enums;

namespace AssetManagement.Domain.Extensions;

public static class AssetExtensions
{
    public static IQueryable<Asset> ApplyAssetSearch(this IQueryable<Asset> query, string? searchTerm)
    {
        if (string.IsNullOrEmpty(searchTerm))
            return query;

        string normalizedSearchTerm = searchTerm.Trim().ToLower();

        return query.Where(u =>
            u.Code.Trim().ToLower().Contains(normalizedSearchTerm) ||
            u.Name.Trim().ToLower().Contains(normalizedSearchTerm));
    }

    public static IQueryable<Asset> ApplyAssetFilters(this IQueryable<Asset> query, List<string>? assetStates, List<string>? assetCategories)
    {
        if (assetStates != null && assetStates.Count > 0 && !assetStates.Contains("All"))
        {
            query = query.Where(asset => assetStates.Contains(asset.State ==
                AssetState.Assigned ? "Assigned"
                : asset.State == AssetState.Available ? "Available"
                : asset.State == AssetState.NotAvailable ? "NotAvailable"
                : asset.State == AssetState.WaitingForRecycling ? "WaitingForRecycling"
                : asset.State == AssetState.Recycled ? "Recycled" : ""));
        }

        if (assetCategories != null && assetCategories.Count > 0)
        {
            query = query.Where(a => assetCategories.Contains(a.Category.Name.ToLower()));
        }

        return query;
    }

    public static IQueryable<Asset> ApplyAssetSorting(this IQueryable<Asset> query, IList<(string Property, string Order)> sortingCriteria)
    {
        if (sortingCriteria == null || !sortingCriteria.Any())
            return query;

        IOrderedQueryable<Asset>? orderedQuery = null;

        foreach (var (property, order) in sortingCriteria)
        {
            if (orderedQuery == null)
            {
                orderedQuery = property.ToLower() switch
                {
                    "name" => order == "desc" ? query.OrderByDescending(a => a.Name)
                        : query.OrderBy(a => a.Name),
                    "code" => order == "desc" ? query.OrderByDescending(a => a.Code)
                        : query.OrderBy(a => a.Code),
                    "state" => order == "desc" ? query.OrderByDescending(a => a.State)
                        : query.OrderBy(a => a.State),
                    "category" => order == "desc" ? query.OrderByDescending(a => a.Category)
                        : query.OrderBy(a => a.Category),
                    _ => query.OrderBy(a => a.Id),
                };
            }
            else
            {
                orderedQuery = property.ToLower() switch
                {
                    "name" => order == "desc" ? orderedQuery.ThenByDescending(a => a.Name)
                        : orderedQuery.ThenBy(a => a.Name),
                    "code" => order == "desc" ? orderedQuery.ThenByDescending(a => a.Code)
                        : orderedQuery.ThenBy(a => a.Code),
                    "state" => order == "desc" ? orderedQuery.ThenByDescending(a => a.State)
                        : orderedQuery.ThenBy(a => a.State),
                    "category" => order == "desc" ? orderedQuery.ThenByDescending(a => a.Category)
                        : orderedQuery.ThenBy(a => a.Category),
                    _ => orderedQuery.ThenBy(a => a.Id),
                };
            }
        }

        return orderedQuery ?? query;
    }
}
