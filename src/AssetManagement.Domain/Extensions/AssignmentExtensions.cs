using System.Linq.Expressions;
using AssetManagement.Domain.Entities;
using AssetManagement.Domain.Enums;

namespace AssetManagement.Domain.Extensions
{
    public static class AssignmentExtensions
    {
        public static IQueryable<Assignment> ApplySearch(this IQueryable<Assignment> query, string? searchTerm)
        {
            if (string.IsNullOrEmpty(searchTerm))
                return query;

            string normalizedSearchTerm = searchTerm.Trim().ToLower();

            return query.Where(u =>
                u.Asset.Code.Trim().ToLower().Contains(normalizedSearchTerm) ||
                u.Asset.Name.Trim().ToLower().Contains(normalizedSearchTerm) ||
                u.Assignee.Username.Trim().ToLower().Contains(normalizedSearchTerm));
        }

        public static IQueryable<Assignment> ApplyFilters(this IQueryable<Assignment> query, IList<AssignmentState>? states, DateTimeOffset? date, Location location)
        {
            query = query.Where(a => a.Asset.Location == location);

            if (states is not null && states.Any())
            {
                query = query.Where(a => states.Contains(a.State));
            }

            if (date.HasValue)
            {
                query = query.Where(a => Equals(a.AssignedDate.Date, date.Value.Date));
            }

            return query;
        }

        public static IQueryable<Assignment> ApplySorting(this IQueryable<Assignment> query, IList<(string property, string order)> sortingCriteria)
        {
            if (sortingCriteria == null || sortingCriteria.Count == 0)
                return query.OrderBy(a => a.AssignedDate);

            IOrderedQueryable<Assignment>? ordered = null;

            foreach (var (property, order) in sortingCriteria)
            {
                bool descending = order.Equals("desc", StringComparison.OrdinalIgnoreCase);

                // build the key selector
                Expression<Func<Assignment, object>> keySelector = property.ToLower() switch
                {
                    "assetcode" => a => a.Asset.Code,
                    "assetname" => a => a.Asset.Name,
                    "assignedto" => a => a.Assignee.Username,
                    "assignedby" => a => a.Assignor.Username,
                    "assigneddate" => a => a.AssignedDate,
                    "state" => a => a.State,
                    "no" => a => a.CreatedDate,
                    "created" => a => a.CreatedDate,
                    "updated" => a => a.LastModifiedDate,
                    _ => a => a.CreatedDate,
                };

                if (ordered == null)
                {
                    ordered = descending
                        ? query.OrderByDescending(keySelector)
                        : query.OrderBy(keySelector);
                }
                else
                {
                    ordered = descending
                        ? ordered.ThenByDescending(keySelector)
                        : ordered.ThenBy(keySelector);
                }
            }

            return ordered ?? query;
        }
    }
}