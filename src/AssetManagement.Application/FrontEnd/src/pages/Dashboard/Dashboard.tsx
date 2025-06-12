import React, { useState, useMemo } from 'react';
import { PieChart, Pie, Cell, BarChart, Bar, XAxis, YAxis, CartesianGrid, Tooltip, Legend, LineChart, Line, ResponsiveContainer } from 'recharts';
import { Package, Users, TrendingUp, AlertTriangle, CheckCircle, Clock, ClipboardList } from 'lucide-react';
import { useDashboard } from '../../hooks/useDashboard';
import { DashboardFilters } from '../../types/dashboard.type';
import { NavLink } from 'react-router-dom';
import path from '@/constants/path';
import { format } from 'date-fns';

// Created by ClaudeAI
const Dashboard: React.FC = () => {
  const [timeRange, setTimeRange] = useState<'7d' | '30d' | '90d' | '1y'>('30d');
  const [selectedLocation] = useState<string | undefined>();

  // Create filters object
  const filters: DashboardFilters = {
    timeRange,
    location: selectedLocation
  };

  // Get hook functions
  const {
    useDashboardStats,
    useAssetsByCategory,
    useAssetsByState,
    useAssetsByLocation,
    useMonthlyStats,
    useRecentActivity
  } = useDashboard();

  // Call individual hooks
  const {
    data: dashboardStats,
    isLoading: statsLoading,
    isError: statsError,
    error: statsErrorData,
    refetch: refetchStats
  } = useDashboardStats(filters);

  const {
    data: assetsByCategory,
    isLoading: categoryLoading,
    isError: categoryError
  } = useAssetsByCategory(filters);

  const {
    data: assetsByState,
    isLoading: stateLoading,
    isError: stateError
  } = useAssetsByState(filters);

  const {
    data: assetsByLocation,
    isLoading: locationLoading,
    isError: locationError
  } = useAssetsByLocation(filters);

  const {
    data: monthlyStats,
    isLoading: monthlyLoading,
    isError: monthlyError
  } = useMonthlyStats(filters);

  const {
    data: recentActivity,
    isLoading: activityLoading,
    isError: activityError
  } = useRecentActivity(filters);

  // Combined loading and error states
  const isLoading = statsLoading || categoryLoading || stateLoading || locationLoading || monthlyLoading || activityLoading;
  const isError = statsError || categoryError || stateError || locationError || monthlyError || activityError;

  // Helper functions
  const getStateColor = (state: string): string => {
    switch (state) {
      case 'Available':
        return '#10B981';
      case 'Assigned':
        return '#3B82F6';
      case 'Not available':
        return '#EF4444';
      default:
        return '#6B7280';
    }
  };

  const getLocationName = (location: string): string => {
    switch (location) {
      case 'HCM':
        return 'Ho Chi Minh';
      case 'HN':
        return 'Ha Noi';
      case 'DN':
        return 'Da Nang';
      default:
        return location;
    }
  };

  const formatTimestamp = (timestamp: string): string => {
    const date = new Date(timestamp);
    const now = new Date();
    const diffMs = now.getTime() - date.getTime();
    const diffMins = Math.floor(diffMs / 60000);
    const diffHours = Math.floor(diffMins / 60);
    const diffDays = Math.floor(diffHours / 24);

    if (diffMins < 60) return `${diffMins} minutes ago`;
    if (diffHours < 24) return `${diffHours} hours ago`;
    return `${diffDays} days ago`;
  };

  const getActivityIcon = (type: string) => {
    switch (type) {
      case 'assignment':
        return <Users className="w-4 h-4 text-blue-500" />;
      case 'return':
        return <CheckCircle className="w-4 h-4 text-green-500" />;
      case 'asset_created':
        return <Package className="w-4 h-4 text-purple-500" />;
      case 'asset_updated':
        return <TrendingUp className="w-4 h-4 text-orange-500" />;
      case 'user_created':
        return <Users className="w-4 h-4 text-indigo-500" />;
      default:
        return <Clock className="w-4 h-4 text-gray-500" />;
    }
  };

  // Calculate derived metrics
  const utilizationRate = useMemo(() => {
    if (!dashboardStats) return '0.0';
    return ((dashboardStats.assignedAssets / dashboardStats.totalAssets) * 100).toFixed(1);
  }, [dashboardStats]);

  const availabilityRate = useMemo(() => {
    if (!dashboardStats) return '0.0';
    return ((dashboardStats.availableAssets / dashboardStats.totalAssets) * 100).toFixed(1);
  }, [dashboardStats]);

  // Transform data for charts
  const stateChartData = useMemo(() => {
    if (!assetsByState) return [];
    return assetsByState.map(item => ({
      ...item,
      color: getStateColor(item.state)
    }));
  }, [assetsByState]);

  const locationChartData = useMemo(() => {
    if (!assetsByLocation) return [];
    return assetsByLocation.map(item => ({
      location: getLocationName(item.location),
      count: item.count
    }));
  }, [assetsByLocation]);

  // Loading state
  if (isLoading) {
    return (
      <div className="flex items-center justify-center h-64">
        <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-blue-500"></div>
      </div>
    );
  }

  // Error state
  if (isError) {
    return (
      <div className="p-6">
        <div className="bg-red-50 border border-red-200 rounded-lg p-4">
          <div className="flex items-center">
            <AlertTriangle className="w-5 h-5 text-red-500 mr-2" />
            <h3 className="text-red-800 font-medium">Error Loading Dashboard</h3>
          </div>
          <p className="text-red-700 mt-2">
            {statsErrorData instanceof Error ? statsErrorData.message : 'An unexpected error occurred'}
          </p>
          <button
            onClick={() => refetchStats()}
            className="mt-3 text-sm bg-red-100 hover:bg-red-200 text-red-800 px-3 py-1 rounded"
          >
            Try Again
          </button>
        </div>
      </div>
    );
  }

  return (
    <div className="p-6 bg-gray-50 min-h-screen">
      {/* Header */}
      <div className="mb-8">
        <h1 className="text-3xl font-bold text-gray-900 mb-2">Asset Management Dashboard</h1>
        <p className="text-gray-600">Overview of your asset inventory and assignments</p>
      </div>

      {/* Time Range Filter */}
      <div className="mb-6">
        <div className="flex space-x-2">
          {(['7d', '30d', '90d', '1y'] as const).map((range) => (
            <button
              key={range}
              onClick={() => setTimeRange(range)}
              className={`px-4 py-2 rounded-lg text-sm font-medium transition-colors ${timeRange === range
                ? 'bg-blue-500 text-white'
                : 'bg-white text-gray-700 hover:bg-gray-100 border border-gray-300'
                }`}
            >
              {range === '7d' && 'Last 7 Days'}
              {range === '30d' && 'Last 30 Days'}
              {range === '90d' && 'Last 90 Days'}
              {range === '1y' && 'Last Year'}
            </button>
          ))}
        </div>
      </div>

      {/* Key Metrics Cards */}
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6 mb-8">
        <div className="bg-white rounded-lg shadow-sm p-6 border border-gray-200">
          <div className="flex items-center justify-between">
            <div>
              <p className="text-sm font-medium text-gray-600">Total Assets</p>
              <p className="text-3xl font-bold text-gray-900">
                {dashboardStats?.totalAssets?.toLocaleString() || '0'}
              </p>
            </div>
            <Package className="w-8 h-8 text-blue-500" />
          </div>
          <p className="text-sm text-gray-500 mt-2">Across all categories</p>
        </div>

        <div className="bg-white rounded-lg shadow-sm p-6 border border-gray-200">
          <div className="flex items-center justify-between">
            <div>
              <p className="text-sm font-medium text-gray-600">Active Assignments</p>
              <p className="text-3xl font-bold text-gray-900">
                {dashboardStats?.activeAssignments || '0'}
              </p>
            </div>
            <Users className="w-8 h-8 text-green-500" />
          </div>
          <p className="text-sm text-gray-500 mt-2">{utilizationRate}% utilization rate</p>
        </div>

        <div className="bg-white rounded-lg shadow-sm p-6 border border-gray-200">
          <div className="flex items-center justify-between">
            <div>
              <p className="text-sm font-medium text-gray-600">Available Assets</p>
              <p className="text-3xl font-bold text-gray-900">
                {dashboardStats?.availableAssets || '0'}
              </p>
            </div>
            <CheckCircle className="w-8 h-8 text-blue-500" />
          </div>
          <p className="text-sm text-gray-500 mt-2">{availabilityRate}% availability rate</p>
        </div>

        <div className="bg-white rounded-lg shadow-sm p-6 border border-gray-200">
          <div className="flex items-center justify-between">
            <div>
              <p className="text-sm font-medium text-gray-600">Pending Returns</p>
              <p className="text-3xl font-bold text-gray-900">
                {dashboardStats?.pendingReturns || '0'}
              </p>
            </div>
            <AlertTriangle className="w-8 h-8 text-orange-500" />
          </div>
          <p className="text-sm text-gray-500 mt-2">Require attention</p>
        </div>
      </div>

      {/* Charts Section */}
      <div className="grid grid-cols-1 lg:grid-cols-2 gap-6 mb-8">
        {/* Asset Distribution by State */}
        <div className="bg-white rounded-lg shadow-sm p-6 border border-gray-200">
          <h3 className="text-lg font-semibold text-gray-900 mb-4">Asset Distribution by State</h3>
          <ResponsiveContainer width="100%" height={300}>
            <PieChart>
              <Pie
                data={stateChartData}
                cx="50%"
                cy="50%"
                outerRadius={100}
                fill="#8884d8"
                dataKey="count"
                label={({ state, count }) => `${state}: ${count}`}
              >
                {stateChartData.map((entry, index) => (
                  <Cell key={`cell-${index}`} fill={entry.color} />
                ))}
              </Pie>
              <Tooltip />
            </PieChart>
          </ResponsiveContainer>
        </div>

        {/* Assets by Category */}
        <div className="bg-white rounded-lg shadow-sm p-6 border border-gray-200">
          <h3 className="text-lg font-semibold text-gray-900 mb-4">Assets by Category</h3>
          <ResponsiveContainer width="100%" height={300}>
            <BarChart data={assetsByCategory}>
              <CartesianGrid strokeDasharray="3 3" />
              <XAxis dataKey="prefix" />
              <YAxis />
              <Tooltip />
              <Bar dataKey="count" fill="#3B82F6" />
            </BarChart>
          </ResponsiveContainer>
        </div>

        {/* Monthly Assignments Trend */}
        <div className="bg-white rounded-lg shadow-sm p-6 border border-gray-200">
          <h3 className="text-lg font-semibold text-gray-900 mb-4">Monthly Assignment Trends</h3>
          <ResponsiveContainer width="100%" height={300}>
            <LineChart data={monthlyStats}>
              <CartesianGrid strokeDasharray="3 3" />
              <XAxis dataKey="month" />
              <YAxis />
              <Tooltip />
              <Legend />
              <Line type="monotone" dataKey="assignments" stroke="#3B82F6" strokeWidth={2} name="Assignments" />
              <Line type="monotone" dataKey="returns" stroke="#10B981" strokeWidth={2} name="Returns" />
            </LineChart>
          </ResponsiveContainer>
        </div>

        {/* Assets by Location */}
        <div className="bg-white rounded-lg shadow-sm p-6 border border-gray-200">
          <h3 className="text-lg font-semibold text-gray-900 mb-4">Assets by Location</h3>
          <ResponsiveContainer width="100%" height={300}>
            <BarChart data={locationChartData} layout="vertical">
              <CartesianGrid strokeDasharray="3 3" />
              <XAxis type="number" />
              <YAxis dataKey="location" type="category" width={100} />
              <Tooltip />
              <Bar dataKey="count" fill="#8B5CF6" />
            </BarChart>
          </ResponsiveContainer>
        </div>
      </div>

      {/* Recent Activity */}
      <div className="bg-white rounded-lg shadow-sm p-6 border border-gray-200">
        <div className="flex items-center justify-between mb-4">
          <h3 className="text-lg font-semibold text-gray-900">Recent Activity</h3>
          <NavLink to={path.assignment} className="text-sm text-blue-500 hover:text-blue-700">View All</NavLink>
        </div>
        <div className="space-y-4">
          {recentActivity?.map((activity) => (
            <div key={activity.id} className="flex items-start space-x-3 p-3 bg-gray-50 rounded-lg">
              <div className="flex-shrink-0">
                {getActivityIcon(activity.type)}
              </div>
              <div className="flex-1 min-w-0">
                <p className="text-sm text-gray-900">{activity.description}</p>
                <div className="flex items-center space-x-2 mt-1">
                  <span className="text-xs text-gray-500">by {activity.userName}</span>
                  <span className="text-xs text-gray-400">•</span>
                  <span className="text-xs text-gray-500">{formatTimestamp(activity.timestamp)} (on {format(activity.timestamp, "dd/MM/yyyy")})</span>
                </div>
              </div>
            </div>
          )) || (
              <div className="text-center py-4 text-gray-500">
                No recent activity to display
              </div>
            )}
        </div>
      </div>

      {/* Quick Actions */}
      <div className="mt-8 bg-white rounded-lg shadow-sm p-6 border border-gray-200">
        <h3 className="text-lg font-semibold text-gray-900 mb-4">Quick Actions</h3>
        <div className="grid grid-cols-2 md:grid-cols-4 gap-4">
          <NavLink to={path.assetCreate} className="flex items-center justify-center space-x-2 p-4 bg-blue-50 hover:bg-blue-100 rounded-lg border border-blue-200 transition-colors hover:cursor-pointer">
            <Package className="w-5 h-5 text-blue-600" />
            <span className="text-sm font-medium text-blue-600">Add Asset</span>
          </NavLink>
          <NavLink to={path.assignmentCreate} className="flex items-center justify-center space-x-2 p-4 bg-green-50 hover:bg-green-100 rounded-lg border border-green-200 transition-colors hover:cursor-pointer">
            <ClipboardList className="w-5 h-5 text-green-600" />
            <span className="text-sm font-medium text-green-600">New Assignment</span>
          </NavLink>
          <NavLink to={path.report} className="flex items-center justify-center space-x-2 p-4 bg-purple-50 hover:bg-purple-100 rounded-lg border border-purple-200 transition-colors hover:cursor-pointer">
            <TrendingUp className="w-5 h-5 text-purple-600" />
            <span className="text-sm font-medium text-purple-600">Reports</span>
          </NavLink>
          <NavLink to={path.user} className="flex items-center justify-center space-x-2 p-4 bg-orange-50 hover:bg-orange-100 rounded-lg border border-orange-200 transition-colors hover:cursor-pointer">
            <Users className="w-5 h-5 text-orange-600" />
            <span className="text-sm font-medium text-orange-600">Users</span>
          </NavLink>
        </div>
      </div>
    </div>
  );
};

export default Dashboard;