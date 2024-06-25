﻿using Shop.Infrastructure.Models;
using Shop.Module.Core.Entities;
using Shop.Module.Orders.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Shop.Module.Shipments.Entities;

public class Shipment : EntityBase
{
    public Shipment()
    {
        CreatedOn = DateTime.Now;
        UpdatedOn = DateTime.Now;
    }

    public int OrderId { get; set; }

    public Order Order { get; set; }

    public decimal TotalWeight { get; set; }

    [StringLength(450)] public string TrackingNumber { get; set; }

    /// <summary>
    /// thời gian vận chuyển
    /// </summary>
    public DateTime? ShippedOn { get; set; }

    /// <summary>
    /// Thời điểm nhận
    /// </summary>
    public DateTime? DeliveredOn { get; set; }

    public string AdminComment { get; set; }

    public int CreatedById { get; set; }

    public User CreatedBy { get; set; }

    public int UpdatedById { get; set; }

    public User UpdatedBy { get; set; }

    public bool IsDeleted { get; set; }

    public DateTime CreatedOn { get; set; }

    public DateTime UpdatedOn { get; set; }

    public IList<ShipmentItem> Items { get; set; } = new List<ShipmentItem>();
}
