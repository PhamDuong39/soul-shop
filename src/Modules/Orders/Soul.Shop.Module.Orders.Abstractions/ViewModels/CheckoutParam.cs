namespace Soul.Shop.Module.Orders.Abstractions.ViewModels;

public class CheckoutParam
{
    public int? UserAddressId { get; set; }

    public int CustomerId { get; set; }

    public IList<CheckoutItemParam> Items { get; set; } = new List<CheckoutItemParam>();
}