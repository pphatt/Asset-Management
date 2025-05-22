using AssetManagement.Domain.Entities;
using AssetManagement.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace AssetManagement.Data.Repositories;

public class UserRepository : GenericRepository<User>, IUserRepository
{
    private readonly AssetManagementDbContext context;

    public UserRepository(AssetManagementDbContext context) : base(context)
    {
        this.context = context;
    }
    public async Task<User?> GetByUsernameAsync(string username) =>
    await context.Users.FirstOrDefaultAsync(u => u.Username == username);
}