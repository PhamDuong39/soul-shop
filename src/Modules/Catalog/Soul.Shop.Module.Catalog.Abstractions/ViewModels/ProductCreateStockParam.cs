﻿namespace Soul.Shop.Module.Catalog.Abstractions.ViewModels;

public class ProductCreateStockParam
{
    public int Quantity { get; set; }

    public int Id { get; set; }

    public int DisplayOrder { get; set; }

    public bool IsEnabled { get; set; }
}