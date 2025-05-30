using AssetManagement.Application.Services;
using AssetManagement.Contracts.Parameters;
using AssetManagement.Domain.Entities;
using AssetManagement.Domain.Enums;
using AssetManagement.Domain.Interfaces.Repositories;
using MockQueryable;
using Moq;

namespace AssetManagement.Application.Tests.Services
{
    public class AssignmentServiceTests
    {
        private readonly Mock<IAssignmentRepository> _mockAssignmentRepository;
        private readonly Mock<IUserRepository> _mockUserRepository;
        private readonly AssignmentService _assignmentService;

        public AssignmentServiceTests()
        {
            _mockAssignmentRepository = new Mock<IAssignmentRepository>();
            _mockUserRepository = new Mock<IUserRepository>();
            _assignmentService = new AssignmentService(_mockAssignmentRepository.Object, _mockUserRepository.Object);
        }

        private User CreateAdminUser()
        {
            var adminId = "123e4567-e89b-12d3-a456-426614174000";

            return new User
            {
                Id = Guid.Parse(adminId),
                Username = "admin1",
                Password = "Password",
                Location = Location.HCM
            };
        }

        private Assignment CreateAssignment(Guid assetId, string assetCode, string assetName, Location location,
                                            Guid assignorId, string assignorUsername, Guid assigneeId, string assigneeUsername,
                                            AssignmentState state)
        {
            return new Assignment
            {
                Id = Guid.NewGuid(),
                AssetId = assetId,
                Asset = new Asset { Code = assetCode, Name = assetName, Location = location },
                AssignorId = assignorId,
                Assignor = new User { Username = assignorUsername, Password = "Password" },
                AssigneeId = assigneeId,
                Assignee = new User { Username = assigneeUsername, Password = "Password" },
                AssignedDate = DateTimeOffset.Now,
                State = state
            };
        }

        private List<Assignment> CreateAssignmentList(int count, Location location, AssignmentState state)
        {
            var assignments = new List<Assignment>();
            for (int i = 1; i <= count; i++)
            {
                assignments.Add(CreateAssignment(
                    Guid.NewGuid(),
                    $"LP{i:000}",
                    $"Laptop {i}",
                    location,
                    Guid.NewGuid(),
                    "admin1",
                    Guid.NewGuid(),
                    $"user{i}",
                    state
                ));
            }
            return assignments;
        }

        [Fact]
        public async Task GetAssignmentsAsync_WithValidAdminId_ReturnsPagedResult()
        {
            // Arrange
            var adminUser = CreateAdminUser();

            var assignments = new List<Assignment>
            {
                CreateAssignment(Guid.NewGuid(), "LP001", "Laptop Dell", Location.HCM, Guid.NewGuid(), "admin1", Guid.NewGuid(), "user1", AssignmentState.Accepted),
                CreateAssignment(Guid.NewGuid(), "MO001", "Monitor LG", Location.HCM, Guid.NewGuid(), "admin2", Guid.NewGuid(), "user2", AssignmentState.WaitingForAcceptance)
            };

            var queryParams = new AssignmentQueryParameters
            {
                PageNumber = 1,
                PageSize = 10
            };

            _mockUserRepository.Setup(x => x.GetByIdAsync(adminUser.Id)).ReturnsAsync(adminUser);

            var mockQueryable = assignments.AsQueryable().BuildMock();
            _mockAssignmentRepository.Setup(x => x.GetAll()).Returns(mockQueryable);

            // Act
            var result = await _assignmentService.GetAssignmentsAsync(adminUser.Id.ToString(), queryParams);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Items.Count());
            Assert.Equal(2, result.PaginationMetadata.TotalItems);
            Assert.Equal(1, result.PaginationMetadata.CurrentPage);
            Assert.Equal(10, result.PaginationMetadata.PageSize);
        }

        [Fact]
        public async Task GetAssignmentsAsync_WithInvalidAdminId_ThrowsKeyNotFoundException()
        {
            // Arrange
            var adminId = "7643dabd-9069-42fc-b4b4-7f46a7ffc254";
            var queryParams = new AssignmentQueryParameters();

            _mockUserRepository.Setup(x => x.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((User?)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<KeyNotFoundException>(
                () => _assignmentService.GetAssignmentsAsync(adminId, queryParams));

            Assert.Contains($"User with id {adminId} not found", exception.Message);
        }

        [Fact]
        public async Task GetAssignmentsAsync_WithSearchTerm_FiltersCorrectly()
        {
            // Arrange
            var adminUser = CreateAdminUser();

            var assignments = new List<Assignment>
            {
                CreateAssignment(Guid.NewGuid(), "LP001", "Laptop Dell", Location.HCM, Guid.NewGuid(), "admin1", Guid.NewGuid(), "johndoe", AssignmentState.Accepted),
                CreateAssignment(Guid.NewGuid(), "MO001", "Monitor LG", Location.HCM, Guid.NewGuid(), "admin2", Guid.NewGuid(), "janesmith", AssignmentState.WaitingForAcceptance)
            };

            var queryParams = new AssignmentQueryParameters
            {
                SearchTerm = "john",
                PageNumber = 1,
                PageSize = 10
            };

            _mockUserRepository.Setup(x => x.GetByIdAsync(adminUser.Id)).ReturnsAsync(adminUser);

            var mockQueryable = assignments.AsQueryable().BuildMock();
            _mockAssignmentRepository.Setup(x => x.GetAll())
                .Returns(mockQueryable);

            // Act
            var result = await _assignmentService.GetAssignmentsAsync(adminUser.Id.ToString(), queryParams);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result.Items);
            Assert.Equal(1, result.PaginationMetadata.TotalItems);
        }

        [Fact]
        public async Task GetAssignmentsAsync_WithPagination_ReturnsCorrectPage()
        {
            // Arrange
            var adminUser = CreateAdminUser();

            var assignments = CreateAssignmentList(15, Location.HCM, AssignmentState.Accepted);

            var queryParams = new AssignmentQueryParameters
            {
                PageNumber = 2,
                PageSize = 5
            };

            _mockUserRepository.Setup(x => x.GetByIdAsync(adminUser.Id)).ReturnsAsync(adminUser);

            var mockQueryable = assignments.AsQueryable().BuildMock();
            _mockAssignmentRepository.Setup(x => x.GetAll())
                .Returns(mockQueryable);

            // Act
            var result = await _assignmentService.GetAssignmentsAsync(adminUser.Id.ToString(), queryParams);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(5, result.Items.Count());
            Assert.Equal(15, result.PaginationMetadata.TotalItems);
            Assert.Equal(2, result.PaginationMetadata.CurrentPage);
            Assert.Equal(3, result.PaginationMetadata.TotalPages);
        }

        [Fact]
        public async Task GetAssignmentsAsync_WithEmptyResult_ReturnsEmptyPagedResult()
        {
            // Arrange
            var adminUser = CreateAdminUser();

            var assignments = new List<Assignment>();
            var queryParams = new AssignmentQueryParameters();

            _mockUserRepository.Setup(x => x.GetByIdAsync(adminUser.Id))
                .ReturnsAsync(adminUser);

            var mockQueryable = assignments.AsQueryable().BuildMock();
            _mockAssignmentRepository.Setup(x => x.GetAll())
                .Returns(mockQueryable);

            // Act
            var result = await _assignmentService.GetAssignmentsAsync(adminUser.Id.ToString(), queryParams);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result.Items);
            Assert.Equal(0, result.PaginationMetadata.TotalItems);
        }
    }
}