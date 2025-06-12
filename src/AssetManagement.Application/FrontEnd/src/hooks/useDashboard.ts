import dashboardApi from "@/apis/dashboard.api";
import { DashboardFilters } from "@/types/dashboard.type";
import { useQuery } from "@tanstack/react-query";

export function useDashboard() {
  function useDashboardStats(filters: DashboardFilters) {
    return useQuery({
      queryKey: ['dashboard-stats', filters],
      queryFn: () => dashboardApi.getStats(filters),
    });
  };

  function useAssetsByCategory(filters: DashboardFilters) {
    return useQuery({
      queryKey: ['assets-by-category', filters],
      queryFn: () => dashboardApi.getAssetsByCategory(filters),
    });
  };

  function useAssetsByState(filters: DashboardFilters) {
    return useQuery({
      queryKey: ['assets-by-state', filters],
      queryFn: () => dashboardApi.getAssetsByState(filters),
    });
  };

  function useAssetsByLocation(filters: DashboardFilters) {
    return useQuery({
      queryKey: ['assets-by-location', filters],
      queryFn: () => dashboardApi.getAssetsByLocation(filters),
    });
  };

  function useMonthlyStats(filters: DashboardFilters) {
    return useQuery({
      queryKey: ['monthly-stats', filters],
      queryFn: () => dashboardApi.getMonthlyStats(filters),
    });
  };

  function useRecentActivity(filters: DashboardFilters) {
    return useQuery({
      queryKey: ['recent-activity', filters],
      queryFn: () => dashboardApi.getRecentActivity(filters),
    });
  };

  return {
    useDashboardStats,
    useAssetsByCategory,
    useAssetsByState,
    useAssetsByLocation,
    useMonthlyStats,
    useRecentActivity,
  }
}

export default useDashboard;