using System.Collections.Generic;

namespace Soul.Shop.Module.Core.Abstractions.ViewModels;

public class WidgetCarouselResult : WidgetResultBase
{
    public IList<WidgetCarouselItem> Items = new List<WidgetCarouselItem>();
}