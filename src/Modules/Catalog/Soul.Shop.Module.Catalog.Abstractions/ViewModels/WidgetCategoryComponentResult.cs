﻿using System.Collections.Generic;

namespace Soul.Shop.Module.Catalog.Abstractions.ViewModels;

public class WidgetCategoryComponentResult
{
    public int Id { get; set; }

    public int WidgetZoneId { get; set; }

    public int WidgetId { get; set; }

    public string WidgetName { get; set; }

    public IList<CategoryHomeResult> Categorys { get; set; } = new List<CategoryHomeResult>();
}