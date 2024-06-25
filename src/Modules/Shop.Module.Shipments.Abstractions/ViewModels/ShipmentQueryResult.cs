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
    /// Tình trạng giao hàng
    /// </summary>
    public ShippingStatus? ShippingStatus { get; set; }

    public string TrackingNumber { get; set; }

    public decimal TotalWeight { get; set; }

    /// <summary>
    /// thời gian vận chuyển
    /// </summary>
    public DateTime? ShippedOn { get; set; }

    /// <summary>
    /// Thời điểm nhận
    /// </summary>
    public DateTime? DeliveredOn { get; set; }

    public string AdminComment { get; set; }

    public string CreatedBy { get; set; }

    public IList<ShipmentQueryItemResult> Items { get; set; } = new List<ShipmentQueryItemResult>();
}
