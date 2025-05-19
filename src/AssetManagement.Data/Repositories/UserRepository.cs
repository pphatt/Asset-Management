using AssetManagement.Domain.Entities;
using AssetManagement.Domain.Repositories;

namespace AssetManagement.Data.Repositories
{
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        public UserRepository(AssetManagementDbContext dbContext) : base(dbContext)
        {

        }
    }
}