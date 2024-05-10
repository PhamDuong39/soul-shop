using Microsoft.EntityFrameworkCore;
using Soul.Shop.Infrastructure.Data;
using Soul.Shop.Module.Shipping.Abstractions.Entities;

namespace Soul.Shop.Module.Shipping.Data;

public class ShippingCustomModelBuilder : ICustomModelBuilder
{
    public void Build(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<FreightTemplate>().HasQueryFilter(c => !c.IsDeleted);
        modelBuilder.Entity<PriceAndDestination>().HasQueryFilter(c => !c.IsDeleted);
    }
}