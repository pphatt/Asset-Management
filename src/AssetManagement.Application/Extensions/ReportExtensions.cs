using AssetManagement.Contracts.DTOs;

namespace AssetManagement.Application.Extensions
{
    public static class ReportExtensions
    {
        public static IQueryable<AssetReportDto> ApplySorting(this IQueryable<AssetReportDto> query, string property, string order)
        {
            return property switch
            {
                "category" => order == "desc"
                    ? query.OrderByDescending(r => r.Category)
                    : query.OrderBy(r => r.Category),
                "total" => order == "desc"
                    ? query.OrderByDescending(r => r.Total)
                    : query.OrderBy(r => r.Total),
                "assigned" => order == "desc"
                    ? query.OrderByDescending(r => r.Assigned)
                    : query.OrderBy(r => r.Assigned),
                "available" => order == "desc"
                    ? query.OrderByDescending(r => r.Available)
                    : query.OrderBy(r => r.Available),
                "notavailable" => order == "desc"
                    ? query.OrderByDescending(r => r.NotAvailable)
                    : query.OrderBy(r => r.NotAvailable),
                "waitingforrecycling" => order == "desc"
                    ? query.OrderByDescending(r => r.WaitingForRecycling)
                    : query.OrderBy(r => r.WaitingForRecycling),
                "recycled" => order == "desc"
                    ? query.OrderByDescending(r => r.Recycled)
                    : query.OrderBy(r => r.Recycled),
                _ => query.OrderBy(r => r.Category),
            };
        }
    }
}