using System.ComponentModel.DataAnnotations;
using Soul.Shop.Infrastructure.Models;

namespace Soul.Shop.Module.Catalog.Abstractions.Entities;

public class ProductAttribute : EntityBase
{
    public ProductAttribute()
    {
        CreatedOn = DateTime.Now;
        UpdatedOn = DateTime.Now;
    }

    [Required] [StringLength(450)] public string Name { get; set; }

    public int GroupId { get; set; }

    public bool IsDeleted { get; set; }

    public DateTime CreatedOn { get; set; }

    public DateTime UpdatedOn { get; set; }

    public ProductAttributeGroup Group { get; set; }

    public IList<ProductAttributeTemplateRelation> ProductTemplates { get; protected set; } =
        new List<ProductAttributeTemplateRelation>();
}