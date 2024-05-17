using MediatR;
using Newtonsoft.Json;
using Soul.Shop.Infrastructure.Data;
using Soul.Shop.Module.Orders.Abstractions.Entities;
using Soul.Shop.Module.Orders.Abstractions.Events;
using Soul.Shop.Module.Orders.Abstractions.Models;

namespace Soul.Shop.Module.Orders.Handlers;

public class OrderCreatedCreateOrderHistoryHandler(IRepository<OrderHistory> orderHistoryRepository)
    : INotificationHandler<OrderCreated>
{
    public async Task Handle(OrderCreated notification, CancellationToken cancellationToken)
    {
        var orderHistory = new OrderHistory
        {
            OrderId = notification.OrderId,
            UpdatedById = notification.UserId,
            CreatedById = notification.UserId,
            NewStatus = OrderStatus.New,
            Note = notification.Note
        };

        if (notification.Order != null) orderHistory.OrderSnapshot = JsonConvert.SerializeObject(notification.Order);

        orderHistoryRepository.Add(orderHistory);
        await orderHistoryRepository.SaveChangesAsync();
    }
}