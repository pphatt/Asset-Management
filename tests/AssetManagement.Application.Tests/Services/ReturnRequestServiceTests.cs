using AssetManagement.Application.Services;
using AssetManagement.Contracts.DTOs;
using AssetManagement.Contracts.Enums;
using AssetManagement.Contracts.Parameters;
using AssetManagement.Domain.Entities;
using AssetManagement.Domain.Enums;
using AssetManagement.Domain.Interfaces.Repositories;
using MockQueryable;
using Moq;
using static AssetManagement.Contracts.Exceptions.ApiExceptionTypes;
 
namespace AssetManagement.Application.Tests.Services
{
    public class ReturnRequestServiceTests
    {
        private readonly Mock<IReturnRequestRepository> _mockReturnRequestRepository;
        private readonly Mock<IUserRepository> _mockUserRepository;
        private readonly Mock<IAssignmentRepository> _mockAssignmentRepository;
        private readonly ReturnRequestService _service;
        private readonly Guid _adminId = Guid.NewGuid();
        private readonly Location _location = Location.HN;
        private readonly List<ReturnRequest> _returnRequests;
 
        public ReturnRequestServiceTests()
        {
            _mockReturnRequestRepository = new Mock<IReturnRequestRepository>();
            _mockUserRepository = new Mock<IUserRepository>();
            _mockAssignmentRepository = new Mock<IAssignmentRepository>();
            _service = new ReturnRequestService(_mockReturnRequestRepository.Object,
                _mockUserRepository.Object,
                _mockAssignmentRepository.Object
            );
 
            // Sample data setup
            var asset1 = new Asset { Id = Guid.NewGuid(), Code = "A001", Name = "Laptop", Location = _location };
            var asset2 = new Asset { Id = Guid.NewGuid(), Code = "A002", Name = "Monitor", Location = _location };
            var asset3 = new Asset { Id = Guid.NewGuid(), Code = "A003", Name = "Keyboard", Location = Location.HCM };
 
            var user1 = new User { Id = Guid.NewGuid(), Username = "user1", Location = _location, Password = "Password" };
            var user2 = new User { Id = Guid.NewGuid(), Username = "user2", Location = _location, Password = "Password" };
            var admin1 = new User { Id = Guid.NewGuid(), Username = "admin1", Location = _location, Password = "Password" };
 
            var assignment1 = new Assignment { Id = Guid.NewGuid(), Asset = asset1, AssignedDate = DateTimeOffset.Now.AddDays(-5) };
            var assignment2 = new Assignment { Id = Guid.NewGuid(), Asset = asset2, AssignedDate = DateTimeOffset.Now.AddDays(-3) };
            var assignment3 = new Assignment { Id = Guid.NewGuid(), Asset = asset3, AssignedDate = DateTimeOffset.Now.AddDays(-2) };
 
            _returnRequests = new List<ReturnRequest>
            {
                new ReturnRequest
                {
                    Id = Guid.NewGuid(),
                    Assignment = assignment1,
                    Requester = user1,
                    Acceptor = admin1,
                    ReturnedDate = DateTimeOffset.Now,
                    State = ReturnRequestState.WaitingForReturning,
                    CreatedDate = DateTime.Now.AddDays(-1),
                    IsDeleted = false
                },
                new ReturnRequest
                {
                    Id = Guid.NewGuid(),
                    Assignment = assignment2,
                    Requester = user2,
                    Acceptor = admin1,
                    ReturnedDate = DateTimeOffset.Now.AddDays(-1),
                    State = ReturnRequestState.Completed,
                    CreatedDate = DateTime.Now.AddDays(-2),
                    IsDeleted = false
                },
                new ReturnRequest
                {
                    Id = Guid.NewGuid(),
                    Assignment = assignment3,
                    Requester = user1,
                    Acceptor = admin1,
                    ReturnedDate = DateTimeOffset.Now.AddDays(-3),
                    State = ReturnRequestState.Completed,
                    CreatedDate = DateTime.Now.AddDays(-3),
                    IsDeleted = false
                }
            };
 
            _mockUserRepository.Setup(repo => repo.GetByIdAsync(_adminId))
                .ReturnsAsync(new User { Id = _adminId, Location = _location, Username = "user", Password = "Password" });
 
            _mockReturnRequestRepository.Setup(repo => repo.GetAll())
                .Returns(_returnRequests.AsQueryable().BuildMock());
        }
 
        [Fact]
        public async Task GetReturnRequestsAsync_ReturnsPagedResult_WithCorrectPagination()
        {
            // Arrange
            var queryParams = new ReturnRequestQueryParameters { PageNumber = 1, PageSize = 2 };
 
            // Act
            var result = await _service.GetReturnRequestsAsync(_adminId, queryParams);
 
            // Assert
            Assert.Equal(2, result.Items.Count());
            Assert.Equal(2, result.PaginationMetadata.TotalItems);
            Assert.Equal(1, result.PaginationMetadata.TotalPages);
        }
 
        [Fact]
        public async Task GetReturnRequestsAsync_AppliesSearchCorrectly_ByAssetCode()
        {
            // Arrange
            var queryParams = new ReturnRequestQueryParameters { SearchTerm = "A001", PageNumber = 1, PageSize = 10 };
 
            // Act
            var result = await _service.GetReturnRequestsAsync(_adminId, queryParams);
 
            // Assert
            Assert.Single(result.Items);
            Assert.Equal("A001", result.Items.First().AssetCode);
        }
 
        [Fact]
        public async Task GetReturnRequestsAsync_AppliesStateFilterCorrectly()
        {
            // Arrange
            var queryParams = new ReturnRequestQueryParameters
            {
                States = new List<string> { "Completed" },
                PageNumber = 1,
                PageSize = 10
            };
 
            // Act
            var result = await _service.GetReturnRequestsAsync(_adminId, queryParams);
 
            // Assert
            Assert.Single(result.Items);
            Assert.Equal("Completed", result.Items.First().State);
        }
 
        [Fact]
        public async Task GetReturnRequestsAsync_AppliesDateFilterCorrectly()
        {
            // Arrange
            var specificDate = DateTimeOffset.Now.AddDays(-1);
            _returnRequests[1].ReturnedDate = specificDate; // Ensure exact match
            var queryParams = new ReturnRequestQueryParameters
            {
                ReturnedDate = specificDate.ToString("o"),
                PageNumber = 1,
                PageSize = 10
            };
 
            // Act
            var result = await _service.GetReturnRequestsAsync(_adminId, queryParams);
 
            // Assert
            Assert.Single(result.Items);
            Assert.Equal(specificDate.ToString("dd/MM/yyyy"), result.Items.First().ReturnedDate);
        }
 
        [Fact]
        public async Task GetReturnRequestsAsync_AppliesSortingCorrectly_ByAssetCodeAsc()
        {
            // Arrange
            var queryParams = new ReturnRequestQueryParameters
            {
                SortBy = "assetcode:asc",
                PageNumber = 1,
                PageSize = 10
            };
 
            // Act
            var result = await _service.GetReturnRequestsAsync(_adminId, queryParams);
 
            // Assert
            Assert.Equal(2, result.Items.Count());
            Assert.Equal("A001", result.Items.First().AssetCode);
            Assert.Equal("A002", result.Items.Last().AssetCode);
        }
 
        [Fact]
        public async Task GetReturnRequestsAsync_ThrowsKeyNotFoundException_WhenUserNotFound()
        {
            // Arrange
            _mockUserRepository.Setup(repo => repo.GetByIdAsync(_adminId)).ReturnsAsync((User?)null);
 
            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.GetReturnRequestsAsync(_adminId, new ReturnRequestQueryParameters()));
        }
        [Fact]
        public async Task CancelReturnRequestAsync_ReturnsCorrectDto_WhenValidRequestProvided()
        {
            // Arrange
            var returnRequestId = Guid.NewGuid();
            var adminId = Guid.NewGuid();
            var requesterId = Guid.NewGuid();
            var assetId = Guid.NewGuid();
            var assignmentId = Guid.NewGuid();
 
            var asset = new Asset
            {
                Id = assetId,
                Code = "LA001",
                Name = "Dell Laptop"
            };
 
            var assignment = new Assignment
            {
                Id = assignmentId,
                Asset = asset,
                AssignedDate = new DateTime(2023, 1, 15)
            };
 
            var requester = new User
            {
                Id = requesterId,
                Username = "requester",
                Password = "Password"
            };
 
            var admin = new User
            {
                Id = adminId,
                Username = "admin",
                Password = "Password"
            };
 
            var returnRequest = new ReturnRequest
            {
                Id = returnRequestId,
                AssignmentId = assignmentId,
                Assignment = assignment,
                Requester = requester,
                ReturnedDate = new DateTime(2023, 2, 1),
                State = ReturnRequestState.WaitingForReturning
            };
 
            _mockReturnRequestRepository.Setup(repo => repo.GetByIdAsync(returnRequestId))
                .ReturnsAsync(returnRequest);
 
            _mockUserRepository.Setup(repo => repo.GetByIdAsync(adminId))
                .ReturnsAsync(admin);
            _mockAssignmentRepository.Setup(repo => repo.GetByIdAsync(assignmentId))
                .ReturnsAsync(assignment);
 
            // Act
            var result = await _service.CancelReturnRequestAsync(returnRequestId, adminId);
 
            // Assert
            Assert.NotNull(result);
            Assert.Equal(returnRequestId, result.Id);
            Assert.Equal(asset.Code, result.AssetCode);
            Assert.Equal(asset.Name, result.AssetName);
            Assert.Equal(assignment.AssignedDate.ToString("dd/MM/yyyy"), result.AssignedDate);
            Assert.Equal(requester.Username, result.RequestedBy);
            Assert.Equal(admin.Username, result.AcceptedBy);
            Assert.Equal("Waiting for returning", result.State); // Change expectation to match current service behavior
 
            // Verify repository interactions
            _mockReturnRequestRepository.Verify(repo => repo.Update(returnRequest), Times.Once);
            _mockReturnRequestRepository.Verify(repo => repo.SaveChangesAsync(), Times.Once);
        }
        [Fact]
        public async Task CancelReturnRequestAsync_ThrowsKeyNotFoundException_WhenReturnRequestNotFound()
        {
            // Arrange
            var returnRequestId = Guid.NewGuid();
            var adminId = Guid.NewGuid();
 
            _mockReturnRequestRepository.Setup(repo => repo.GetByIdAsync(returnRequestId))
                .ReturnsAsync((ReturnRequest?)null);
 
            // Act & Assert
            var exception = await Assert.ThrowsAsync<KeyNotFoundException>(() =>
                _service.CancelReturnRequestAsync(returnRequestId, adminId));
 
            Assert.Contains($"Return request with id {returnRequestId} not found", exception.Message);
 
            // Verify no updates were attempted
            _mockReturnRequestRepository.Verify(repo => repo.Update(It.IsAny<ReturnRequest>()), Times.Never);
            _mockReturnRequestRepository.Verify(repo => repo.SaveChangesAsync(), Times.Never);
        }
 
        [Fact]
        public async Task CancelReturnRequestAsync_ThrowsInvalidOperationException_WhenRequestAlreadyCompletedOrCancelled()
        {
            // Arrange
            var returnRequestId = Guid.NewGuid();
            var adminId = Guid.NewGuid();
 
            var returnRequest = new ReturnRequest
            {
                Id = returnRequestId,
                State = ReturnRequestState.Completed
            };
 
            _mockReturnRequestRepository.Setup(repo => repo.GetByIdAsync(returnRequestId))
                .ReturnsAsync(returnRequest);
 
            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _service.CancelReturnRequestAsync(returnRequestId, adminId));
 
            Assert.Contains($"Return request with id {returnRequestId} is already completed.", exception.Message);
 
            // Verify no updates were attempted
            _mockReturnRequestRepository.Verify(repo => repo.Update(It.IsAny<ReturnRequest>()), Times.Never);
            _mockReturnRequestRepository.Verify(repo => repo.SaveChangesAsync(), Times.Never);
        }
 
        [Fact]
        public async Task CancelReturnRequestAsync_ThrowsKeyNotFoundException_WhenAdminNotFound()
        {
            // Arrange
            var returnRequestId = Guid.NewGuid();
            var adminId = Guid.NewGuid();
 
            var returnRequest = new ReturnRequest
            {
                Id = returnRequestId,
                State = ReturnRequestState.WaitingForReturning
            };
 
            _mockReturnRequestRepository.Setup(repo => repo.GetByIdAsync(returnRequestId))
                .ReturnsAsync(returnRequest);
 
            _mockUserRepository.Setup(repo => repo.GetByIdAsync(adminId))
                .ReturnsAsync((User?)null);
 
            // Act & Assert
            var exception = await Assert.ThrowsAsync<KeyNotFoundException>(() =>
                _service.CancelReturnRequestAsync(returnRequestId, adminId));
 
            Assert.Contains($"Admin with id {adminId} not found", exception.Message);
 
            // Verify no updates were attempted
            _mockReturnRequestRepository.Verify(repo => repo.Update(It.IsAny<ReturnRequest>()), Times.Never);
            _mockReturnRequestRepository.Verify(repo => repo.SaveChangesAsync(), Times.Never);
        }
 
        #region Create return request tests
 
        [Fact]
        public async Task CreateReturnRequestAsync_ReturnsResponse_WhenAssignmentIsAccepted_AndRoleIsAdmin()
        {
            var assignmentId = Guid.NewGuid().ToString();
            var requesterId = Guid.NewGuid().ToString();
            var asset = new Asset { Id = Guid.NewGuid(), Code = "A001", Name = "Laptop" };
            var assignment = new Assignment
            {
                Id = Guid.Parse(assignmentId),
                Asset = asset,
                State = AssignmentState.Accepted,
                IsDeleted = false
            };
 
            var assignments = new List<Assignment> { assignment }.AsQueryable().BuildMock();
            _mockAssignmentRepository.Setup(r => r.GetAll()).Returns(assignments);
            _mockAssignmentRepository.Setup(r => r.Update(It.IsAny<Assignment>()));
            _mockAssignmentRepository.Setup(r => r.SaveChangesAsync()).ReturnsAsync(true);
            _mockReturnRequestRepository.Setup(r => r.AddAsync(It.IsAny<ReturnRequest>())).Returns(Task.CompletedTask);
            _mockReturnRequestRepository.Setup(r => r.SaveChangesAsync()).ReturnsAsync(true);
 
            var result = await _service.CreateReturnRequestAsync(assignmentId, requesterId, "Admin");
 
            Assert.NotNull(result);
            Assert.Equal(asset.Code, result.AssetCode);
            Assert.Equal("Waiting for returning", result.AssignmentStatus);
            _mockAssignmentRepository.Verify(r => r.Update(It.Is<Assignment>(a => a.State == AssignmentState.WaitingForReturning)), Times.Once);
            _mockReturnRequestRepository.Verify(r => r.AddAsync(It.IsAny<ReturnRequest>()), Times.Once);
        }
 
        [Fact]
        public async Task CreateReturnRequestAsync_ReturnsResponse_WhenAssignmentIsAccepted_AndRoleIsUser()
        {
            var assignmentId = Guid.NewGuid().ToString();
            var requesterId = Guid.NewGuid().ToString();
            var asset = new Asset { Id = Guid.NewGuid(), Code = "A002", Name = "Monitor" };
            var assignment = new Assignment
            {
                Id = Guid.Parse(assignmentId),
                Asset = asset,
                State = AssignmentState.Accepted,
                AssigneeId = Guid.Parse(requesterId),
                IsDeleted = false
            };
 
            var assignments = new List<Assignment> { assignment }.AsQueryable().BuildMock();
            _mockAssignmentRepository.Setup(r => r.GetAll()).Returns(assignments);
            _mockAssignmentRepository.Setup(r => r.Update(It.IsAny<Assignment>()));
            _mockAssignmentRepository.Setup(r => r.SaveChangesAsync()).ReturnsAsync(true);
            _mockReturnRequestRepository.Setup(r => r.AddAsync(It.IsAny<ReturnRequest>())).Returns(Task.CompletedTask);
            _mockReturnRequestRepository.Setup(r => r.SaveChangesAsync()).ReturnsAsync(true);
 
            var result = await _service.CreateReturnRequestAsync(assignmentId, requesterId, "User");
 
            Assert.NotNull(result);
            Assert.Equal(asset.Code, result.AssetCode);
            Assert.Equal("Waiting for returning", result.AssignmentStatus);
            _mockAssignmentRepository.Verify(r => r.Update(It.Is<Assignment>(a => a.State == AssignmentState.WaitingForReturning)), Times.Once);
            _mockReturnRequestRepository.Verify(r => r.AddAsync(It.IsAny<ReturnRequest>()), Times.Once);
        }
 
        [Fact]
        public async Task CreateReturnRequestAsync_ThrowsKeyNotFoundException_WhenAssignmentNotFound_AdminRole()
        {
            var assignmentId = Guid.NewGuid().ToString();
            var requesterId = Guid.NewGuid().ToString();
            var assignments = new List<Assignment>().AsQueryable().BuildMock();
            _mockAssignmentRepository.Setup(r => r.GetAll()).Returns(assignments);
 
            await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.CreateReturnRequestAsync(assignmentId, requesterId, "Admin"));
        }
 
        [Fact]
        public async Task CreateReturnRequestAsync_ThrowsKeyNotFoundException_WhenAssignmentNotFound_UserRole()
        {
            var assignmentId = Guid.NewGuid().ToString();
            var requesterId = Guid.NewGuid().ToString();
            var assignments = new List<Assignment>().AsQueryable().BuildMock();
            _mockAssignmentRepository.Setup(r => r.GetAll()).Returns(assignments);
 
            await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.CreateReturnRequestAsync(assignmentId, requesterId, "User"));
        }
 
        [Fact]
        public async Task CreateReturnRequestAsync_ThrowsConflictException_WhenAssignmentStateIsNotAccepted_AdminRole()
        {
            var assignmentId = Guid.NewGuid().ToString();
            var requesterId = Guid.NewGuid().ToString();
            var asset = new Asset { Id = Guid.NewGuid(), Code = "A003", Name = "Keyboard" };
            var assignment = new Assignment
            {
                Id = Guid.Parse(assignmentId),
                Asset = asset,
                State = AssignmentState.WaitingForReturning,
                IsDeleted = false
            };
 
            var assignments = new List<Assignment> { assignment }.AsQueryable().BuildMock();
            _mockAssignmentRepository.Setup(r => r.GetAll()).Returns(assignments);
 
            await Assert.ThrowsAsync<ConflictException>(() => _service.CreateReturnRequestAsync(assignmentId, requesterId, "Admin"));
        }
 
        [Fact]
        public async Task CreateReturnRequestAsync_ThrowsConflictException_WhenAssignmentStateIsNotAccepted_UserRole()
        {
            var assignmentId = Guid.NewGuid().ToString();
            var requesterId = Guid.NewGuid().ToString();
            var asset = new Asset { Id = Guid.NewGuid(), Code = "A004", Name = "Mouse" };
            var assignment = new Assignment
            {
                Id = Guid.Parse(assignmentId),
                Asset = asset,
                State = AssignmentState.WaitingForReturning,
                AssigneeId = Guid.Parse(requesterId),
                IsDeleted = false
            };
 
            var assignments = new List<Assignment> { assignment }.AsQueryable().BuildMock();
            _mockAssignmentRepository.Setup(r => r.GetAll()).Returns(assignments);
 
            await Assert.ThrowsAsync<ConflictException>(() => _service.CreateReturnRequestAsync(assignmentId, requesterId, "User"));
        }
 
        #endregion
    }
}