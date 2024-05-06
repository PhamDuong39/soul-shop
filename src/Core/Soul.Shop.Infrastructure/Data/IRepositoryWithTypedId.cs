using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Storage;
using Soul.Shop.Infrastructure.Models;

namespace Soul.Shop.Infrastructure.Data;

public interface IRepositoryWithTypedId<T, in TKey> where T : IEntityWithTypedId<TKey>
{
    IQueryable<T> Query();

    void Add(T entity);

    void AddRange(IList<T> entities);

    IDbContextTransaction BeginTransaction();

    int SaveChanges();

    Task<int> SaveChangesAsync();

    void Remove(T entity);

    Task<T> FirstOrDefaultAsync(TKey id);

    IQueryable<T> Query(Expression<Func<T, bool>> predicate);
}