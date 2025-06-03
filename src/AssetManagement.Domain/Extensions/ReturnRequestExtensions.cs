using System.Linq.Expressions;
using AssetManagement.Contracts.Enums;
using AssetManagement.Domain.Entities;
using AssetManagement.Domain.Enums;

namespace AssetManagement.Domain.Extensions
{
    public static class ReturnRequestExtensions
    {
        public static IQueryable<ReturnRequest> ApplySearch(this IQueryable<ReturnRequest> query, string? searchTerm)
        {
            if (string.IsNullOrEmpty(searchTerm))
                return query;

            string normalizedSearchTerm = searchTerm.Trim().ToLower();

            return query.Where(rr =>
                rr.Assignment.Asset.Code.Trim().ToLower().Contains(normalizedSearchTerm) ||
                rr.Assignment.Asset.Name.Trim().ToLower().Contains(normalizedSearchTerm) ||
                rr.Requester.Username.Trim().ToLower().Contains(normalizedSearchTerm));
        }

        public static IQueryable<ReturnRequest> ApplyFilters(this IQueryable<ReturnRequest> query, IList<ReturnRequestState>? states, DateTimeOffset? date, Location location)
        {
            query = query.Where(rr => rr.Assignment.Asset.Location == location);

            if (states is not null && states.Any())
            {
                // TODO: add filtering logic here
                query = query.Where(rr => states.Contains(rr.State));
            }

            if (date.HasValue)
            {
                query = query.Where(rr => Equals(rr.ReturnedDate, date));
            }

            return query;
        }

        public static IQueryable<ReturnRequest> ApplySorting(this IQueryable<ReturnRequest> query, IList<(string property, string order)> sortingCriteria)
        {
            if (sortingCriteria == null || sortingCriteria.Count == 0)
                return query.OrderBy(rr => rr.ReturnedDate);

            IOrderedQueryable<ReturnRequest>? ordered = null;

            foreach (var (property, order) in sortingCriteria)
            {
                bool descending = order.Equals("desc", StringComparison.OrdinalIgnoreCase);

                // build the key selector
                Expression<Func<ReturnRequest, object>> keySelector = property.ToLower() switch
                {
                    "assetcode" => rr => rr.Assignment.Asset.Code,
                    "assetname" => rr => rr.Assignment.Asset.Name,
                    "requestedby" => rr => rr.Requester.Username,
                    "assigneddate" => rr => rr.Assignment.AssignedDate,
                    "acceptedby" => rr => rr.Acceptor.Username,
                    "returneddate" => rr => rr.ReturnedDate,
                    "state" => rr => rr.State,
                    "created" => rr => rr.CreatedDate,
                    "updated" => rr => rr.LastModifiedDate,
                    "no" => rr => rr.CreatedDate,
                    _ => rr => rr.CreatedDate,
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