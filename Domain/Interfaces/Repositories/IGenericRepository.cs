using System.Linq.Expressions;

namespace Domain.Interfaces.Repositories;

public interface IGenericRepository<T> where T : class
{
    // The key to deferred execution
    IQueryable<T> EntityQuery { get; }

    // Basic Persistence
    Task AddAsync(T entity);
    void Update(T entity);
    void Delete(T entity);
    Task<T?> GetByIdAsync(object id);

    // Helpers for simple cases
    Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null);
}
