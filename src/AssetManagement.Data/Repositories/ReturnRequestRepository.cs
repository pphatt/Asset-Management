using AssetManagement.Domain.Entities;
using AssetManagement.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace AssetManagement.Data.Repositories
{
    public class ReturnRequestRepository : GenericRepository<ReturnRequest>, IReturnRequestRepository
    {
        private readonly AssetManagementDbContext context;
        public ReturnRequestRepository(AssetManagementDbContext dbContext) : base(dbContext)
        {
            context = dbContext;
        }

        public async Task<ReturnRequest?> GetByIdAsync(Guid id)
        {
            return await context.ReturnRequests
                .Include(rr => rr.Assignment)
                .ThenInclude(a => a.Asset)
                .Include(rr => rr.Requester)
                .FirstOrDefaultAsync(rr => rr.Id == id);
        }
    }
}