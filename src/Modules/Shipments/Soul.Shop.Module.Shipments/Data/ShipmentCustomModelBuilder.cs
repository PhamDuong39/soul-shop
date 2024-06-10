using Microsoft.EntityFrameworkCore;
using Soul.Shop.Infrastructure.Data;
using Soul.Shop.Module.Shipments.Abstractions.Entities;

namespace Soul.Shop.Module.Shipments.Data;

public class ShipmentCustomModelBuilder : ICustomModelBuilder
{
    public void Build(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Shipment>().HasQueryFilter(c => !c.IsDeleted);
        modelBuilder.Entity<ShipmentItem>().HasQueryFilter(c => !c.IsDeleted);

        modelBuilder.Entity<Shipment>().HasIndex(c => c.IsDeleted);
        modelBuilder.Entity<Shipment>().HasIndex(c => c.TrackingNumber);
    }
}
