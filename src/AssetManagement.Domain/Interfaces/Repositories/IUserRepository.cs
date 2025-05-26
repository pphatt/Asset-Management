using AssetManagement.Domain.Entities;

namespace AssetManagement.Domain.Interfaces.Repositories
{
    public interface IUserRepository : IGenericRepository<User>
    {
        Task<User?> GetByUsernameAsync(string username);
        Task<User?> GetByStaffCodeAsync(string staffCode);
        Task<IEnumerable<string>> GetAllUsernamesAsync();
        Task<IEnumerable<string>> GetStaffCodesAsync();
    }
}
