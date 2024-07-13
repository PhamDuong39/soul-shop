namespace Shop.Module.Orders.ViewModels;

public class OrderGetItemResult
{
    public int Id { get; set; }

    public int OrderItemId { get; set; }

    public string Name { get; set; }

    public string MediaUrl { get; set; }

    /// <summary>
    /// Product price (original price) (allowed to modify)
    /// </summary>
    public decimal ProductPrice { get; set; }

    /// <summary>
    /// Quantity (allowed to modify)
    /// </summary>
    public int Quantity { get; set; }

    /// <summary>
    /// Discount subtotal (discount total amount) (allowed to modify)
    /// </summary>
    public decimal DiscountAmount { get; set; }

    /// <summary>
    /// Amount subtotal (product price * quantity - discount total amount) (allowed to modify)
    /// </summary>
    public decimal ItemAmount { get; set; }

    /// <summary>
    /// Weight subtotal
    /// </summary>
    public decimal ItemWeight { get; set; }

    /// <summary>
    /// Notes, discount notes
    /// For example: 20% off for 2 items or more; 40% off for 3 items or more; event price 90; 40% off; event 1xxx, event 2xxx
    /// Event information
    /// </summary>
    public string Note { get; set; }

    public int ShippedQuantity { get; set; }

    public int? AvailableQuantity { get; set; }

    public int QuantityToShip => Quantity - ShippedQuantity;
}
