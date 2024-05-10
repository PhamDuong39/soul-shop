﻿using Soul.Shop.Infrastructure.Models;
using Soul.Shop.Module.Core.Abstractions.Entities;

namespace Soul.Shop.Module.Shipping.Abstractions.Entities;

public class PriceAndDestination : EntityBase
{
    public PriceAndDestination()
    {
        CreatedOn = DateTime.Now;
        UpdatedOn = DateTime.Now;
    }

    public int FreightTemplateId { get; set; }

    public FreightTemplate FreightTemplate { get; set; }

    public Country Country { get; set; }

    public int CountryId { get; set; }

    public StateOrProvince StateOrProvince { get; set; }

    public int? StateOrProvinceId { get; set; }

    public decimal MinOrderSubtotal { get; set; }

    public decimal ShippingPrice { get; set; }

    public string Note { get; set; }

    public bool IsEnabled { get; set; }

    public bool IsDeleted { get; set; }

    public DateTime CreatedOn { get; set; }

    public DateTime UpdatedOn { get; set; }
}