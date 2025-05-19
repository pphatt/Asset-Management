using AssetManagement.Domain.Entities;

namespace AssetManagement.Domain.Repositories;

public interface IUserRepository : IGenericRepository<User>
{
    Task<User> GetByUsernameAsync(string username);
}
