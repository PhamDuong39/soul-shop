﻿using Soul.Shop.Infrastructure.Models;
using Soul.Shop.Module.Catalog.Abstractions.Entities;
using Soul.Shop.Module.Core.Abstractions.Entities;
using Soul.Shop.Module.Orders.Abstractions.Entities;

namespace Soul.Shop.Module.Shipments.Abstractions.Entities;

public class ShipmentItem : EntityBase
{
    public ShipmentItem()
    {
        CreatedOn = DateTime.Now;
        UpdatedOn = DateTime.Now;
    }

    public int ShipmentId { get; set; }

    public Shipment Shipment { get; set; }

    public int OrderItemId { get; set; }

    // bug: should be OrderItem
    public OrderItem OrderItem { get; set; }

    public int ProductId { get; set; }
    public Product Product { get; set; }

    public int Quantity { get; set; }

    public int CreatedById { get; set; }

    public User CreatedBy { get; set; }

    public int UpdatedById { get; set; }

    public User UpdatedBy { get; set; }

    public bool IsDeleted { get; set; }

    public DateTime CreatedOn { get; set; }

    public DateTime UpdatedOn { get; set; }
}