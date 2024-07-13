using Shop.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Shop.Module.Catalog.Entities;

/// <summary>
/// Product attribute template (e.g. women's clothing attribute template, dress attribute template)
/// </summary>
public class ProductAttributeTemplate : EntityBase
{
    public ProductAttributeTemplate()
    {
        CreatedOn = DateTime.Now;
        UpdatedOn = DateTime.Now;
    }

    [Required] [StringLength(450)] public string Name { get; set; }

    public bool IsDeleted { get; set; }

    public DateTime CreatedOn { get; set; }

    public DateTime UpdatedOn { get; set; }

    public IList<ProductAttributeTemplateRelation> ProductAttributes { get; protected set; } =
        new List<ProductAttributeTemplateRelation>();

    public void AddAttribute(int attributeId)
    {
        var productTempateProductAttribute = new ProductAttributeTemplateRelation
        {
            Template = this,
            AttributeId = attributeId
        };
        ProductAttributes.Add(productTempateProductAttribute);
    }
}
