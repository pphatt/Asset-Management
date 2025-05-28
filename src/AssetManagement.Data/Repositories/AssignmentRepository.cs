using AssetManagement.Domain.Entities;
using AssetManagement.Domain.Interfaces.Repositories;

namespace AssetManagement.Data.Repositories
{
    public class AssignmentRepository : GenericRepository<Assignment>, IAssignmentRepository
    {
        public AssignmentRepository(AssetManagementDbContext dbContext) : base(dbContext) { }
    }
}