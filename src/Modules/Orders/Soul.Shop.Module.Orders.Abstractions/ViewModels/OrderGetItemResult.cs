namespace Soul.Shop.Module.Orders.Abstractions.ViewModels;

public class OrderGetItemResult
{
    public int Id { get; set; }

    public int OrderItemId { get; set; }

    public string Name { get; set; }

    public string MediaUrl { get; set; }

    public decimal ProductPrice { get; set; }
    public int Quantity { get; set; }

    public decimal DiscountAmount { get; set; }

    public decimal ItemAmount { get; set; }

    public decimal ItemWeight { get; set; }

    public string Note { get; set; }
    public int ShippedQuantity { get; set; }

    public int? AvailableQuantity { get; set; }

    public int QuantityToShip => Quantity - ShippedQuantity;
}