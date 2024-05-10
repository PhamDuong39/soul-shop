namespace Soul.Shop.Module.Orders.Abstractions.Models;

public enum ShippingStatus
{
    NoShipping = 0,

    NotYetShipped = 20,

    PartiallyShipped = 25,

    Shipped = 30,

    Delivered = 40
}