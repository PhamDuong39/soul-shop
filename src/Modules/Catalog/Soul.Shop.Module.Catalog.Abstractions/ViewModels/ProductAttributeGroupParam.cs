using System.ComponentModel.DataAnnotations;

namespace Soul.Shop.Module.Catalog.Abstractions.ViewModels;

public class ProductAttributeGroupParam
{
    public int Id { get; set; }

    [Required] public string Name { get; set; }
}