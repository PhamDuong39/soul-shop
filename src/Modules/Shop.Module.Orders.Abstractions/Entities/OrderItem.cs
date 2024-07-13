using Newtonsoft.Json;
using Shop.Infrastructure.Models;
using Shop.Module.Catalog.Entities;
using Shop.Module.Core.Entities;
using System;

namespace Shop.Module.Orders.Entities;

public class OrderItem : EntityBase
{
    public OrderItem()
    {
        CreatedOn = DateTime.Now;
        UpdatedOn = DateTime.Now;
    }

    public int OrderId { get; set; }

    [JsonIgnore] public Order Order { get; set; }

    public int ProductId { get; set; }

    [JsonIgnore] public Product Product { get; set; }

    /// <summary>
    /// Product price (original price) (modification allowed)
    /// </summary>
    public decimal ProductPrice { get; set; }

    /// <summary>
    /// Product name (snapshot)
    /// </summary>
    public string ProductName { get; set; }

    /// <summary>
    /// Product image (snapshot)
    /// </summary>
    public string ProductMediaUrl { get; set; }

    /// <summary>
    /// Quantity (modification allowed)
    /// </summary>
    public int Quantity { get; set; }

    /// <summary>
    /// Shipped quantity
    /// </summary>
    public int ShippedQuantity { get; set; }

    /// <summary>
    /// Discount subtotal (total amount of discount) (modification allowed)
    /// </summary>
    public decimal DiscountAmount { get; set; }

    /// <summary>
    /// Amount subtotal (product price * quantity - total amount of discount) (modification allowed)
    /// </summary>
    public decimal ItemAmount { get; set; }

    /// <summary>
    /// Weight subtotal
    /// </summary>
    public decimal ItemWeight { get; set; }

    /// <summary>
    /// Remarks, discount remarks
    /// For example: 20% off for 2 pieces or more; 40% off for 3 pieces or more; Promotion price 90; 40% off; Promotion 1xxx, Promotion 2xxx
    /// Promotion information
    /// </summary>
    public string Note { get; set; }

    public int CreatedById { get; set; }

    [JsonIgnore] public User CreatedBy { get; set; }

    public int UpdatedById { get; set; }

    [JsonIgnore] public User UpdatedBy { get; set; }

    public bool IsDeleted { get; set; }

    public DateTime CreatedOn { get; set; }

    public DateTime UpdatedOn { get; set; }
}
