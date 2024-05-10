using Soul.Shop.Infrastructure.Models;

namespace Soul.Shop.Module.Catalog.Abstractions.Entities;

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