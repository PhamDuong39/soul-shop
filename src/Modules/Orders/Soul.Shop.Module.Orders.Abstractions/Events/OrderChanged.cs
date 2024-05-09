using MediatR;
using Soul.Shop.Module.Orders.Abstractions.Entities;
using Soul.Shop.Module.Orders.Abstractions.Models;

namespace Soul.Shop.Module.Orders.Abstractions.Events;

public class OrderChanged : INotification
{
    public int OrderId { get; set; }

    public Order Order { get; set; }

    public OrderStatus? OldStatus { get; set; }

    public OrderStatus NewStatus { get; set; }

    public int UserId { get; set; }

    public string Note { get; set; }
}