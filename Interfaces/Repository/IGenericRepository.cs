namespace Auth.Api.Interfaces;

public interface IGenericRepository<T> where T : class
{
    Task<bool> SaveAsync(T entity);
    Task<bool> DeleteAsync(int id);
    Task<T?> GetByIdAsync(int id);
    Task<IEnumerable<T>> GetAllAsync();
    Task UpdateAsync(T entity);
}
