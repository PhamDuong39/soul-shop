using Soul.Shop.Infrastructure.Data;
using Soul.Shop.Infrastructure.Models;

namespace Soul.Shop.Module.Core.Data;

public class Repository<T>(ShopDbContext context) : RepositoryWithTypedId<T, int>(context), IRepository<T>
    where T : class, IEntityWithTypedId<int>;