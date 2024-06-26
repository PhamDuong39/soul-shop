using System;

namespace Shop.Module.Shipments.ViewModels;

public class ShipmentQueryItemResult
{
    public int Id { get; set; }

    public int ShipmentId { get; set; }

    public int OrderItemId { get; set; }

    public int ProductId { get; set; }

    /// <summary>
    /// Product name (snapshot)
    /// </summary>
    public string ProductName { get; set; set; }

    /// <summary>
    /// Product image (snapshot)
    /// </summary>
    public string ProductMediaUrl { get; set; set; }

    /// <summary>
    /// Number of orders
    /// </summary>
    public int OrderedQuantity { get; set; set; }

    /// <summary>
    /// Shipping quantity
    /// </summary>
    public int ShippedQuantity { get; set; }

    public int Quantity { get; set; }

    public DateTime CreatedOn { get; set; }

    public DateTime UpdatedOn { get; set; }
}
