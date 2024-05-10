using MediatR;
using Newtonsoft.Json;
using Soul.Shop.Infrastructure.Data;
using Soul.Shop.Module.Orders.Abstractions.Entities;
using Soul.Shop.Module.Orders.Abstractions.Events;

namespace Soul.Shop.Module.Orders.Handlers;

public class OrderChangedCreateOrderHistoryHandler(IRepository<OrderHistory> orderHistoryRepository)
    : INotificationHandler<OrderChanged>
{
    public async Task Handle(OrderChanged notification, CancellationToken cancellationToken)
    {
        var orderHistory = new OrderHistory
        {
            OrderId = notification.OrderId,
            CreatedById = notification.UserId,
            UpdatedById = notification.UserId,
            OldStatus = notification.OldStatus,
            NewStatus = notification.NewStatus,
            Note = notification.Note
        };

        if (notification.Order != null) orderHistory.OrderSnapshot = JsonConvert.SerializeObject(notification.Order);

        orderHistoryRepository.Add(orderHistory);
        await orderHistoryRepository.SaveChangesAsync();
    }
}