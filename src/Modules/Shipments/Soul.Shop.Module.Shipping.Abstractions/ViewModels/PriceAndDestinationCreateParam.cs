﻿using System.ComponentModel.DataAnnotations;

namespace Soul.Shop.Module.Shipping.Abstractions.ViewModels;

public class PriceAndDestinationCreateParam
{
    public int FreightTemplateId { get; set; }

    public int CountryId { get; set; }

    public int? StateOrProvinceId { get; set; }

    [Range(0, double.MaxValue)] public decimal MinOrderSubtotal { get; set; }

    [Range(0, double.MaxValue)] public decimal ShippingPrice { get; set; }

    public bool IsEnabled { get; set; }

    public string Note { get; set; }
}