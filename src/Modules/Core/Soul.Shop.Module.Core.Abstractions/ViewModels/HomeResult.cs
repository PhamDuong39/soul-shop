using System.Collections.Generic;

namespace Soul.Shop.Module.Core.Abstractions.ViewModels;

public class HomeResult
{
    public IList<HomeWidgetInstanceResult> WidgetInstances { get; set; } = new List<HomeWidgetInstanceResult>();
}