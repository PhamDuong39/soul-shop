namespace Soul.Shop.Module.ShoppingCart.Abstractions.ViewModels;

public class DeleteItemParam
{
    public IList<int> ProductIds { get; set; } = new List<int>();
}