﻿using Soul.Shop.Infrastructure.Models;

namespace Soul.Shop.Module.Catalog.Abstractions.Entities;

public class ProductAttributeValue : EntityBase
{
    public ProductAttributeValue()
    {
        CreatedOn = DateTime.Now;
        UpdatedOn = DateTime.Now;
    }

    public int AttributeId { get; set; }

    public ProductAttribute Attribute { get; set; }

    public int ProductId { get; set; }

    public Product Product { get; set; }

    public string Value { get; set; }

    public string Description { get; set; }

    public bool IsDeleted { get; set; }

    public DateTime CreatedOn { get; set; }

    public DateTime UpdatedOn { get; set; }
}