﻿namespace Soul.Shop.Module.Orders.Abstractions.ViewModels;

public class OrderShipmentItemParam
{
    public int OrderItemId { get; set; }

    public int QuantityToShip { get; set; }
}