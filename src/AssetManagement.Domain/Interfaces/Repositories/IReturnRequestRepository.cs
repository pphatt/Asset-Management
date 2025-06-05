using AssetManagement.Domain.Entities;

namespace AssetManagement.Domain.Interfaces.Repositories
{
    public interface IReturnRequestRepository : IGenericRepository<ReturnRequest>
    {
        Task<ReturnRequest?> GetByIdAsync(Guid id);
    }
}