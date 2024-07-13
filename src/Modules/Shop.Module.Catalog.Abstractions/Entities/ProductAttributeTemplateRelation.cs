using Shop.Infrastructure.Models;
using System;

namespace Shop.Module.Catalog.Entities;

/// <summary>
/// Product attribute template and product attribute association table (many-to-many relationship)
/// </summary>
public class ProductAttributeTemplateRelation : EntityBase
{
    public ProductAttributeTemplateRelation()
    {
        CreatedOn = DateTime.Now;
        UpdatedOn = DateTime.Now;
    }

    public int TemplateId { get; set; }

    public ProductAttributeTemplate Template { get; set; }

    public int AttributeId { get; set; }

    public ProductAttribute Attribute { get; set; }

    public bool IsDeleted { get; set; }

    public DateTime CreatedOn { get; set; }

    public DateTime UpdatedOn { get; set; }
}
