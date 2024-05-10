using Soul.Shop.Module.Catalog.Abstractions.Models;

namespace Soul.Shop.Module.Catalog.Abstractions.ViewModels;

public class ProductOptionResult
{
    public int Id { get; set; }

    public string Name { get; set; }

    public OptionDisplayType DisplayType { get; set; }
}