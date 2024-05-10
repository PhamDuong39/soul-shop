using System.ComponentModel.DataAnnotations;

namespace Soul.Shop.Module.Catalog.Abstractions.ViewModels;

public class NameParam
{
    [Required] public string Name { get; set; }
}