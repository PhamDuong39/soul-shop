﻿using Soul.Shop.Infrastructure.Models;
using Soul.Shop.Module.Catalog.Abstractions.Models;

namespace Soul.Shop.Module.Catalog.Abstractions.Entities;

public class ProductLink : EntityBase
{
    public ProductLink()
    {
        CreatedOn = DateTime.Now;
        UpdatedOn = DateTime.Now;
    }

    public int ProductId { get; set; }

    public Product Product { get; set; }

    public int LinkedProductId { get; set; }

    public Product LinkedProduct { get; set; }

    public ProductLinkType LinkType { get; set; }

    public bool IsDeleted { get; set; }

    public DateTime CreatedOn { get; set; }

    public DateTime UpdatedOn { get; set; }
}