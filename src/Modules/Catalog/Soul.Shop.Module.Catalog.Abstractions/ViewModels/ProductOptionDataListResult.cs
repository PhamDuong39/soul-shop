﻿using Soul.Shop.Module.Catalog.Abstractions.Models;

namespace Soul.Shop.Module.Catalog.Abstractions.ViewModels;

public class ProductOptionDataListResult
{
    public int Id { get; set; }

    public int OptionId { get; set; }

    public string OptionName { get; set; }

    public OptionDisplayType OptionDisplayType { get; set; }

    public string Value { get; set; }

    public string Display { get; set; }

    public string Description { get; set; }

    public bool IsPublished { get; set; }

    public DateTime CreatedOn { get; set; }

    public DateTime UpdatedOn { get; set; }
}