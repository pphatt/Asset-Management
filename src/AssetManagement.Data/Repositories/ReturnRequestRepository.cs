using AssetManagement.Domain.Entities;
using AssetManagement.Domain.Interfaces.Repositories;

namespace AssetManagement.Data.Repositories
{
    public class ReturnRequestRepository : GenericRepository<ReturnRequest>, IReturnRequestRepository
    {
        public ReturnRequestRepository(AssetManagementDbContext dbContext) : base(dbContext) { }
    }
}