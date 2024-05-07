namespace Soul.Shop.Module.Catalog.Abstractions.ViewModels;

public class WidgetSimpleProductSetting
{
    public IList<ProductLinkResult> Products { get; set; } = new List<ProductLinkResult>();
}