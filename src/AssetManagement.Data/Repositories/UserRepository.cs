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
    public async Task<User?> GetByUsernameAsync(string username)
    {
        var user = await context.Users
            .FirstOrDefaultAsync(u => u.Username == username);
        return user;
    }
    
    public async Task<IEnumerable<string>> GetAllUsernamesAsync()
    {
        var userNames = await context.Users
            .Select(u => u.Username)
            .ToListAsync();
        
        if (userNames == null)
            throw new KeyNotFoundException("User not found");
        
        return userNames;
    }
    
    public async Task<IEnumerable<string>> GetStaffCodesAsync()
    {
       var staffCodes = await context.Users
            .Select(u => u.StaffCode)
            .ToListAsync();
       
        if (staffCodes == null)
            throw new KeyNotFoundException("Staff Code not found");
        
        return staffCodes;
    }
}