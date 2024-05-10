using System.ComponentModel.DataAnnotations;
using Soul.Shop.Infrastructure.Models;

namespace Soul.Shop.Module.Catalog.Abstractions.Entities;

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