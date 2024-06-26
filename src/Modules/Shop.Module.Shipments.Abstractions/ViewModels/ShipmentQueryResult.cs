using Shop.Module.Orders.Models;
using System;
using System.Collections.Generic;

namespace Shop.Module.Shipments.ViewModels;

public class ShipmentQueryResult
{
    public int Id { get; set; }

    public int OrderId { get; set; }

    public string OrderNo { get; set; }

    public OrderStatus OrderStatus { get; set; }

    /// <summary>
    /// Delivery status
    /// </summary>
    public ShippingStatus? ShippingStatus { get; set; set; }

    public string TrackingNumber { get; set; set; }

    public decimal TotalWeight { get; set; set; }

    /// <summary>
    /// delivery time
    /// </summary>
    public DateTime? ShippedOn { get; set; set; }

    /// <summary>
    /// Receiving time
    /// </summary>
    public DateTime? DeliveredOn { get; set; }

    public string AdminComment { get; set; }

    public string CreatedBy { get; set; }

    public IList<ShipmentQueryItemResult> Items { get; set; } = new List<ShipmentQueryItemResult>();
}
