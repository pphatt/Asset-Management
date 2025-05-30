using AssetManagement.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace AssetManagement.Data.Repositories;

public class GenericRepository<T> : IGenericRepository<T> where T : class
{
    private readonly AssetManagementDbContext _dbContext;
    private DbSet<T> _dbSet;

    public GenericRepository(AssetManagementDbContext dbContext)
    {
        _dbContext = dbContext;
        _dbSet = dbContext.Set<T>();
    }

    public IQueryable<T> GetAll()
    {
        return _dbSet.AsQueryable();
    }

    public async Task<T?> GetByIdAsync(params object[]? keyValues)
    {
        return await _dbSet.FindAsync(keyValues);
    }

    public async Task AddAsync(T entity)
    {
        await _dbContext.AddAsync(entity);
    }

    public void Delete(T entity)
    {
        _dbContext.Remove(entity);
    }

    public void Update(T entity)
    {
        _dbContext.Update(entity);
    }

    public async Task<bool> SaveChangesAsync()
    {
        return await _dbContext.SaveChangesAsync() > 0;
    }
}