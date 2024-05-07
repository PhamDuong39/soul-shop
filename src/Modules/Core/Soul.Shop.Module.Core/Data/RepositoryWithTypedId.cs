using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Soul.Shop.Infrastructure.Data;
using Soul.Shop.Infrastructure.Models;

namespace Soul.Shop.Module.Core.Data;

public class RepositoryWithTypedId<T, TId> : IRepositoryWithTypedId<T, TId> where T : class, IEntityWithTypedId<TId>
{
    public RepositoryWithTypedId(ShopDbContext context)
    {
        Context = context;
        DbSet = Context.Set<T>();
    }

    protected DbContext Context { get; }

    protected DbSet<T> DbSet { get; }

    public void Add(T entity)
    {
        if (entity != null)
            DbSet.Add(entity);
    }

    public void AddRange(IList<T> entities)
    {
        if (entities != null && entities.Count > 0) DbSet.AddRange(entities);
    }

    public IDbContextTransaction BeginTransaction()
    {
        return Context.Database.BeginTransaction();
    }

    public int SaveChanges()
    {
        return Context.SaveChanges();
    }

    public Task<int> SaveChangesAsync()
    {
        return Context.SaveChangesAsync();
    }

    public IQueryable<T> Query()
    {
        return DbSet;
    }

    public void Remove(T entity)
    {
        if (entity != null)
            DbSet.Remove(entity);
    }

    public Task<T> FirstOrDefaultAsync(TId id)
    {
        return DbSet.FirstOrDefaultAsync(c => c.Id.Equals(id));
    }

    public IQueryable<T> Query(Expression<Func<T, bool>> predicate)
    {
        return DbSet.Where(predicate);
    }
}