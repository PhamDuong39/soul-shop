using System.Collections.Generic;

namespace Soul.Shop.Module.Catalog.Abstractions.ViewModels;

public class WidgetCategorySetting
{
    public IList<int> CategoryIds { get; set; } = new List<int>();
}