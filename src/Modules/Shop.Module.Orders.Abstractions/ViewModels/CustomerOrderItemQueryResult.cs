namespace Shop.Module.Orders.ViewModels;

public class CustomerOrderItemQueryResult
{
    public int Id { get; set; }

    public int OrderId { get; set; }

    public int ProductId { get; set; }

    public string ProductName { get; set; }

    public string ProductMediaUrl { get; set; }

    /// <summary>
    /// Product price (original price)
    /// </summary>
    public decimal ProductPrice { get; set; }

    /// <summary>
    /// Quantity
    /// </summary>
    public int Quantity { get; set; }

    /// <summary>
    /// Discount subtotal (discount total amount)
    /// </summary>
    public decimal DiscountAmount { get; set; }

    /// <summary>
    /// Amount subtotal (product price * quantity - Total amount of discount)
    /// </summary>
    public decimal ItemAmount { get; set; }

    /// <summary>
    /// Subtotal weight
    /// </summary>
    public decimal ItemWeight { get; set; }

    /// <summary>
    /// Notes, discount notes
    /// For example: 20% off for 2 items or more; 40% off for 3 items or more; event price 90; 40% off; event 1xxx, event 2xxx
    /// Event information
    /// </summary>
    public string Note { get; set; }

    public int ShippedQuantity { get; set; }

    /// <summary>
    /// Has it been reviewed?
    /// </summary>
    public bool IsReviewed { get; set; }
}
