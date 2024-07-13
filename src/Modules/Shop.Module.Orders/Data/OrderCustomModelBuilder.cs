using Microsoft.EntityFrameworkCore;
using Shop.Infrastructure.Data;
using Shop.Module.Core.Entities;
using Shop.Module.Orders.Entities;
using Shop.Module.Orders.Events;

namespace Shop.Module.Orders.Data;

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
                Note = "Time for automatic cancellation of order after order placement (unit: minutes)"
            },
            new AppSetting(OrderKeys.OrderAutoCompleteTimeForMinute)
            {
                Module = module,
                IsVisibleInCommonSettingPage = true,
                Value = opt.OrderAutoCompleteTimeForMinute.ToString(),
                Type = typeof(int).FullName,
                Note = "Time to automatically complete the order after the order is paid (if the buyer does not confirm receipt within the specified time, the system will automatically confirm receipt and complete the order, unit: minutes)"
            },
            new AppSetting(OrderKeys.OrderCompleteAutoReviewTimeForMinute)
            {
                Module = module,
                IsVisibleInCommonSettingPage = true,
                Value = opt.OrderCompleteAutoReviewTimeForMinute.ToString(),
                Type = typeof(int).FullName,
                Note = "Timeout for automatic positive review after order completion (if the buyer does not review within the specified time, the system will automatically give a positive review, unit: minutes)"
            });
    }
}
