namespace Soul.Shop.Module.Orders.Abstractions.ViewModels;

public class OrderCreateItemParam
{
    public int Id { get; set; }

    public decimal ProductPrice { get; set; }

    public int Quantity { get; set; }

    public decimal DiscountAmount { get; set; }

    public decimal ItemAmount { get; set; }

    public decimal ItemWeight { get; set; }

    public string Note { get; set; }
}