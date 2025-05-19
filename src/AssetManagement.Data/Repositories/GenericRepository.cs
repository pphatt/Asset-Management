using AssetManagement.Domain.Repositories;
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

    public void Add(T entity)
    {
        _dbContext.Add(entity);
    }

    public void Delete(T entity)
    {
        _dbContext.Remove(entity);
    }

    public IQueryable<T> GetAll()
    {
        return _dbSet.AsQueryable();
    }

    public async Task<T?> GetByIdAsync(params object[]? keyValues)
    {
        return await _dbSet.FindAsync(keyValues);
    }

    public void Update(T entity)
    {
        _dbContext.Update(entity);
    }
}