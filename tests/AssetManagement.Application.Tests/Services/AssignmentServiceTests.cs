using AssetManagement.Application.Extensions;
using AssetManagement.Application.Services;
using AssetManagement.Contracts.DTOs.Requests;
using AssetManagement.Contracts.Exceptions;
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
        private readonly Mock<IAssetRepository> _mockAssetRepository;
        private readonly Mock<IUserRepository> _mockUserRepository;
        private readonly AssignmentService _assignmentService;

        public AssignmentServiceTests()
        {
            _mockAssignmentRepository = new Mock<IAssignmentRepository>();
            _mockAssetRepository = new Mock<IAssetRepository>();
            _mockUserRepository = new Mock<IUserRepository>();
            _assignmentService = new AssignmentService(_mockAssignmentRepository.Object, _mockAssetRepository.Object, _mockUserRepository.Object);
        }

        private User CreateAdminUser(Guid? id = null)
        {
            return new User
            {
                Id = id ?? Guid.NewGuid(),
                Username = "admin1",
                Password = "Password",
                Location = Location.HCM
            };
        }

        private User CreateStaffUser(Guid? id = null)
        {
            return new User
            {
                Id = id ?? Guid.NewGuid(),
                Username = "staff1",
                Password = "Password",
                Location = Location.HCM
            };
        }

        private Assignment CreateAssignment(Guid assignmentId, Guid assetId, string assetCode, string assetName, Location location,
                                            Guid assignorId, string assignorUsername, Guid assigneeId, string assigneeUsername,
                                            AssignmentState state)
        {
            return new Assignment
            {
                Id = assignmentId,
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
                CreateAssignment(Guid.NewGuid(), Guid.NewGuid(), "LP001", "Laptop Dell", Location.HCM, Guid.NewGuid(), "admin1", Guid.NewGuid(), "user1", AssignmentState.Accepted),
                CreateAssignment(Guid.NewGuid(), Guid.NewGuid(), "MO001", "Monitor LG", Location.HCM, Guid.NewGuid(), "admin2", Guid.NewGuid(), "user2", AssignmentState.WaitingForAcceptance)
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
                CreateAssignment(Guid.NewGuid(), Guid.NewGuid(), "LP001", "Laptop Dell", Location.HCM, Guid.NewGuid(), "admin1", Guid.NewGuid(), "johndoe", AssignmentState.Accepted),
                CreateAssignment(Guid.NewGuid(), Guid.NewGuid(), "MO001", "Monitor LG", Location.HCM, Guid.NewGuid(), "admin2", Guid.NewGuid(), "janesmith", AssignmentState.WaitingForAcceptance)
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

        [Fact]
        public async Task GetAssignmentByIdAsync_WhenAssignmentExists_ReturnsAssignmentDto()
        {
            // Arrange
            var assignmentId = Guid.NewGuid();
            var assignment = CreateAssignment(assignmentId, Guid.NewGuid(), "LP001", "Laptop Dell", Location.HCM, Guid.NewGuid(), "admin1", Guid.NewGuid(), "user1", AssignmentState.Accepted);
            _mockAssignmentRepository.Setup(x => x.GetByIdAsync(assignmentId)).ReturnsAsync(assignment);

            // Act
            var result = await _assignmentService.GetAssignmentByIdAsync(assignmentId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(assignmentId, result.Id);
            Assert.Equal("LP001", result.AssetCode);
            Assert.Equal("Laptop Dell", result.AssetName);
            Assert.Equal("admin1", result.AssignedBy);
            Assert.Equal("user1", result.AssignedTo);
            Assert.Equal(assignment.AssignedDate.ToString("dd/MM/yyyy"), result.AssignedDate);
            Assert.Equal(assignment.State.GetDisplayName(), result.State);
        }

        [Fact]
        public async Task GetAssignmentByIdAsync_WhenAssignmentDoesNotExist_ReturnsNull()
        {
            // Arrange
            var assignmentId = Guid.NewGuid();
            _mockAssignmentRepository.Setup(x => x.GetByIdAsync(assignmentId)).ReturnsAsync((Assignment?)null);

            // Act
            var result = await _assignmentService.GetAssignmentByIdAsync(assignmentId);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task CreateAssignmentAsync_WhenValidationPasses_CreatesAssignment()
        {
            // Arrange
            var adminId = Guid.NewGuid();
            var assetId = Guid.NewGuid();
            var assigneeId = Guid.NewGuid();
            var assignedDate = DateTimeOffset.Now.AddDays(1).ToString("yyyy/MM/dd");

            var dto = new CreateAssignmentRequestDto
            {
                AssetId = assetId.ToString(),
                AssigneeId = assigneeId.ToString(),
                AssignedDate = assignedDate,
                Note = "Test assignment"
            };

            var admin = CreateAdminUser(adminId);
            var assignee = CreateStaffUser(assigneeId);
            var asset = new Asset { Id = assetId, State = AssetState.Available, Location = Location.HCM };

            _mockUserRepository.Setup(x => x.GetByIdAsync(adminId)).ReturnsAsync(admin);
            _mockAssetRepository.Setup(x => x.GetByIdAsync(assetId)).ReturnsAsync(asset);
            _mockUserRepository.Setup(x => x.GetByIdAsync(assigneeId)).ReturnsAsync(assignee);

            // Mock no existing assignments for this asset
            var mockAssignmentQueryable = new List<Assignment>().AsQueryable().BuildMock();
            _mockAssignmentRepository.Setup(x => x.GetAll()).Returns(mockAssignmentQueryable);

            // Capture the added assignment
            Assignment? capturedAssignment = null;
            _mockAssignmentRepository.Setup(x => x.AddAsync(It.IsAny<Assignment>()))
                .Callback<Assignment>(a => capturedAssignment = a);
            _mockAssignmentRepository.Setup(x => x.SaveChangesAsync()).ReturnsAsync(true);

            // Mock GetAssignmentByIdAsync to return the created assignment
            _mockAssignmentRepository.Setup(x => x.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync((object[] keyValues) =>
                {
                    var id = (Guid)keyValues[0];
                    return new Assignment
                    {
                        Id = id,
                        AssetId = assetId,
                        Asset = asset,
                        AssignorId = adminId,
                        Assignor = admin,
                        AssigneeId = assigneeId,
                        Assignee = assignee,
                        AssignedDate = DateTimeOffset.Parse(assignedDate),
                        State = AssignmentState.WaitingForAcceptance,
                        Note = dto.Note
                    };
                });

            // Act
            var result = await _assignmentService.CreateAssignmentAsync(adminId, dto);

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(capturedAssignment);
            Assert.Equal(assetId, capturedAssignment.AssetId);
            Assert.Equal(assigneeId, capturedAssignment.AssigneeId);
            Assert.Equal(adminId, capturedAssignment.AssignorId);
            Assert.Equal(AssignmentState.WaitingForAcceptance, capturedAssignment.State);
            Assert.Equal(dto.Note, capturedAssignment.Note);
            Assert.Equal(DateTimeOffset.Parse(assignedDate), capturedAssignment.AssignedDate);
            Assert.Equal(adminId, capturedAssignment.CreatedBy);
            Assert.Equal(adminId, capturedAssignment.LastModifiedBy);
        }

        [Fact]
        public async Task CreateAssignmentAsync_WhenAssetNotFound_ThrowsAggregateFieldValidationException()
        {
            // Arrange
            var adminId = Guid.NewGuid();
            var assetId = Guid.NewGuid();
            var assigneeId = Guid.NewGuid();
            var assignedDate = DateTimeOffset.Now.AddDays(1).ToString("yyyy/MM/dd");

            var dto = new CreateAssignmentRequestDto
            {
                AssetId = assetId.ToString(),
                AssigneeId = assigneeId.ToString(),
                AssignedDate = assignedDate,
                Note = "Test assignment"
            };

            var admin = CreateAdminUser(adminId);
            var assignee = CreateStaffUser(assigneeId);
            _mockUserRepository.Setup(x => x.GetByIdAsync(adminId)).ReturnsAsync(admin);
            _mockUserRepository.Setup(x => x.GetByIdAsync(assigneeId)).ReturnsAsync(assignee);
            _mockAssetRepository.Setup(x => x.GetByIdAsync(assetId)).ReturnsAsync((Asset?)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<AggregateFieldValidationException>(
                () => _assignmentService.CreateAssignmentAsync(adminId, dto));

            Assert.Single(exception.Errors);
            Assert.Equal("AssetId", exception.Errors[0].Field);
            Assert.Equal("Asset not found", exception.Errors[0].Message);
        }
    }
}