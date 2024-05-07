﻿using Soul.Shop.Infrastructure.Models;
using Soul.Shop.Module.Core.Abstractions.Entities;

namespace Soul.Shop.Module.Catalog.Abstractions.Entities;

public class ProductRecentlyViewed : EntityBase
{
    public ProductRecentlyViewed()
    {
        CreatedOn = DateTime.Now;
        LatestViewedOn = DateTime.Now;
    }

    public int ProductId { get; set; }

    public Product Product { get; set; }

    public int CustomerId { get; set; }

    public User Customer { get; set; }

    public int ViewedCount { get; set; }

    public bool IsDeleted { get; set; }

    public DateTime CreatedOn { get; set; }

    public DateTime LatestViewedOn { get; set; }
}