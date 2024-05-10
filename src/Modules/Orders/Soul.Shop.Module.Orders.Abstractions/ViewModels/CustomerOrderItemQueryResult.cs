namespace Soul.Shop.Module.Orders.Abstractions.ViewModels;

public class CustomerOrderItemQueryResult
{
    public int Id { get; set; }

    public int OrderId { get; set; }

    public int ProductId { get; set; }

    public string ProductName { get; set; }

    public string ProductMediaUrl { get; set; }
    public decimal ProductPrice { get; set; }
    public int Quantity { get; set; }

    public decimal DiscountAmount { get; set; }

    public decimal ItemAmount { get; set; }

    public decimal ItemWeight { get; set; }

    public string Note { get; set; }

    public int ShippedQuantity { get; set; }

    public bool IsReviewed { get; set; }
}