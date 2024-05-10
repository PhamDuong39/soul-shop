using Soul.Shop.Infrastructure.Models;

namespace Soul.Shop.Infrastructure.Data;

public interface IRepository<T> : IRepositoryWithTypedId<T, int> where T : IEntityWithTypedId<int>
{
}