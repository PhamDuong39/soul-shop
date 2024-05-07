using System.ComponentModel.DataAnnotations;
using Soul.Shop.Infrastructure.Models;
using Soul.Shop.Module.Catalog.Abstractions.Models;

namespace Soul.Shop.Module.Catalog.Abstractions.Entities;

public class ProductOption : EntityBase
{
    public ProductOption()
    {
        CreatedOn = DateTime.Now;
        UpdatedOn = DateTime.Now;
    }

    public ProductOption(int id) : this()
    {
        Id = id;
    }

    [Required] [StringLength(450)] public string Name { get; set; }

    public OptionDisplayType DisplayType { get; set; }

    public bool IsDeleted { get; set; }

    public DateTime CreatedOn { get; set; }

    public DateTime UpdatedOn { get; set; }
}