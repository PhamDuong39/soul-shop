using System.ComponentModel.DataAnnotations;
using Soul.Shop.Infrastructure.Models;

namespace Soul.Shop.Module.Catalog.Abstractions.Entities;

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