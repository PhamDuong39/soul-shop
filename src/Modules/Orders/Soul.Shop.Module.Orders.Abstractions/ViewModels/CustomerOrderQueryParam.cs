using Soul.Shop.Module.Orders.Abstractions.Models;

namespace Soul.Shop.Module.Orders.Abstractions.ViewModels;

public class CustomerOrderQueryParam
{
    public IList<OrderStatus> OrderStatus { get; set; } = new List<OrderStatus>();

    public ShippingStatus? ShippingStatus { get; set; }
}