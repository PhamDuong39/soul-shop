namespace Shop.Module.Orders.ViewModels;

public class OrderCreateItemParam
{
    /// <summary>
    /// ProductId
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Product price (original price) (modification allowed)
    /// </summary>
    public decimal ProductPrice { get; set; }

    /// <summary>
    /// Quantity (modification allowed)
    /// </summary>
    public int Quantity { get; set; }

    /// <summary>
    /// Discount subtotal (discount total amount) (modification allowed)
    /// </summary>
    public decimal DiscountAmount { get; set; }

    /// <summary>
    /// Amount subtotal (product price * quantity - discount total amount) (modification allowed)
    /// </summary>
    public decimal ItemAmount { get; set; }

    /// <summary>
    /// Subtotal weight
    /// </summary>
    public decimal ItemWeight { get; set; }

    /// <summary>
    /// Notes, discount notes
    /// For example: 20% off for 2 items or more; 40% off for 3 items or more; promotional price 90; 40% off; promotional 1xxx, promotional 2xxx
    /// Promotional information
    /// </summary>
    public string Note { get; set; }
}
