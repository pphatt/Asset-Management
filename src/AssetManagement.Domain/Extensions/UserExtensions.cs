using System.Linq.Expressions;
using AssetManagement.Domain.Entities;
using AssetManagement.Domain.Enums;

namespace AssetManagement.Domain.Extensions
{
    public static class UserExtensions
    {
        public static IQueryable<User> ApplySearch(this IQueryable<User> query, string? searchTerm)
        {
            if (string.IsNullOrEmpty(searchTerm))
                return query;

            string normalizedSearchTerm = searchTerm.Trim().ToLower();

            return query.Where(u =>
                (u.FirstName + " " + u.LastName).Trim().ToLower().Contains(normalizedSearchTerm) ||
                (u.LastName + " " + u.FirstName).Trim().ToLower().Contains(normalizedSearchTerm) ||
                u.StaffCode.Contains(normalizedSearchTerm));
        }

        public static IQueryable<User> ApplyFilters(this IQueryable<User> query, string? userType, Location userLocation)
        {
            // Apply location filter - admin can only see users from their own location
            query = query.Where(u => u.Location == userLocation);

            if (string.IsNullOrEmpty(userType))
                return query;

            query = userType.ToLower() switch
            {
                "all" => query,
                "staff" => query.Where(u => u.Type == UserType.Staff),
                "admin" => query.Where(u => u.Type == UserType.Admin),
                _ => query,
            };

            return query;
        }

        public static IQueryable<User> ApplySorting(
            this IQueryable<User> query,
            IList<(string property, string order)> sortingCriteria)
        {
            if (sortingCriteria == null || sortingCriteria.Count == 0)
                return query;

            IOrderedQueryable<User>? ordered = null;

            foreach (var (property, order) in sortingCriteria)
            {
                bool descending = order.Equals("desc", StringComparison.OrdinalIgnoreCase);

                // build the key selector
                Expression<Func<User, object>> keySelector = property.ToLower() switch
                {
                    "name" => u => u.FirstName + " " + u.LastName,
                    "code" => u => u.StaffCode,
                    "username" => u => u.Username,
                    "joined" => u => u.JoinedDate,
                    "type" => u => u.Type,
                    "created" => u => u.CreatedDate,
                    "updated" => u => u.LastModifiedDate,
                    _ => u => u.Id,
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
