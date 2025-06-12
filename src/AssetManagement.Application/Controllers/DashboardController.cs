using AssetManagement.Application.Services.Interfaces;
using AssetManagement.Contracts.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AssetManagement.Application.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardService _dashboardService;

        public DashboardController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        [HttpGet("stats")]
        public async Task<ActionResult<DashboardStatsDto>> GetDashboardStats([FromQuery] DashboardFilters filters)
        {
            var stats = await _dashboardService.GetDashboardStatsAsync(filters);
            return Ok(stats);
        }

        [HttpGet("assets-by-category")]
        public async Task<ActionResult<List<AssetByCategoryDto>>> GetAssetsByCategory()
        {
            var data = await _dashboardService.GetAssetsByCategoryAsync();
            return Ok(data);
        }

        [HttpGet("assets-by-state")]
        public async Task<ActionResult<List<AssetByStateDto>>> GetAssetsByState([FromQuery] DashboardFilters filters)
        {
            var data = await _dashboardService.GetAssetsByStateAsync(filters);
            return Ok(data);
        }

        [HttpGet("assets-by-location")]
        public async Task<ActionResult<List<AssetByLocationDto>>> GetAssetsByLocation([FromQuery] DashboardFilters filters)
        {
            var data = await _dashboardService.GetAssetsByLocationAsync(filters);
            return Ok(data);
        }

        [HttpGet("monthly-stats")]
        public async Task<ActionResult<List<MonthlyAssignmentStatsDto>>> GetMonthlyStats([FromQuery] DashboardFilters filters)
        {
            var data = await _dashboardService.GetMonthlyStatsAsync(filters);
            return Ok(data);
        }

        [HttpGet("recent-activity")]
        public async Task<ActionResult<List<RecentActivityDto>>> GetRecentActivity([FromQuery] DashboardFilters filters, [FromQuery] int limit = 10)
        {
            var data = await _dashboardService.GetRecentActivityAsync(filters, limit);
            return Ok(data);
        }
    }
}