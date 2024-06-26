﻿using Newtonsoft.Json;

namespace Soul.Shop.Module.Catalog.Abstractions.ViewModels;

public class ProductGetOptionValueResult
{
    public int Id { get; set; }

    public string Value { get; set; }

    public string Display { get; set; }

    public int DisplayOrder { get; set; }

    public int? MediaId { get; set; }

    public string MediaUrl { get; set; }

    public bool IsDefault { get; set; }
}