export interface DashboardStats {
  totalAssets: number;
  availableAssets: number;
  assignedAssets: number;
  notAvailableAssets: number;
  totalUsers: number;
  activeAssignments: number;
  pendingReturns: number;
  totalCategories: number;
}

export interface AssetByCategory {
  categoryId: string;
  categoryName: string;
  count: number;
  prefix: string;
}

export interface AssetByState {
  state: string;
  count: number;
}

export interface AssetByLocation {
  location: string;
  count: number;
}

export interface MonthlyAssignmentStats {
  month: string;
  year: number;
  assignments: number;
  returns: number;
}

export interface RecentActivity {
  id: string;
  type: 'assignment' | 'return' | 'asset_created' | 'asset_updated' | 'user_created';
  description: string;
  timestamp: string;
  userId: string;
  userName: string;
  assetId?: string;
  assetCode?: string;
}

export interface DashboardFilters {
  timeRange: '7d' | '30d' | '90d' | '1y';
  location?: string;
}