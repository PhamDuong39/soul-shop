﻿using System.ComponentModel.DataAnnotations;
using Soul.Shop.Infrastructure.Models;

namespace Soul.Shop.Module.Catalog.Abstractions.Entities;

public class ProductAttributeData : EntityBase
{
    public int AttributeId { get; set; }

    public ProductAttribute Attribute { get; set; }

    [StringLength(450)] [Required] public string Value { get; set; }

    public string Description { get; set; }

    public bool IsPublished { get; set; }

    public bool IsDeleted { get; set; }

    public DateTime CreatedOn { get; set; } = DateTime.Now;

    public DateTime UpdatedOn { get; set; } = DateTime.Now;
}