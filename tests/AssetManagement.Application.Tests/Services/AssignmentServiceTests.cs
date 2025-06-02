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

        #region Helpers
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
        #endregion

        #region GetAssignments
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
            var result = await _assignmentService.GetAssignmentsAsync(adminUser.Id, queryParams);

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
            var adminId = Guid.NewGuid();
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
            var result = await _assignmentService.GetAssignmentsAsync(adminUser.Id, queryParams);

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
            var result = await _assignmentService.GetAssignmentsAsync(adminUser.Id, queryParams);

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
            var result = await _assignmentService.GetAssignmentsAsync(adminUser.Id, queryParams);

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

            _mockAssignmentRepository.Setup(x => x.GetAll())
                .Returns(new List<Assignment> { assignment }.AsQueryable().BuildMock());

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
            _mockAssignmentRepository.Setup(x => x.GetAll())
                .Returns(new List<Assignment>().AsQueryable().BuildMock());

            // Act
            var result = await _assignmentService.GetAssignmentByIdAsync(assignmentId);

            // Assert
            Assert.Null(result);
        }
        #endregion

        #region CreateAssignment
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
            _mockAssignmentRepository.Setup(x => x.GetAll())
                .Returns(() =>
                {
                    if (capturedAssignment != null)
                    {
                        // Create a list with the captured assignment that includes navigation properties
                        var assignmentWithNavProps = new Assignment
                        {
                            Id = capturedAssignment.Id,
                            AssetId = assetId,
                            Asset = asset,
                            AssignorId = adminId,
                            Assignor = admin,
                            AssigneeId = assigneeId,
                            Assignee = assignee,
                            AssignedDate = capturedAssignment.AssignedDate,
                            State = capturedAssignment.State,
                            Note = capturedAssignment.Note,
                            CreatedBy = capturedAssignment.CreatedBy,
                            CreatedDate = capturedAssignment.CreatedDate,
                            LastModifiedBy = capturedAssignment.LastModifiedBy,
                            LastModifiedDate = capturedAssignment.LastModifiedDate
                        };
                        return new List<Assignment> { assignmentWithNavProps }.AsQueryable().BuildMock();
                    }
                    return new List<Assignment>().AsQueryable().BuildMock();
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
        #endregion

        #region UpdateAssignment

        [Fact]
        public async Task UpdateAssignmentAsync_WhenValidationPasses_UpdatesAssignment()
        {
            // Arrange
            var adminId = Guid.NewGuid();
            var assignmentId = Guid.NewGuid();
            var originalAssetId = Guid.NewGuid();
            var newAssetId = Guid.NewGuid();
            var originalAssigneeId = Guid.NewGuid();
            var newAssigneeId = Guid.NewGuid();
            var newAssignedDate = DateTimeOffset.Now.AddDays(1).ToString("yyyy/MM/dd");
            var dto = new UpdateAssignmentRequestDto
            {
                AssetId = newAssetId.ToString(),
                AssigneeId = newAssigneeId.ToString(),
                AssignedDate = newAssignedDate,
                Note = "Updated note"
            };
            var admin = CreateAdminUser(adminId);
            var originalAsset = new Asset
            {
                Id = originalAssetId,
                Name = "Old asset",
                Code = "OLD001",
                State = AssetState.Available,
                Location = Location.HCM
            };
            var originalAssignee = CreateStaffUser(originalAssigneeId);
            var assignment = new Assignment
            {
                Id = assignmentId,
                AssetId = originalAssetId,
                Asset = originalAsset,
                AssigneeId = originalAssigneeId,
                Assignee = originalAssignee,
                AssignorId = adminId,
                Assignor = admin,
                State = AssignmentState.WaitingForAcceptance,
                AssignedDate = DateTimeOffset.Now,
                Note = "Original note",
                LastModifiedBy = adminId,
                LastModifiedDate = DateTime.UtcNow.AddDays(-1)
            };
            var newAsset = new Asset
            {
                Id = newAssetId,
                Name = "New asset",
                Code = "NEW001",
                State = AssetState.Available,
                Location = Location.HCM
            };
            var newAssignee = CreateStaffUser(newAssigneeId);

            _mockAssignmentRepository.Setup(x => x.GetByIdAsync(assignmentId)).ReturnsAsync(assignment);
            _mockUserRepository.Setup(x => x.GetByIdAsync(adminId)).ReturnsAsync(admin);
            _mockAssetRepository.Setup(x => x.GetByIdAsync(newAssetId)).ReturnsAsync(newAsset);
            _mockUserRepository.Setup(x => x.GetByIdAsync(newAssigneeId)).ReturnsAsync(newAssignee);

            // Mock GetAll for validation (empty list)
            _mockAssignmentRepository.Setup(x => x.GetAll()).Returns(new List<Assignment>().AsQueryable().BuildMock());

            _mockAssignmentRepository.Setup(x => x.Update(It.IsAny<Assignment>()));
            _mockAssignmentRepository.Setup(x => x.SaveChangesAsync()).ReturnsAsync(true);

            // Mock GetAll for GetAssignmentByIdAsync after update
            _mockAssignmentRepository.SetupSequence(x => x.GetAll())
                .Returns(new List<Assignment>().AsQueryable().BuildMock()) // First call for validation
                .Returns(() =>
                {
                    // Second call for GetAssignmentByIdAsync - return updated assignment
                    var updatedAssignment = new Assignment
                    {
                        Id = assignmentId,
                        AssetId = newAssetId,
                        Asset = newAsset,
                        AssigneeId = newAssigneeId,
                        Assignee = newAssignee,
                        AssignorId = adminId,
                        Assignor = admin,
                        State = AssignmentState.WaitingForAcceptance,
                        AssignedDate = DateTimeOffset.Parse(newAssignedDate),
                        Note = "Updated note",
                        LastModifiedBy = adminId,
                        LastModifiedDate = DateTime.UtcNow
                    };
                    return new List<Assignment> { updatedAssignment }.AsQueryable().BuildMock();
                });

            // Act
            var result = await _assignmentService.UpdateAssignmentAsync(assignmentId, adminId, dto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(newAssetId, assignment.AssetId);
            Assert.Equal(newAssigneeId, assignment.AssigneeId);
            Assert.Equal(DateTimeOffset.Parse(newAssignedDate), assignment.AssignedDate);
            Assert.Equal("Updated note", assignment.Note);
            Assert.Equal(adminId, assignment.LastModifiedBy);
            Assert.True(assignment.LastModifiedDate > DateTime.UtcNow.AddSeconds(-10));

            // Additional assertions for the returned DTO
            Assert.Equal(assignmentId, result.Id);
            Assert.Equal("NEW001", result.AssetCode);
            Assert.Equal("New asset", result.AssetName);
            Assert.Equal(admin.Username, result.AssignedBy);
            Assert.Equal(newAssignee.Username, result.AssignedTo);

            _mockAssignmentRepository.Verify(x => x.Update(assignment), Times.Once);
            _mockAssignmentRepository.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task UpdateAssignmentAsync_AssignmentNotFound_ThrowsKeyNotFoundException()
        {
            // Arrange
            var adminId = Guid.NewGuid();
            var assignmentId = Guid.NewGuid();
            var dto = new UpdateAssignmentRequestDto();

            _mockAssignmentRepository.Setup(x => x.GetByIdAsync(assignmentId)).ReturnsAsync((Assignment?)null);

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() =>
                _assignmentService.UpdateAssignmentAsync(assignmentId, adminId, dto));
        }

        [Fact]
        public async Task UpdateAssignmentAsync_InvalidState_ThrowsInvalidOperationException()
        {
            // Arrange
            var adminId = Guid.NewGuid();
            var assignmentId = Guid.NewGuid();
            var assignment = new Assignment
            {
                Id = assignmentId,
                State = AssignmentState.Accepted,
                AssignorId = adminId
            };
            var dto = new UpdateAssignmentRequestDto();

            _mockAssignmentRepository.Setup(x => x.GetByIdAsync(assignmentId)).ReturnsAsync(assignment);
            _mockUserRepository.Setup(x => x.GetByIdAsync(adminId)).ReturnsAsync(CreateAdminUser(adminId));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _assignmentService.UpdateAssignmentAsync(assignmentId, adminId, dto));
            Assert.Equal("Can only edit assignments with state 'Waiting for acceptance'", exception.Message);
        }

        [Fact]
        public async Task UpdateAssignmentAsync_InvalidAsset_ThrowsAggregateFieldValidationException()
        {
            // Arrange
            var adminId = Guid.NewGuid();
            var assignmentId = Guid.NewGuid();
            var invalidAssetId = Guid.NewGuid();
            var assignment = new Assignment
            {
                Id = assignmentId,
                State = AssignmentState.WaitingForAcceptance,
                AssignorId = adminId,
                AssetId = Guid.NewGuid()
            };
            var dto = new UpdateAssignmentRequestDto { AssetId = invalidAssetId.ToString() };

            _mockAssignmentRepository.Setup(x => x.GetByIdAsync(assignmentId)).ReturnsAsync(assignment);
            _mockUserRepository.Setup(x => x.GetByIdAsync(adminId)).ReturnsAsync(CreateAdminUser(adminId));
            _mockAssetRepository.Setup(x => x.GetByIdAsync(invalidAssetId)).ReturnsAsync((Asset?)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<AggregateFieldValidationException>(() =>
                _assignmentService.UpdateAssignmentAsync(assignmentId, adminId, dto));
            Assert.Contains("Asset not found", exception.Errors.Select(e => e.Message));
        }
        #endregion

        #region DeleteAssignment
        [Fact]
        public async Task DeleteAssignmentAsync_WhenAssignmentExistsAndIsWaitingForAcceptance_ShouldDeleteSuccessfully()
        {
            // Arrange
            var adminId = Guid.NewGuid();
            var admin = CreateAdminUser(adminId);
            var assetId = Guid.NewGuid();
            var asset = new Asset { Id = assetId, Location = admin.Location, State = AssetState.Assigned };
            var assignmentId = Guid.NewGuid();
            var assignment = CreateAssignment(assignmentId, assetId, "LP001", "Laptop 1", admin.Location, adminId, "admin1", Guid.NewGuid(), "user1", AssignmentState.WaitingForAcceptance);
            assignment.Asset = asset;

            var assignments = new List<Assignment> { assignment };
            var mockQueryable = assignments.AsQueryable().BuildMock();

            _mockAssignmentRepository.Setup(r => r.GetAll()).Returns(mockQueryable);
            _mockUserRepository.Setup(r => r.GetByIdAsync(adminId)).ReturnsAsync(admin);
            _mockAssetRepository.Setup(r => r.GetByIdAsync(assetId)).ReturnsAsync(asset);
            _mockAssignmentRepository.Setup(r => r.Update(It.IsAny<Assignment>())).Verifiable();
            _mockAssetRepository.Setup(r => r.Update(It.IsAny<Asset>())).Verifiable();
            _mockAssetRepository.Setup(r => r.SaveChangesAsync()).ReturnsAsync(true);

            // Act
            var result = await _assignmentService.DeleteAssignmentAsync(assignmentId, adminId);

            // Assert
            Assert.True(result);
            Assert.True(assignment.IsDeleted);
            Assert.Equal(adminId, assignment.DeletedBy);
            Assert.NotNull(assignment.DeletedDate);
            Assert.Equal(AssetState.Available, asset.State);
            _mockAssignmentRepository.Verify(r => r.Update(assignment), Times.Once());
            _mockAssetRepository.Verify(r => r.Update(asset), Times.Once());
        }

        [Fact]
        public async Task DeleteAssignmentAsync_WhenAssignmentNotFound_ShouldReturnFalse()
        {
            // Arrange
            var adminId = Guid.NewGuid();
            var admin = CreateAdminUser(adminId);
            var assignmentId = Guid.NewGuid();
            var assignments = new List<Assignment>();
            var mockQueryable = assignments.AsQueryable().BuildMock();

            _mockAssignmentRepository.Setup(r => r.GetAll()).Returns(mockQueryable);
            _mockUserRepository.Setup(r => r.GetByIdAsync(adminId)).ReturnsAsync(admin);

            // Act
            var result = await _assignmentService.DeleteAssignmentAsync(assignmentId, adminId);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task DeleteAssignmentAsync_WhenLocationsDoNotMatch_ShouldReturnFalse()
        {
            // Arrange
            var adminId = Guid.NewGuid();
            var admin = CreateAdminUser(adminId);
            var assetId = Guid.NewGuid();
            var asset = new Asset { Id = assetId, Location = Location.HN, State = AssetState.Assigned };
            var assignmentId = Guid.NewGuid();
            var assignment = CreateAssignment(assignmentId, assetId, "LP001", "Laptop 1", Location.HN, adminId, "admin1", Guid.NewGuid(), "user1", AssignmentState.WaitingForAcceptance);
            assignment.Asset = asset;

            var assignments = new List<Assignment> { assignment };
            var mockQueryable = assignments.AsQueryable().BuildMock();

            _mockAssignmentRepository.Setup(r => r.GetAll()).Returns(mockQueryable);
            _mockUserRepository.Setup(r => r.GetByIdAsync(adminId)).ReturnsAsync(admin);

            // Act
            var result = await _assignmentService.DeleteAssignmentAsync(assignmentId, adminId);

            // Assert
            Assert.False(result);
            Assert.True(assignment.IsDeleted == null || assignment.IsDeleted.Equals(false));
        }

        [Fact]
        public async Task DeleteAssignmentAsync_WhenStateIsNotWaitingForAcceptance_ShouldThrowInvalidOperationException()
        {
            // Arrange
            var adminId = Guid.NewGuid();
            var admin = CreateAdminUser(adminId);
            var assetId = Guid.NewGuid();
            var asset = new Asset { Id = assetId, Location = admin.Location, State = AssetState.Assigned };
            var assignmentId = Guid.NewGuid();
            var assignment = CreateAssignment(assignmentId, assetId, "LP001", "Laptop 1", admin.Location, adminId, "admin1", Guid.NewGuid(), "user1", AssignmentState.Accepted);
            assignment.Asset = asset;

            var assignments = new List<Assignment> { assignment };
            var mockQueryable = assignments.AsQueryable().BuildMock();

            _mockAssignmentRepository.Setup(r => r.GetAll()).Returns(mockQueryable);
            _mockUserRepository.Setup(r => r.GetByIdAsync(adminId)).ReturnsAsync(admin);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _assignmentService.DeleteAssignmentAsync(assignmentId, adminId));
            Assert.Equal("Can only delete assignments that are waiting for acceptance", exception.Message);
        }
        #endregion

        #region AcceptAssignment
        [Fact]
        public async Task AcceptAssignmentAsync_WhenValid_ShouldAcceptAssignment()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var assetId = Guid.NewGuid();
            var asset = new Asset
            {
                Id = assetId,
                State = AssetState.Available,
                Code = "LP001",
                Name = "Laptop 1"
            };
            var assignmentId = Guid.NewGuid();
            var assignment = CreateAssignment(assignmentId, assetId, "LP001", "Laptop 1", Location.HCM, Guid.NewGuid(), "admin1", userId, "user1", AssignmentState.WaitingForAcceptance);
            assignment.Asset = asset;

            _mockAssignmentRepository.Setup(r => r.GetByIdAsync(assignmentId)).ReturnsAsync(assignment);
            _mockAssetRepository.Setup(r => r.GetByIdAsync(assetId)).ReturnsAsync(asset);
            _mockAssignmentRepository.Setup(r => r.Update(It.IsAny<Assignment>())).Verifiable();
            _mockAssetRepository.Setup(r => r.Update(It.IsAny<Asset>())).Verifiable();
            _mockAssignmentRepository.Setup(r => r.SaveChangesAsync()).ReturnsAsync(true);

            // Mock GetAll for GetAssignmentByIdAsync (if AcceptAssignmentAsync calls it)
            _mockAssignmentRepository.Setup(x => x.GetAll())
                .Returns(() =>
                {
                    var acceptedAssignment = new Assignment
                    {
                        Id = assignmentId,
                        AssetId = assetId,
                        Asset = asset,
                        AssigneeId = assignment.AssigneeId,
                        Assignee = assignment.Assignee,
                        AssignorId = assignment.AssignorId,
                        Assignor = assignment.Assignor,
                        State = AssignmentState.Accepted,
                        AssignedDate = assignment.AssignedDate,
                        Note = assignment.Note,
                        LastModifiedBy = userId,
                        LastModifiedDate = DateTime.UtcNow
                    };
                    return new List<Assignment> { acceptedAssignment }.AsQueryable().BuildMock();
                });

            // Act
            var result = await _assignmentService.AcceptAssignmentAsync(assignmentId, userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(AssignmentState.Accepted, assignment.State);
            Assert.Equal(AssetState.Assigned, asset.State);
            Assert.Equal(userId, assignment.LastModifiedBy);
            Assert.NotEqual(DateTime.MinValue, assignment.LastModifiedDate);
            _mockAssignmentRepository.Verify(r => r.Update(assignment), Times.Once());
            _mockAssetRepository.Verify(r => r.Update(asset), Times.Once());
        }

        [Fact]
        public async Task AcceptAssignmentAsync_WhenAssignmentNotFound_ShouldThrowKeyNotFoundException()
        {
            // Arrange
            var assignmentId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            _mockAssignmentRepository.Setup(r => r.GetByIdAsync(assignmentId)).ReturnsAsync((Assignment?)null);

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => _assignmentService.AcceptAssignmentAsync(assignmentId, userId));
        }

        [Fact]
        public async Task AcceptAssignmentAsync_WhenUserIsNotAssignee_ShouldThrowUnauthorizedAccessException()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var assigneeId = Guid.NewGuid();
            var assignmentId = Guid.NewGuid();
            var assignment = CreateAssignment(assignmentId, Guid.NewGuid(), "LP001", "Laptop 1", Location.HCM, Guid.NewGuid(), "admin1", assigneeId, "user1", AssignmentState.WaitingForAcceptance);

            _mockAssignmentRepository.Setup(r => r.GetByIdAsync(assignmentId)).ReturnsAsync(assignment);

            // Act & Assert
            await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _assignmentService.AcceptAssignmentAsync(assignmentId, userId));
        }

        [Fact]
        public async Task AcceptAssignmentAsync_WhenStateIsNotWaitingForAcceptance_ShouldThrowInvalidOperationException()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var assignmentId = Guid.NewGuid();
            var assignment = CreateAssignment(assignmentId, Guid.NewGuid(), "LP001", "Laptop 1", Location.HCM, Guid.NewGuid(), "admin1", userId, "user1", AssignmentState.Accepted);

            _mockAssignmentRepository.Setup(r => r.GetByIdAsync(assignmentId)).ReturnsAsync(assignment);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => _assignmentService.AcceptAssignmentAsync(assignmentId, userId));
        }
        #endregion

        #region DeclineAssignment
        [Fact]
        public async Task DeclineAssignmentAsync_WhenValid_ShouldDeclineAssignment()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var assetId = Guid.NewGuid();
            var asset = new Asset
            {
                Id = assetId,
                State = AssetState.Assigned,
                Code = "LP001",
                Name = "Laptop 1"
            };
            var assignmentId = Guid.NewGuid();
            var assignment = CreateAssignment(assignmentId, assetId, "LP001", "Laptop 1", Location.HCM, Guid.NewGuid(), "admin1", userId, "user1", AssignmentState.WaitingForAcceptance);
            assignment.Asset = asset;

            _mockAssignmentRepository.Setup(r => r.GetByIdAsync(assignmentId)).ReturnsAsync(assignment);
            _mockAssetRepository.Setup(r => r.GetByIdAsync(assetId)).ReturnsAsync(asset);
            _mockAssignmentRepository.Setup(r => r.Update(It.IsAny<Assignment>())).Verifiable();
            _mockAssetRepository.Setup(r => r.Update(It.IsAny<Asset>())).Verifiable();
            _mockAssignmentRepository.Setup(r => r.SaveChangesAsync()).ReturnsAsync(true);

            // Mock GetAll for GetAssignmentByIdAsync (if DeclineAssignmentAsync calls it)
            _mockAssignmentRepository.Setup(x => x.GetAll())
                .Returns(() =>
                {
                    var declinedAssignment = new Assignment
                    {
                        Id = assignmentId,
                        AssetId = assetId,
                        Asset = asset,
                        AssigneeId = assignment.AssigneeId,
                        Assignee = assignment.Assignee,
                        AssignorId = assignment.AssignorId,
                        Assignor = assignment.Assignor,
                        State = AssignmentState.Declined,
                        AssignedDate = assignment.AssignedDate,
                        Note = assignment.Note,
                        LastModifiedBy = userId,
                        LastModifiedDate = DateTime.UtcNow
                    };
                    return new List<Assignment> { declinedAssignment }.AsQueryable().BuildMock();
                });

            // Act
            var result = await _assignmentService.DeclineAssignmentAsync(assignmentId, userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(AssignmentState.Declined, assignment.State);
            Assert.Equal(AssetState.Available, asset.State);
            Assert.Equal(userId, assignment.LastModifiedBy);
            Assert.NotEqual(DateTime.MinValue, assignment.LastModifiedDate);
            _mockAssignmentRepository.Verify(r => r.Update(assignment), Times.Once());
            _mockAssetRepository.Verify(r => r.Update(asset), Times.Once());
        }

        [Fact]
        public async Task DeclineAssignmentAsync_WhenAssignmentNotFound_ShouldThrowKeyNotFoundException()
        {
            // Arrange
            var assignmentId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            _mockAssignmentRepository.Setup(r => r.GetByIdAsync(assignmentId)).ReturnsAsync((Assignment?)null);

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => _assignmentService.DeclineAssignmentAsync(assignmentId, userId));
        }

        [Fact]
        public async Task DeclineAssignmentAsync_WhenUserIsNotAssignee_ShouldThrowUnauthorizedAccessException()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var assigneeId = Guid.NewGuid();
            var assignmentId = Guid.NewGuid();
            var assignment = CreateAssignment(assignmentId, Guid.NewGuid(), "LP001", "Laptop 1", Location.HCM, Guid.NewGuid(), "admin1", assigneeId, "user1", AssignmentState.WaitingForAcceptance);

            _mockAssignmentRepository.Setup(r => r.GetByIdAsync(assignmentId)).ReturnsAsync(assignment);

            // Act & Assert
            await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _assignmentService.DeclineAssignmentAsync(assignmentId, userId));
        }

        [Fact]
        public async Task DeclineAssignmentAsync_WhenStateIsNotWaitingForAcceptance_ShouldThrowInvalidOperationException()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var assignmentId = Guid.NewGuid();
            var assignment = CreateAssignment(assignmentId, Guid.NewGuid(), "LP001", "Laptop 1", Location.HCM, Guid.NewGuid(), "admin1", userId, "user1", AssignmentState.Declined);

            _mockAssignmentRepository.Setup(r => r.GetByIdAsync(assignmentId)).ReturnsAsync(assignment);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => _assignmentService.DeclineAssignmentAsync(assignmentId, userId));
        }
        #endregion
    }
}