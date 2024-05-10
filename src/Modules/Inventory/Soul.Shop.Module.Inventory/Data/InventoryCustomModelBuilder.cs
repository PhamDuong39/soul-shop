using Microsoft.EntityFrameworkCore;
using Soul.Shop.Infrastructure.Data;
using Soul.Shop.Module.Inventory.Abstractions.Entities;

namespace Soul.Shop.Module.Inventory.Data;

public class InventoryCustomModelBuilder : ICustomModelBuilder
{
    public void Build(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Stock>().HasQueryFilter(c => !c.IsDeleted);
        modelBuilder.Entity<StockHistory>().HasQueryFilter(c => !c.IsDeleted);
        modelBuilder.Entity<Warehouse>().HasQueryFilter(c => !c.IsDeleted);
    }
}