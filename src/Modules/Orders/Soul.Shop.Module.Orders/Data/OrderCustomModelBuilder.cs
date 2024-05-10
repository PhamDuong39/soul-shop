using Microsoft.EntityFrameworkCore;
using Soul.Shop.Infrastructure.Data;
using Soul.Shop.Module.Core.Abstractions.Entities;
using Soul.Shop.Module.Orders.Abstractions.Data;
using Soul.Shop.Module.Orders.Abstractions.Entities;

namespace Soul.Shop.Module.Orders.Data;

public class OrderCustomModelBuilder : ICustomModelBuilder
{
    public void Build(ModelBuilder modelBuilder)
    {
        const string module = "Orders";

        modelBuilder.Entity<Order>().HasQueryFilter(c => !c.IsDeleted);
        modelBuilder.Entity<OrderAddress>().HasQueryFilter(c => !c.IsDeleted);
        modelBuilder.Entity<OrderHistory>().HasQueryFilter(c => !c.IsDeleted);
        modelBuilder.Entity<OrderItem>().HasQueryFilter(c => !c.IsDeleted);

        modelBuilder.Entity<Order>(u =>
        {
            u.HasOne(x => x.ShippingAddress)
                .WithMany()
                .HasForeignKey(x => x.ShippingAddressId);
        });

        modelBuilder.Entity<Order>()
            .HasIndex(b => b.No)
            .IsUnique();

        modelBuilder.Entity<Order>(u =>
        {
            u.HasOne(x => x.BillingAddress)
                .WithMany()
                .HasForeignKey(x => x.BillingAddressId);
        });

        var opt = new OrderOptions();
        modelBuilder.Entity<AppSetting>().HasData(
            new AppSetting(OrderKeys.OrderAutoCanceledTimeForMinute)
            {
                Module = module,
                IsVisibleInCommonSettingPage = true,
                Value = opt.OrderAutoCanceledTimeForMinute.ToString(),
                Type = typeof(int).FullName,
                Note = "Automatic order cancellation time after timeout after placing the order (unit: minutes)"
            },
            new AppSetting(OrderKeys.OrderAutoCompleteTimeForMinute)
            {
                Module = module,
                IsVisibleInCommonSettingPage = true,
                Value = opt.OrderAutoCompleteTimeForMinute.ToString(),
                Type = typeof(int).FullName,
                Note =
                    "Time to automatically complete the order after timeout after order payment (if the buyer does not confirm receipt within the specified time, the system will automatically confirm receipt and complete the order, unit: minutes)"
            },
            new AppSetting(OrderKeys.OrderCompleteAutoReviewTimeForMinute)
            {
                Module = module,
                IsVisibleInCommonSettingPage = true,
                Value = opt.OrderCompleteAutoReviewTimeForMinute.ToString(),
                Type = typeof(int).FullName,
                Note =
                    "Timeout for automatic praise after the order is completed (if the buyer fails to rate within the specified time, the system will automatically rate the review, unit: minutes)"
            });
    }
}