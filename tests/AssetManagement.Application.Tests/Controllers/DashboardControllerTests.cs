using AssetManagement.Application.Controllers;
using AssetManagement.Application.Services.Interfaces;
using AssetManagement.Contracts.DTOs;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace AssetManagement.Application.Tests.Controllers
{
    public class DashboardControllerTests
    {
        private readonly Mock<IDashboardService> _dashboardServiceMock;
        private readonly DashboardController _controller;

        public DashboardControllerTests()
        {
            _dashboardServiceMock = new Mock<IDashboardService>();
            _controller = new DashboardController(_dashboardServiceMock.Object);
        }

        [Fact]
        public async Task GetDashboardStats_ReturnsOkWithStats()
        {
            // Arrange
            var filters = new DashboardFilters { TimeRange = "30d", Location = "HaNoi" };
            var expectedStats = new DashboardStatsDto
            {
                TotalAssets = 100,
                AvailableAssets = 50,
                AssignedAssets = 30,
                NotAvailableAssets = 20,
                TotalUsers = 25,
                ActiveAssignments = 30,
                PendingReturns = 5,
                TotalCategories = 10
            };

            _dashboardServiceMock
                .Setup(x => x.GetDashboardStatsAsync(It.IsAny<DashboardFilters>()))
                .ReturnsAsync(expectedStats);

            // Act
            var result = await _controller.GetDashboardStats(filters);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var actualStats = Assert.IsType<DashboardStatsDto>(okResult.Value);
            Assert.Equal(expectedStats.TotalAssets, actualStats.TotalAssets);
            Assert.Equal(expectedStats.AvailableAssets, actualStats.AvailableAssets);

            _dashboardServiceMock.Verify(x => x.GetDashboardStatsAsync(It.IsAny<DashboardFilters>()), Times.Once);
        }

        [Fact]
        public async Task GetAssetsByCategory_ReturnsOkWithData()
        {
            // Arrange
            var expectedData = new List<AssetByCategoryDto>
            {
                new AssetByCategoryDto { CategoryId = Guid.NewGuid(), CategoryName = "Laptop", Count = 25, Prefix = "LP" },
                new AssetByCategoryDto { CategoryId = Guid.NewGuid(), CategoryName = "Monitor", Count = 15, Prefix = "MN" }
            };

            _dashboardServiceMock
                .Setup(x => x.GetAssetsByCategoryAsync())
                .ReturnsAsync(expectedData);

            // Act
            var result = await _controller.GetAssetsByCategory();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var actualData = Assert.IsType<List<AssetByCategoryDto>>(okResult.Value);
            Assert.Equal(expectedData.Count, actualData.Count);
            Assert.Equal(expectedData.First().CategoryName, actualData.First().CategoryName);

            _dashboardServiceMock.Verify(x => x.GetAssetsByCategoryAsync(), Times.Once);
        }

        [Fact]
        public async Task GetAssetsByState_ReturnsOkWithData()
        {
            // Arrange
            var filters = new DashboardFilters { Location = "HaNoi" };
            var expectedData = new List<AssetByStateDto>
            {
                new AssetByStateDto { State = "Available", Count = 50 },
                new AssetByStateDto { State = "Assigned", Count = 30 }
            };

            _dashboardServiceMock
                .Setup(x => x.GetAssetsByStateAsync(It.IsAny<DashboardFilters>()))
                .ReturnsAsync(expectedData);

            // Act
            var result = await _controller.GetAssetsByState(filters);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var actualData = Assert.IsType<List<AssetByStateDto>>(okResult.Value);
            Assert.Equal(expectedData.Count, actualData.Count);

            _dashboardServiceMock.Verify(x => x.GetAssetsByStateAsync(It.IsAny<DashboardFilters>()), Times.Once);
        }

        [Fact]
        public async Task GetAssetsByLocation_ReturnsOkWithData()
        {
            // Arrange
            var filters = new DashboardFilters();
            var expectedData = new List<AssetByLocationDto>
            {
                new AssetByLocationDto { Location = "Ha Noi", Count = 60 },
                new AssetByLocationDto { Location = "Ho Chi Minh", Count = 40 }
            };

            _dashboardServiceMock
                .Setup(x => x.GetAssetsByLocationAsync(It.IsAny<DashboardFilters>()))
                .ReturnsAsync(expectedData);

            // Act
            var result = await _controller.GetAssetsByLocation(filters);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var actualData = Assert.IsType<List<AssetByLocationDto>>(okResult.Value);
            Assert.Equal(expectedData.Count, actualData.Count);

            _dashboardServiceMock.Verify(x => x.GetAssetsByLocationAsync(It.IsAny<DashboardFilters>()), Times.Once);
        }

        [Fact]
        public async Task GetMonthlyStats_ReturnsOkWithData()
        {
            // Arrange
            var filters = new DashboardFilters { TimeRange = "90d" };
            var expectedData = new List<MonthlyAssignmentStatsDto>
            {
                new MonthlyAssignmentStatsDto { Month = "Jan", Year = 2024, Assignments = 10, Returns = 8 },
                new MonthlyAssignmentStatsDto { Month = "Feb", Year = 2024, Assignments = 15, Returns = 12 }
            };

            _dashboardServiceMock
                .Setup(x => x.GetMonthlyStatsAsync(It.IsAny<DashboardFilters>()))
                .ReturnsAsync(expectedData);

            // Act
            var result = await _controller.GetMonthlyStats(filters);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var actualData = Assert.IsType<List<MonthlyAssignmentStatsDto>>(okResult.Value);
            Assert.Equal(expectedData.Count, actualData.Count);

            _dashboardServiceMock.Verify(x => x.GetMonthlyStatsAsync(It.IsAny<DashboardFilters>()), Times.Once);
        }

        [Fact]
        public async Task GetRecentActivity_WithDefaultLimit_ReturnsOkWithData()
        {
            // Arrange
            var filters = new DashboardFilters { TimeRange = "7d" };
            var expectedData = new List<RecentActivityDto>
            {
                new RecentActivityDto
                {
                    Id = Guid.NewGuid(),
                    Type = "assignment",
                    Description = "Asset LP001 assigned to John Doe",
                    Timestamp = DateTimeOffset.Now.Date,
                    UserName = "Admin User",
                    AssetCode = "LP001"
                }
            };

            _dashboardServiceMock
                .Setup(x => x.GetRecentActivityAsync(It.IsAny<DashboardFilters>(), It.IsAny<int>()))
                .ReturnsAsync(expectedData);

            // Act
            var result = await _controller.GetRecentActivity(filters);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var actualData = Assert.IsType<List<RecentActivityDto>>(okResult.Value);
            Assert.Equal(expectedData.Count, actualData.Count);

            _dashboardServiceMock.Verify(x => x.GetRecentActivityAsync(It.IsAny<DashboardFilters>(), 10), Times.Once);
        }

        [Fact]
        public async Task GetRecentActivity_WithCustomLimit_ReturnsOkWithData()
        {
            // Arrange
            var filters = new DashboardFilters { TimeRange = "7d" };
            var customLimit = 25;
            var expectedData = new List<RecentActivityDto>();

            _dashboardServiceMock
                .Setup(x => x.GetRecentActivityAsync(It.IsAny<DashboardFilters>(), customLimit))
                .ReturnsAsync(expectedData);

            // Act
            var result = await _controller.GetRecentActivity(filters, customLimit);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var actualData = Assert.IsType<List<RecentActivityDto>>(okResult.Value);
            Assert.Equal(expectedData.Count, actualData.Count);

            _dashboardServiceMock.Verify(x => x.GetRecentActivityAsync(It.IsAny<DashboardFilters>(), customLimit), Times.Once);
        }

        [Fact]
        public async Task GetDashboardStats_WhenServiceThrowsException_PropagatesException()
        {
            // Arrange
            var filters = new DashboardFilters();
            _dashboardServiceMock
                .Setup(x => x.GetDashboardStatsAsync(It.IsAny<DashboardFilters>()))
                .ThrowsAsync(new InvalidOperationException("Test exception"));

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => _controller.GetDashboardStats(filters));
        }
    }
}