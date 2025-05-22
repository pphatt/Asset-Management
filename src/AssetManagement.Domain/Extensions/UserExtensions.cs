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
                u.FirstName.Trim().ToLower().Contains(normalizedSearchTerm) ||
                u.LastName.Trim().ToLower().Contains(normalizedSearchTerm) ||
                u.StaffCode.Contains(normalizedSearchTerm));
        }

        public static IQueryable<User> ApplyFilters(this IQueryable<User> query, string? userType, LocationEnum userLocation)
        {
            // Apply location filter - admin can only see users from their own location
            query = query.Where(u => u.Location == userLocation);

            if (string.IsNullOrEmpty(userType))
                return query;

            query = userType.ToLower() switch
            {
                "all" => query,
                "staff" => query.Where(u => u.Type == UserTypeEnum.Staff),
                "admin" => query.Where(u => u.Type == UserTypeEnum.Admin),
                _ => query,
            };

            return query;
        }

        public static IQueryable<User> ApplySorting(this IQueryable<User> query, IList<(string, string)> sortingCriteria)
        {
            if (sortingCriteria.Count > 0)
            {
                IOrderedQueryable<User>? orderedQuery = null;
                foreach (var (property, order) in sortingCriteria)
                {
                    if (orderedQuery == null)
                    {
                        orderedQuery = property.ToLower() switch
                        {
                            "name" => order == "desc" ? query.OrderByDescending(u => u.FirstName).ThenByDescending(u => u.LastName)
                                : query.OrderBy(u => u.FirstName).ThenBy(u => u.LastName),
                            "code" => order == "desc" ? query.OrderByDescending(u => u.StaffCode) : query.OrderBy(u => u.StaffCode),
                            "username" => order == "desc" ? query.OrderByDescending(u => u.Username) : query.OrderBy(u => u.Username),
                            "joined" => order == "desc" ? query.OrderByDescending(u => u.JoinedDate) : query.OrderBy(u => u.JoinedDate),
                            "type" => order == "desc" ? query.OrderByDescending(u => u.Type) : query.OrderBy(u => u.Type),
                            "created" => order == "desc" ? query.OrderByDescending(u => u.CreatedDate) : query.OrderBy(u => u.CreatedDate),
                            "updated" => order == "desc" ? query.OrderByDescending(u => u.LastModifiedDate) : query.OrderBy(u => u.LastModifiedDate),
                            _ => query.OrderBy(u => u.Id),
                        };
                    }
                    else
                    {
                        orderedQuery = property.ToLower() switch
                        {
                            "name" => order == "desc" ? orderedQuery.ThenByDescending(u => u.FirstName).ThenByDescending(u => u.LastName)
                                : orderedQuery.ThenBy(u => u.FirstName).ThenBy(u => u.LastName),
                            "code" => order == "desc" ? orderedQuery.ThenByDescending(u => u.StaffCode) : orderedQuery.ThenBy(u => u.StaffCode),
                            "username" => order == "desc" ? orderedQuery.ThenByDescending(u => u.Username) : orderedQuery.ThenBy(u => u.Username),
                            "joined" => order == "desc" ? orderedQuery.ThenByDescending(u => u.JoinedDate) : orderedQuery.ThenBy(u => u.JoinedDate),
                            "type" => order == "desc" ? orderedQuery.ThenByDescending(u => u.Type) : orderedQuery.ThenBy(u => u.Type),
                            "created" => order == "desc" ? orderedQuery.ThenByDescending(u => u.CreatedDate) : orderedQuery.ThenBy(u => u.CreatedDate),
                            "updated" => order == "desc" ? orderedQuery.ThenByDescending(u => u.LastModifiedDate) : orderedQuery.ThenBy(u => u.LastModifiedDate),
                            _ => orderedQuery.ThenBy(u => u.Id),
                        };
                    }
                }
                query = orderedQuery ?? query;
            }

            return query;
        }
    }
}