namespace Soul.Shop.Module.ShoppingCart.Abstractions.ViewModels;

public class CheckedItemParam
{
    public IList<int> ProductIds { get; set; } = new List<int>();

    public bool IsChecked { get; set; }
}