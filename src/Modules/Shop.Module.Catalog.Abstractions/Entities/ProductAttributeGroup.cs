using Shop.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Shop.Module.Catalog.Entities;

/// <summary>
/// Product attribute groups (shoe attributes, sports shoe attributes, women's clothing attributes, men's clothing attributes, common attributes, key attributes, jewelry attributes, electronic product attributes, etc.)
/// </summary>
public class ProductAttributeGroup : EntityBase
{
    public ProductAttributeGroup()
    {
        CreatedOn = DateTime.Now;
        UpdatedOn = DateTime.Now;
    }

    [Required] [StringLength(450)] public string Name { get; set; }

    public IList<ProductAttribute> Attributes { get; set; } = new List<ProductAttribute>();

    public bool IsDeleted { get; set; }

    public DateTime CreatedOn { get; set; }

    public DateTime UpdatedOn { get; set; }
}
