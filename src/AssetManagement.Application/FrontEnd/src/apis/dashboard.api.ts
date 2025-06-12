import { AssetByCategory, AssetByLocation, AssetByState, DashboardFilters, DashboardStats, MonthlyAssignmentStats, RecentActivity } from "@/types/dashboard.type";
import http from "@/utils/http";

const dashboardApi = {
  async getStats(filters: DashboardFilters): Promise<DashboardStats> {
    const { data } = await http.get(`/dashboard/stats?timeRange=${filters.timeRange}&location=${filters.location || ''}`);
    return data;
  },

  async getAssetsByCategory(filters: DashboardFilters): Promise<AssetByCategory[]> {
    const { data } = await http.get(`/dashboard/assets-by-category?timeRange=${filters.timeRange}&location=${filters.location || ''}`);
    return data;
  },

  async getAssetsByState(filters: DashboardFilters): Promise<AssetByState[]> {
    const { data } = await http.get(`/dashboard/assets-by-state?timeRange=${filters.timeRange}&location=${filters.location || ''}`);
    return data;
  },

  async getAssetsByLocation(filters: DashboardFilters): Promise<AssetByLocation[]> {
    const { data } = await http.get(`/dashboard/assets-by-location?timeRange=${filters.timeRange}`);
    return data;
  },

  async getMonthlyStats(filters: DashboardFilters): Promise<MonthlyAssignmentStats[]> {
    const { data } = await http.get(`/dashboard/monthly-stats?timeRange=${filters.timeRange}&location=${filters.location || ''}`);
    return data;
  },

  async getRecentActivity(filters: DashboardFilters): Promise<RecentActivity[]> {
    const { data } = await http.get(`/dashboard/recent-activity?timeRange=${filters.timeRange}&location=${filters.location || ''}&limit=10`);
    return data;
  }
};

export default dashboardApi;