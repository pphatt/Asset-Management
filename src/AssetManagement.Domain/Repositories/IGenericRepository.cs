namespace AssetManagement.Domain.Repositories;

public interface IGenericRepository<T> where T : class
{
    IQueryable<T> GetAll();

    Task<T?> GetByIdAsync(params object[]? keyValues);

    void Add(T entity);

    void Update(T entity);

    void Delete(T entity);
}