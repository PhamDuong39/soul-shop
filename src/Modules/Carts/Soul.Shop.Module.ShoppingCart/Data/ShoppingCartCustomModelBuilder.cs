using Microsoft.EntityFrameworkCore;
using Soul.Shop.Infrastructure.Data;
using Soul.Shop.Module.ShoppingCart.Abstractions.Entities;

namespace Soul.Shop.Module.ShoppingCart.Data;

public class ShoppingCartCustomModelBuilder : ICustomModelBuilder
{
    public void Build(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Cart>().HasQueryFilter(c => !c.IsDeleted);
        modelBuilder.Entity<CartItem>().HasQueryFilter(c => !c.IsDeleted);
    }
}