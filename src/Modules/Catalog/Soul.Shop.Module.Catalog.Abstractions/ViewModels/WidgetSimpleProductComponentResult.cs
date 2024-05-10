﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Soul.Shop.Module.Catalog.Abstractions.ViewModels;

public class WidgetSimpleProductComponentResult
{
    public int Id { get; set; }

    public int WidgetZoneId { get; set; }

    public int WidgetId { get; set; }

    public string WidgetName { get; set; }

    public WidgetSimpleProductSetting Setting { get; set; }

    public IList<GoodsListResult> Products { get; set; } = new List<GoodsListResult>();
}