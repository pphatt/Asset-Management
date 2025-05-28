using AssetManagement.Domain.Entities;
using AssetManagement.Domain.Interfaces.Repositories;

namespace AssetManagement.Data.Repositories
{
    public class CategoryRepository : GenericRepository<Category>, ICategoryRepository
    {
        public CategoryRepository(AssetManagementDbContext dbContext) : base(dbContext) { }
    }
}