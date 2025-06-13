using AssetManagement.Application.Extensions;
using AssetManagement.Application.Services.Interfaces;
using AssetManagement.Contracts.DTOs;
using AssetManagement.Contracts.Enums;
using AssetManagement.Domain.Enums;
using AssetManagement.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace AssetManagement.Application.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly IAssetRepository _assetRepository;
        private readonly IUserRepository _userRepository;
        private readonly IAssignmentRepository _assignmentRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IReturnRequestRepository _returnRequestRepository;

        public DashboardService(IAssetRepository assetRepository,
            IUserRepository userRepository,
            IAssignmentRepository assignmentRepository,
            ICategoryRepository categoryRepository,
            IReturnRequestRepository returnRequestRepository)
        {
            _assetRepository = assetRepository;
            _userRepository = userRepository;
            _assignmentRepository = assignmentRepository;
            _categoryRepository = categoryRepository;
            _returnRequestRepository = returnRequestRepository;
        }

        public async Task<DashboardStatsDto> GetDashboardStatsAsync(DashboardFilters filters)
        {
            var dateRange = GetDateRange(filters.TimeRange);

            var assetsQuery = _assetRepository.GetAll().Where(a => !a.IsDeleted.HasValue || !a.IsDeleted.Value);
            var usersQuery = _userRepository.GetAll().Where(u => !u.IsDeleted.HasValue || !u.IsDeleted.Value);
            var assignmentsQuery = _assignmentRepository.GetAll().Where(a => !a.IsDeleted.HasValue || !a.IsDeleted.Value);

            if (!string.IsNullOrEmpty(filters.Location) && Enum.TryParse<Location>(filters.Location, true, out var location))
            {
                assetsQuery = assetsQuery.Where(a => a.Location == location);
                usersQuery = usersQuery.Where(u => u.Location == location);
            }

            var totalAssets = await assetsQuery.CountAsync();
            var availableAssets = await assetsQuery.CountAsync(a => a.State == AssetState.Available);
            var assignedAssets = await assetsQuery.CountAsync(a => a.State == AssetState.Assigned);
            var notAvailableAssets = await assetsQuery.CountAsync(a => a.State == AssetState.NotAvailable);

            var totalUsers = await usersQuery.CountAsync(u => u.IsActive);
            var activeAssignments = await assignmentsQuery.CountAsync(a => a.State == AssignmentState.Accepted);

            var pendingReturns = await _returnRequestRepository.GetAll()
                .Where(r => r.State == ReturnRequestState.WaitingForReturning &&
                           (!r.IsDeleted.HasValue || !r.IsDeleted.Value))
                .CountAsync();

            var totalCategories = await _categoryRepository.GetAll()
                .Where(c => !c.IsDeleted.HasValue || !c.IsDeleted.Value)
                .CountAsync();

            return new DashboardStatsDto
            {
                TotalAssets = totalAssets,
                AvailableAssets = availableAssets,
                AssignedAssets = assignedAssets,
                NotAvailableAssets = notAvailableAssets,
                TotalUsers = totalUsers,
                ActiveAssignments = activeAssignments,
                PendingReturns = pendingReturns,
                TotalCategories = totalCategories
            };
        }

        public async Task<List<AssetByCategoryDto>> GetAssetsByCategoryAsync()
        {
            var query = _assetRepository.GetAll()
                .Where(a => !a.IsDeleted.HasValue || !a.IsDeleted.Value)
                .Include(a => a.Category);

            return await query
                .GroupBy(a => new { a.CategoryId, a.Category.Name, a.Category.Prefix })
                .Select(g => new AssetByCategoryDto
                {
                    CategoryId = g.Key.CategoryId,
                    CategoryName = g.Key.Name,
                    Count = g.Count(),
                    Prefix = g.Key.Prefix
                })
                .OrderByDescending(x => x.Count)
                .ToListAsync();
        }

        public async Task<List<AssetByStateDto>> GetAssetsByStateAsync(DashboardFilters filters)
        {
            var query = _assetRepository.GetAll()
                .Where(a => !a.IsDeleted.HasValue || !a.IsDeleted.Value);

            if (!string.IsNullOrEmpty(filters.Location) && Enum.TryParse<Location>(filters.Location, true, out var location))
            {
                query = query.Where(a => a.Location == location);
            }

            return await query
                .GroupBy(a => a.State)
                .Select(g => new AssetByStateDto
                {
                    State = g.Key.GetDisplayName(),
                    Count = g.Count()
                })
                .ToListAsync();
        }

        public async Task<List<AssetByLocationDto>> GetAssetsByLocationAsync(DashboardFilters filters)
        {
            return await _assetRepository.GetAll()
                .Where(a => !a.IsDeleted.HasValue || !a.IsDeleted.Value)
                .GroupBy(a => a.Location)
                .Select(g => new AssetByLocationDto
                {
                    Location = g.Key.GetDisplayName(),
                    Count = g.Count()
                })
                .OrderByDescending(x => x.Count)
                .ToListAsync();
        }

        public async Task<List<MonthlyAssignmentStatsDto>> GetMonthlyStatsAsync(DashboardFilters filters)
        {
            var dateRange = GetDateRange(filters.TimeRange);
            var startDate = dateRange.startDate;
            var endDate = dateRange.endDate;

            var assignmentsQuery = _assignmentRepository.GetAll()
                .Where(a => a.AssignedDate >= startDate && a.AssignedDate <= endDate &&
                           (!a.IsDeleted.HasValue || !a.IsDeleted.Value));

            var returnsQuery = _returnRequestRepository.GetAll()
                .Where(r => r.ReturnedDate.HasValue &&
                           r.ReturnedDate >= startDate && r.ReturnedDate <= endDate &&
                           (!r.IsDeleted.HasValue || !r.IsDeleted.Value));

            if (!string.IsNullOrEmpty(filters.Location) && Enum.TryParse<Location>(filters.Location, true, out var location))
            {
                assignmentsQuery = assignmentsQuery
                    .Include(a => a.Asset)
                    .Where(a => a.Asset.Location == location);

                returnsQuery = returnsQuery
                    .Include(r => r.Assignment)
                    .ThenInclude(a => a.Asset)
                    .Where(r => r.Assignment.Asset.Location == location);
            }

            var assignments = await assignmentsQuery
                .GroupBy(a => new { a.AssignedDate.Year, a.AssignedDate.Month })
                .Select(g => new { g.Key.Year, g.Key.Month, Count = g.Count() })
                .ToListAsync();

            var returns = await returnsQuery
                .GroupBy(r => new { r.ReturnedDate!.Value.Year, r.ReturnedDate!.Value.Month })
                .Select(g => new { g.Key.Year, g.Key.Month, Count = g.Count() })
                .ToListAsync();

            var result = new List<MonthlyAssignmentStatsDto>();
            var currentDate = startDate;

            while (currentDate <= endDate)
            {
                var assignmentCount = assignments
                    .FirstOrDefault(a => a.Year == currentDate.Year && a.Month == currentDate.Month)?.Count ?? 0;

                var returnCount = returns
                    .FirstOrDefault(r => r.Year == currentDate.Year && r.Month == currentDate.Month)?.Count ?? 0;

                result.Add(new MonthlyAssignmentStatsDto
                {
                    Month = currentDate.ToString("MMM"),
                    Year = currentDate.Year,
                    Assignments = assignmentCount,
                    Returns = returnCount
                });

                currentDate = currentDate.AddMonths(1);
            }

            return result;
        }

        public async Task<List<RecentActivityDto>> GetRecentActivityAsync(DashboardFilters filters, int limit = 10)
        {
            var dateRange = GetDateRange(filters.TimeRange);
            var activities = new List<RecentActivityDto>();

            // Get recent assignments
            var recentAssignments = await _assignmentRepository.GetAll()
                .Where(a => a.CreatedDate >= dateRange.startDate &&
                           (!a.IsDeleted.HasValue || !a.IsDeleted.Value))
                .Include(a => a.Asset)
                .Include(a => a.Assignee)
                .Include(a => a.Assignor)
                .OrderByDescending(a => a.CreatedDate)
                .Take(limit)
                .Select(a => new RecentActivityDto
                {
                    Id = a.Id,
                    Type = "assignment",
                    Description = $"Asset {a.Asset.Code} assigned to {a.Assignee.Username} ({a.Assignee.FirstName} {a.Assignee.LastName})",
                    Timestamp = a.CreatedDate,
                    UserId = a.AssignorId,
                    UserName = a.Assignor.Username,
                    AssetId = a.AssetId,
                    AssetCode = a.Asset.Code
                })
                .ToListAsync();

            activities.AddRange(recentAssignments);

            // Get recent returns
            var recentReturns = await _returnRequestRepository.GetAll()
                .Where(r => r.CreatedDate >= dateRange.startDate &&
                           r.State == ReturnRequestState.Completed &&
                           (!r.IsDeleted.HasValue || !r.IsDeleted.Value))
                .Include(r => r.Assignment)
                .ThenInclude(a => a.Asset)
                .Include(r => r.Assignment)
                .ThenInclude(a => a.Assignee)
                .Include(r => r.Acceptor)
                .OrderByDescending(r => r.ReturnedDate)
                .Take(limit)
                .Select(r => new RecentActivityDto
                {
                    Id = r.Id,
                    Type = "return",
                    Description = $"Asset {r.Assignment.Asset.Code} returned by {r.Assignment.Assignee.FirstName} {r.Assignment.Assignee.LastName}",
                    Timestamp = r.CreatedDate,
                    UserId = r.AcceptorId ?? Guid.Empty,
                    UserName = r.Acceptor != null ? r.Acceptor.Username : "System",
                    AssetId = r.Assignment.AssetId,
                    AssetCode = r.Assignment.Asset.Code
                })
                .ToListAsync();

            activities.AddRange(recentReturns);

            // Get recently created assets
            var recentAssets = await _assetRepository.GetAll()
                .Where(a => a.CreatedDate >= dateRange.startDate &&
                           (!a.IsDeleted.HasValue || !a.IsDeleted.Value))
                .Include(a => a.Category)
                .OrderByDescending(a => a.CreatedDate)
                .Take(limit)
                .Select(a => new RecentActivityDto
                {
                    Id = a.Id,
                    Type = "asset_created",
                    Description = $"New {a.Category.Name} {a.Code} added to inventory",
                    Timestamp = a.CreatedDate,
                    UserId = a.CreatedBy ?? Guid.Empty,
                    UserName = "System",
                    AssetId = a.Id,
                    AssetCode = a.Code
                })
                .ToListAsync();

            activities.AddRange(recentAssets);

            return activities
                .OrderByDescending(a => a.Timestamp)
                .Take(limit)
                .ToList();
        }

        private (DateTimeOffset startDate, DateTimeOffset endDate) GetDateRange(string timeRange)
        {
            var endDate = DateTimeOffset.Now;
            var startDate = timeRange switch
            {
                "7d" => endDate.AddDays(-7),
                "30d" => endDate.AddDays(-30),
                "90d" => endDate.AddDays(-90),
                "1y" => endDate.AddYears(-1),
                _ => endDate.AddDays(-30)
            };

            return (startDate, endDate);
        }
    }
}