using AssetManagement.Contracts.DTOs;

namespace AssetManagement.Application.Services.Interfaces
{
    public interface IDashboardService
    {
        Task<DashboardStatsDto> GetDashboardStatsAsync(DashboardFilters filters);
        Task<List<AssetByCategoryDto>> GetAssetsByCategoryAsync();
        Task<List<AssetByStateDto>> GetAssetsByStateAsync(DashboardFilters filters);
        Task<List<AssetByLocationDto>> GetAssetsByLocationAsync(DashboardFilters filters);
        Task<List<MonthlyAssignmentStatsDto>> GetMonthlyStatsAsync(DashboardFilters filters);
        Task<List<RecentActivityDto>> GetRecentActivityAsync(DashboardFilters filters, int limit = 10);
    }
}